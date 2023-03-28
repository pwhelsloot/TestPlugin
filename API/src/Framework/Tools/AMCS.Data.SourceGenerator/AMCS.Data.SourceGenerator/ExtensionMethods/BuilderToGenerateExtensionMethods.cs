using AMCS.Data.SourceGenerator.Helpers;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace AMCS.Data.SourceGenerator.ExtensionMethods
{
  internal static class BuilderToGenerateExtensionMethods
  {
    /// <summary>
    /// Generate the compilation units from the filtered files in the Syntax Trees.
    /// </summary>
    /// <param name="builders">The BuilderToGenerate List</param>
    /// <param name="context"><see cref="GeneratorExecutionContext"/></param>
    /// <returns><see cref="List<(string FileName, CompilationUnitSyntax CompilationUnit)>"/></returns>
    public static List<(string FileName, CompilationUnitSyntax CompilationUnit)> GenerateCompilationUnits(
      this Dictionary<string, BuilderToGenerate> builders,
      GeneratorExecutionContext context)
    {
      List<(string FileName, CompilationUnitSyntax CompilationUnit)> compilationUnits = new();

      foreach (BuilderToGenerate builder in builders.Values)
      {
        context.CancellationToken.ThrowIfCancellationRequested();

        var builderClass = ClassDeclaration(builder.BuilderName)
          .AddModifiers(builder.Accessibility.GetModifiers())
          .AddModifiers(Token(SyntaxKind.PartialKeyword));

        // Add instance fields
        builderClass = builderClass.WithMembers(new SyntaxList<MemberDeclarationSyntax>(
          builder.Properties.Select(property =>
            FieldDeclaration(
              VariableDeclaration(property.TypeSyntax)
              .AddVariables(VariableDeclarator(Identifier(property.FieldName)))
            ).AddModifiers(Token(SyntaxKind.PrivateKeyword))
          ))
        );

        // Add reference fields
        if (builder.ReferenceEntities.Any())
        {
          foreach (var refEntity in builder.ReferenceEntities)
          {
            builderClass = builderClass.AddMembers(
              FieldDeclaration(
                VariableDeclaration(
                  GenericName(Identifier("Func"))
                  .WithTypeArgumentList(
                    TypeArgumentList(
                      SeparatedList<TypeSyntax>(
                        new SyntaxNodeOrToken[]{
                          IdentifierName("ISessionToken"),
                          Token(SyntaxKind.CommaToken),
                          IdentifierName("IDataSession"),
                          Token(SyntaxKind.CommaToken),
                          refEntity.TypeSyntax
                        }
                      )
                    )
                  )
                ).AddVariables(VariableDeclarator(Identifier(refEntity.FieldName)))
              ).WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
            );
          }
        }

        // Add Entity field
        builderClass = builderClass.AddMembers(
          FieldDeclaration(
            VariableDeclaration(IdentifierName(builder.EntityType.Name))
              .AddVariables(VariableDeclarator(Identifier(builder.EntityType.Name.FirstCharToLowerCase())))
          ).WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword))));

        // Add parameterless constructor
        builderClass = builderClass.AddMembers(
          ConstructorDeclaration(builder.BuilderName)
            .AddModifiers(Token(SyntaxKind.PublicKeyword))
            .AddBodyStatements()
        );

        if (builder.DeDuplicate.Any())
        {
          builderClass = builder.AddFindExistingEntityMethod(builderClass);
        }

        builderClass = builder.AddMutationMethods(builderClass);

        builderClass = builder.AddEntityReferenceMethods(builderClass);

        builderClass = CreateExtendingPartialMethods(builder, builderClass);

        if (!TryAddBuildMethod(builder, ref builderClass)) continue;

        var originalCompilationUnit = builder.TypeNode.Ancestors().OfType<CompilationUnitSyntax>().FirstOrDefault();
        if (originalCompilationUnit is null) continue;

        var originalNamespaceDeclaration = builder.TypeNode.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault();

        var declaration = originalNamespaceDeclaration is null
          ? (MemberDeclarationSyntax)builderClass
          : NamespaceDeclaration(
            originalNamespaceDeclaration.Name,
            originalNamespaceDeclaration.Externs,
            originalNamespaceDeclaration.Usings,
            new SyntaxList<MemberDeclarationSyntax>(builderClass));

        var defaultUsings = new List<string>
        {
          "AMCS.Data",
          "AMCS.Data.Entity",
          "AMCS.Data.Server",
          "AMCS.Data.Server.SQL.Querying",
          "System.Diagnostics"
        };

        var mergedUsings = originalCompilationUnit.Usings;
        var mergedUsingsList = mergedUsings.Select(property => property.Name.ToString()).ToList();

        /* Only add unique Usings */
        foreach (var defaultUsing in defaultUsings)
        {
          if (!mergedUsingsList.Contains(defaultUsing))
          {
            mergedUsings = mergedUsings.Add(UsingDirective(IdentifierName(defaultUsing)));
          }
        }

        var compilationUnit = CompilationUnit()
          .WithExterns(originalCompilationUnit.Externs)
          .WithUsings(mergedUsings)
          .AddMembers(declaration)
          .WithTrailingTrivia(CarriageReturnLineFeed)
          .NormalizeWhitespace();

        compilationUnits.Add((builder.BuilderName + ".cs", compilationUnit));
      }

      return compilationUnits;
    }

    private static MethodDeclarationSyntax GetPartialMethodFor(string methodName, BuilderToGenerate builder)
    {
      return MethodDeclaration(PredefinedType(Token(SyntaxKind.VoidKeyword)), Identifier(methodName))
        .WithModifiers(TokenList(Token(SyntaxKind.PartialKeyword)))
        .WithParameterList(
          ParameterList(SeparatedList<ParameterSyntax>(new SyntaxNodeOrToken[]
          {
            Parameter(Identifier("userId")).WithType(IdentifierName("ISessionToken")),
            Token(SyntaxKind.CommaToken),
            Parameter(Identifier("session")).WithType(IdentifierName("IDataSession")),
            Token(SyntaxKind.CommaToken),
            Parameter(Identifier(builder.EntityType.Name.FirstCharToLowerCase())).WithType(IdentifierName(builder.EntityType.Name)),
          }))
        ).WithSemicolonToken(Token(SyntaxKind.SemicolonToken));
    }

    private static ClassDeclarationSyntax CreateExtendingPartialMethods(BuilderToGenerate builder, ClassDeclarationSyntax builderClass)
    {
      builderClass = builderClass.AddMembers(GetPartialMethodFor("OnNewEntity", builder));
      builderClass = builderClass.AddMembers(GetPartialMethodFor("OnEntitySaved", builder));
      builderClass = builderClass.AddMembers(GetPartialMethodFor("BeforeEntityReturned", builder));

      return builderClass;
    }

    private static ClassDeclarationSyntax AddFindExistingEntityMethod(this BuilderToGenerate builder, ClassDeclarationSyntax builderClass)
    {
      var entityIdentifier = IdentifierName(builder.EntityType.Name);
      List<StatementSyntax> statements = new();

      statements.Add(LocalDeclarationStatement(
        VariableDeclaration(IdentifierName("var")).AddVariables(
          VariableDeclarator(Identifier("criteria"))
          .WithInitializer(
            EqualsValueClause(
              InvocationExpression(MemberAccessExpression(
                SyntaxKind.SimpleMemberAccessExpression, IdentifierName("Criteria"), IdentifierName("For")
              )).AddArgumentListArguments(Argument(TypeOfExpression(entityIdentifier)))
            )
          )
        )
      ));

      foreach (var propertyToFilter in builder.DeDuplicate)
      {
        statements.Add(ExpressionStatement(
          InvocationExpression(MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("criteria"),
            IdentifierName("Add")
          )).AddArgumentListArguments(Argument(
            InvocationExpression(MemberAccessExpression(
              SyntaxKind.SimpleMemberAccessExpression,
              IdentifierName("Expression"),
              IdentifierName("Eq"))
            ).WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
              new SyntaxNodeOrToken[]{
                Argument(InvocationExpression(IdentifierName("nameof"))
                .WithArgumentList(ArgumentList(SingletonSeparatedList(
                  Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, entityIdentifier, IdentifierName(propertyToFilter)))
                )))),
                Token(SyntaxKind.CommaToken),
                Argument(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("newEntity"), IdentifierName(propertyToFilter)))
              }
            )))
          ))
        ));
      }

      statements.Add(ReturnStatement(InvocationExpression(
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
          InvocationExpression(MemberAccessExpression(
            SyntaxKind.SimpleMemberAccessExpression,
            IdentifierName("session"),
            GenericName(Identifier("GetAllByCriteria")).AddTypeArgumentListArguments(entityIdentifier)
          )).WithArgumentList(ArgumentList(
            SeparatedList<ArgumentSyntax>(
              new SyntaxNodeOrToken[] {
                Argument(IdentifierName("userId")),
                Token(SyntaxKind.CommaToken),
                Argument(IdentifierName("criteria"))
              }
            )
          )), IdentifierName("FirstOrDefault")
        )))
      );

      builderClass = builderClass.AddMembers(
        MethodDeclaration(entityIdentifier, Identifier("FindExistingEntity"))
        .WithModifiers(TokenList(Token(SyntaxKind.PrivateKeyword)))
        .AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("DebuggerStepThroughAttribute")))))
        .WithParameterList(
          ParameterList(
            SeparatedList<ParameterSyntax>(
              new SyntaxNodeOrToken[]{
                Parameter(Identifier("userId")).WithType(IdentifierName("ISessionToken")),
                Token(SyntaxKind.CommaToken),
                Parameter(Identifier("session")).WithType(IdentifierName("IDataSession")),
                Token(SyntaxKind.CommaToken),
                Parameter(Identifier("newEntity")).WithType(entityIdentifier)
              }
            )
          )
        ).WithBody(Block(statements))
      );

      return builderClass;
    }

    /// <summary>
    /// Generate With methods to the builder.EntityType properties
    /// </summary>
    /// <param name="builder">The Builder Object</param>
    /// <param name="builderClass">The Class being builded. <see cref="ClassDeclarationSyntax"/></param>
    /// <returns><see cref="ClassDeclarationSyntax"/></returns>
    private static ClassDeclarationSyntax AddMutationMethods(this BuilderToGenerate builder, ClassDeclarationSyntax builderClass)
    {
      foreach (var property in builder.Properties)
      {
        var lowerCamelParameterName = property.PropertyName.Substring(0, 1).ToLowerInvariant() + property.PropertyName.Substring(1);
        var upperCamelParameterName = property.PropertyName.Substring(0, 1).ToUpperInvariant() + property.PropertyName.Substring(1);

        if (upperCamelParameterName == builder.EntityTypePrimaryKey) continue;

        builderClass = builderClass.AddMembers(
          MethodDeclaration(builder.BuilderTypeSyntax, "With" + upperCamelParameterName)
          .AddModifiers(Token(SyntaxKind.PublicKeyword))
          .AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("DebuggerStepThroughAttribute")))))
          .AddParameterListParameters(Parameter(Identifier(lowerCamelParameterName)).WithType(property.TypeSyntax))
          .AddBodyStatements(
            ExpressionStatement(
              AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(property.FieldName)),
                IdentifierName(lowerCamelParameterName)
              )
            ),
            ReturnStatement(ThisExpression())
          )
        );
      }

      return builderClass;
    }

    private static ClassDeclarationSyntax AddEntityReferenceMethods(this BuilderToGenerate builder, ClassDeclarationSyntax builderClass)
    {
      foreach (var attributeData in builder.ReferenceEntities)
      {
        // E.g: public PersonBuilder WithCurrencyId(int currencyId)
        var referenceWithPropertyMemberMethod = MethodDeclaration(builder.BuilderTypeSyntax, Identifier($"With{attributeData.PropertyName}"))
          .AddModifiers(Token(SyntaxKind.PublicKeyword))
          .AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("DebuggerStepThroughAttribute")))))
          .AddParameterListParameters(Parameter(Identifier(attributeData.FieldName)).WithType(attributeData.TypeSyntax))
          .AddBodyStatements(GetValidateEntityIsNotGenerated(builder),
            attributeData.IsNullable ? GetNullableIfWithElseStatement(attributeData) : GetExpressionStatementAssignmentExpression(attributeData),
            ReturnStatement(ThisExpression())
          );

        builderClass = builderClass.AddMembers(referenceWithPropertyMemberMethod);

        // E.g: public PersonBuilder WithCurrency(CurrencyBuilder otherCurrencyBuilder)
        builderClass = builderClass.AddMembers(GetWithMethodForBuilderEntity(builder, attributeData));
      }

      return builderClass;
    }

    /// <summary>
    /// Adds the With method for the referenced builder entity with the attribute <see cref="Attributes.EntityObjectBuilderReferenceAttribute"/><br/>
    /// E.g:
    /// <br/>public AreaOfOriginBuilder WithCompanyOutlet(CompanyOutletBuilder companyOutletBuilder)
    /// <br/>{
    /// <br/>    ValidateEntityIsNotGenerated(areaOfOriginEntity);
    /// <br/>    this.companyOutletId = (userId, session) => companyOutletBuilder.Build(userId, session).Id32;
    /// <br/>    return this;
    /// <br/>}
    /// </summary>
    /// <param name="builder"><see cref="BuilderToGenerate"/></param>
    /// <param name="propertyToGenerate"><see cref="PropertyToGenerate"/></param>
    /// <returns><see cref="MethodDeclarationSyntax"/></returns>
    private static MethodDeclarationSyntax GetWithMethodForBuilderEntity(BuilderToGenerate builder, PropertyToGenerate propertyToGenerate)
    {
      var referenceEntityName = propertyToGenerate.ReferenceEntity!.Name;
      var methodName = $"With{propertyToGenerate.PropertyName}";

      // Remove the "id" part from the method name.
      int builderIndex = methodName.ToLower().IndexOf("id");
      if (builderIndex >= 0)
      {
        methodName = methodName.Remove(builderIndex, 2);
      }

      return MethodDeclaration(builder.BuilderTypeSyntax, Identifier(methodName))
        .AddModifiers(Token(SyntaxKind.PublicKeyword))
        .AddParameterListParameters(
          Parameter(Identifier($"{referenceEntityName.FirstCharToLowerCase()}"))
          .WithType(IdentifierName($"{referenceEntityName}"))
        )
        .AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("DebuggerStepThroughAttribute")))))
        .WithBody(
          Block(
            GetValidateEntityIsNotGenerated(builder),
            ExpressionStatement(
              AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(propertyToGenerate.FieldName)),
                ParenthesizedLambdaExpression()
                .WithParameterList(
                  ParameterList(
                    SeparatedList<ParameterSyntax>(
                      new SyntaxNodeOrToken[]{
                        Parameter(Identifier("userId")),
                        Token(SyntaxKind.CommaToken),
                        Parameter(Identifier("session"))
                      }
                    )
                  )
                )
                .WithExpressionBody(
                  MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    InvocationExpression(
                      MemberAccessExpression(
                        SyntaxKind.SimpleMemberAccessExpression,
                        IdentifierName(referenceEntityName.FirstCharToLowerCase()),
                        IdentifierName("Build")
                      )
                    )
                    .WithArgumentList(
                      ArgumentList(
                        SeparatedList<ArgumentSyntax>(
                          new SyntaxNodeOrToken[]{
                            Argument(IdentifierName("userId")),
                            Token(SyntaxKind.CommaToken),
                            Argument(IdentifierName("session"))
                          }
                        )
                      )
                    ),
                    IdentifierName("Id32")
                  )
                )
              )
            ),
            ReturnStatement(ThisExpression())
          )
        );
    }

    /// <summary>
    /// Gets "if/else" statement for the <paramref name="propertyToGenerate"/><br/>
    /// E.g:<br/>
    /// if (companyOutletId.HasValue)<br/>
    /// {<br/>
    ///     this.companyOutletId = (userId, session) => companyOutletId.Value;<br/>
    /// }<br/>
    /// else<br/>
    /// {<br/>
    ///     this.companyOutletId = null;<br/>
    /// }<br/>
    /// </summary>
    /// <param name="propertyToGenerate">The Nullable <see cref="PropertyToGenerate"/></param>
    /// <returns><see cref="IfStatementSyntax"/></returns>
    private static IfStatementSyntax GetNullableIfWithElseStatement(PropertyToGenerate propertyToGenerate)
    {
      return IfStatement(
        MemberAccessExpression(
          SyntaxKind.SimpleMemberAccessExpression,
          IdentifierName(propertyToGenerate.FieldName),
          IdentifierName("HasValue")
        ),
        Block(SingletonList<StatementSyntax>(GetExpressionStatementAssignmentExpression(propertyToGenerate)))
      )
      .WithElse(
        ElseClause(
          Block(
            SingletonList<StatementSyntax>(
              ExpressionStatement(
                AssignmentExpression(
                  SyntaxKind.SimpleAssignmentExpression,
                  MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    ThisExpression(),
                    IdentifierName(propertyToGenerate.FieldName)
                  ), LiteralExpression(SyntaxKind.NullLiteralExpression)
                )
              )
            )
          )
        )
      );
    }

    /// <summary>
    /// Gets the assignment expression to the property.<br/>
    /// E.g: <br/>this.companyOutletId = (userId, session) => companyOutletId.Value;
    /// </summary>
    /// <param name="propertyToGenerate"><see cref="PropertyToGenerate"/></param>
    /// <returns><see cref="ExpressionStatementSyntax"/></returns>
    private static ExpressionStatementSyntax GetExpressionStatementAssignmentExpression(PropertyToGenerate propertyToGenerate)
    {
      return ExpressionStatement(
        AssignmentExpression(
          SyntaxKind.SimpleAssignmentExpression,
          MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(propertyToGenerate.FieldName)),
          ParenthesizedLambdaExpression()
          .WithParameterList(
            ParameterList(
            SeparatedList<ParameterSyntax>(
                new SyntaxNodeOrToken[]{
                  Parameter(Identifier("userId")),
                  Token(SyntaxKind.CommaToken),
                  Parameter(Identifier("session"))
                }
              )
            )
          )
          .WithExpressionBody(PropertyAccessUnwrappingNullables(propertyToGenerate))
        )
      );
    }

    /// <summary>
    /// Adds the ValidateEntityIsNotGenerated method.<br/>
    /// E.g:<br/>ValidateEntityIsNotGenerated(areaOfOriginEntity);
    /// </summary>
    /// <param name="builder">The <see cref="BuilderToGenerate"/> object.</param>
    /// <returns><see cref="ExpressionStatementSyntax"/></returns>
    private static ExpressionStatementSyntax GetValidateEntityIsNotGenerated(BuilderToGenerate builder)
    {
      return ExpressionStatement(
        InvocationExpression(IdentifierName("ValidateEntityIsNotGenerated"))
        .AddArgumentListArguments(Argument(IdentifierName(builder.EntityType.Name.FirstCharToLowerCase())))
      );
    }

    /// <summary>
    /// Adds the call to the partial methods: OnNewEntity, OnEntitySaved and BeforeEntityReturned
    /// </summary>
    /// <param name="methodName">The method name. E.g: OnNewEntity</param>
    /// <param name="entityNameIdentifier">The IdentifierName <see cref="IdentifierNameSyntax"/></param>
    /// <returns><see cref="ExpressionStatementSyntax"/></returns>
    private static ExpressionStatementSyntax GetExpressionStatementFor(string methodName, IdentifierNameSyntax entityNameIdentifier)
    {
      return ExpressionStatement(
        InvocationExpression(IdentifierName(methodName)).WithArgumentList(
          ArgumentList(SeparatedList<ArgumentSyntax>(new SyntaxNodeOrToken[]
          {
            Argument(IdentifierName("userId")),
            Token(SyntaxKind.CommaToken),
            Argument(IdentifierName("session")),
            Token(SyntaxKind.CommaToken),
            Argument(entityNameIdentifier)
          }))
        )
      );
    }

    /// <summary>
    /// Try to add the Build method for the current builder generator
    /// </summary>
    /// <param name="builder">The BuilderToGenerate object.</param>
    /// <param name="builderClass"><see cref="ClassDeclarationSyntax"/></param>
    /// <returns>true if everything goes well, otherwise false.</returns>
    private static bool TryAddBuildMethod(BuilderToGenerate builder, ref ClassDeclarationSyntax builderClass)
    {
      var entityIdentifierName = IdentifierName(builder.EntityType.Name.FirstCharToLowerCase());

      var buildMethodStatements = new List<StatementSyntax>
      {
        IfStatement(
          BinaryExpression(SyntaxKind.NotEqualsExpression, entityIdentifierName, LiteralExpression(SyntaxKind.NullLiteralExpression)),
          ReturnStatement(entityIdentifierName)
        )
      };

      var buildingInstanceIdentifier = Identifier("entityTemplate");

      builder.DefaultValues.Insert(0, "userId");
      ArgumentSyntax[] defaultValues = builder.DefaultValues.Select(defValue => Argument(IdentifierName(defValue))).ToArray();

      foreach (var defaultValue in defaultValues.Skip(1))
      {
        PropertyToGenerate? property = null;
        TypeSyntax typeSyntax;

        var referenceEntity = builder.ReferenceEntities.FirstOrDefault(property => property.FieldName == defaultValue.ToString());
        if (referenceEntity != null)
        {
          builder.ReferenceEntities.RemoveAll(property => property.FieldName == defaultValue.ToString());
          typeSyntax = referenceEntity.TypeSyntax;
        }
        else
        {
          property = builder.Properties.FirstOrDefault(property => property.FieldName == defaultValue.ToString());
          builder.Properties.RemoveAll(property => property.FieldName == defaultValue.ToString());
          typeSyntax = property.TypeSyntax;
        }

        buildMethodStatements.Add(
          LocalDeclarationStatement(
            VariableDeclaration(typeSyntax)
            .WithVariables(
              SingletonSeparatedList<VariableDeclaratorSyntax>(
                VariableDeclarator(Identifier(defaultValue.ToString()))
                .WithInitializer(
                  EqualsValueClause(
                    LiteralExpression(
                      SyntaxKind.NullLiteralExpression))))))
        );

        if (referenceEntity != null)
        {
          buildMethodStatements.Add(AddIfReferenceEntitieAssignment(referenceEntity, IdentifierName(defaultValue.ToString())));
        }
        else
        {
          buildMethodStatements.Add(AddIfPropertyAssignment(property!, IdentifierName(defaultValue.ToString())));
        }
      }

      buildMethodStatements.Add(
        LocalDeclarationStatement(
          VariableDeclaration(IdentifierName(Identifier("var"))).AddVariables(
            VariableDeclarator(buildingInstanceIdentifier).WithInitializer(
              EqualsValueClause(
                InvocationExpression(
                  MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    IdentifierName("session"),
                    GenericName(Identifier("GetNew")).AddTypeArgumentListArguments(IdentifierName(builder.EntityType.Name))
                  )
                ).AddArgumentListArguments(defaultValues)
              )
            )
          )
        )
      );

      buildMethodStatements.Add(GetExpressionStatementFor("OnNewEntity", IdentifierName("entityTemplate")));

      /* Add nullable properties */
      buildMethodStatements.AddRange(builder.Properties
        .Where(property => property.IsNullable)
        .Select(property =>
          IfStatement(
            BinaryExpression(
              SyntaxKind.LogicalAndExpression,
              AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.NullLiteralExpression),
              AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword))
            ),
            ExpressionStatement(
              AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(buildingInstanceIdentifier), IdentifierName(property.PropertyName)),
                ThisPropertyAccessUnwrappingNullables(property)
      )))));

      /* Add not nullable properties */
      buildMethodStatements.AddRange(builder.Properties
        .Where(property => !property.IsNullable)
        .Select(property =>
          IfStatement(
            AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)),
            ExpressionStatement(
              AssignmentExpression(
                SyntaxKind.SimpleAssignmentExpression,
                MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(buildingInstanceIdentifier), IdentifierName(property.PropertyName)),
                ThisPropertyAccessUnwrappingNullables(property)
      )))));

      /* Add nullable builder references */
      buildMethodStatements.AddRange(builder.ReferenceEntities
        .Where(property => property.IsNullable)
        .Select(property =>
          IfStatement(
            BinaryExpression(
              SyntaxKind.LogicalAndExpression,
              AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.NullLiteralExpression),
              AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword))
            ),
            Block(AddBuilderCallExpressionStatement(buildingInstanceIdentifier, property.PropertyName, property.FieldName))
      )));

      /* Add not nullable builder references */
      buildMethodStatements.AddRange(builder.ReferenceEntities
        .Where(property => !property.IsNullable)
        .Select(property =>
          IfStatement(
            AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword)),
            Block(AddBuilderCallExpressionStatement(buildingInstanceIdentifier, property.PropertyName, property.FieldName))
      )));

      if (builder.DeDuplicate.Any())
      {
        buildMethodStatements.Add(ExpressionStatement(AssignmentExpression(
          SyntaxKind.SimpleAssignmentExpression,
          entityIdentifierName,
          InvocationExpression(IdentifierName("FindExistingEntity"))
          .WithArgumentList(ArgumentList(SeparatedList<ArgumentSyntax>(
            new SyntaxNodeOrToken[]{
              Argument(IdentifierName("userId")),
              Token(SyntaxKind.CommaToken),
              Argument(IdentifierName("session")),
              Token(SyntaxKind.CommaToken),
              Argument(IdentifierName("entityTemplate"))
            }
          )))
        )));
      }

      buildMethodStatements.Add(IfStatement(
        BinaryExpression(SyntaxKind.EqualsExpression, entityIdentifierName, LiteralExpression(SyntaxKind.NullLiteralExpression)),
        Block(
          ExpressionStatement(AssignmentExpression(SyntaxKind.SimpleAssignmentExpression, entityIdentifierName, IdentifierName("entityTemplate"))),
          ExpressionStatement(AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, entityIdentifierName, IdentifierName(builder.EntityTypePrimaryKey)),
            InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName("session"), IdentifierName("Save"))).WithArgumentList(
              ArgumentList(SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]{
                  Argument(IdentifierName("userId")),
                  Token(SyntaxKind.CommaToken),
                  Argument(entityIdentifierName)
                }
              ))
            )
          )),
          GetExpressionStatementFor("OnEntitySaved", entityIdentifierName)
        )
      ));

      buildMethodStatements.Add(GetExpressionStatementFor("BeforeEntityReturned", entityIdentifierName));

      buildMethodStatements.Add(ReturnStatement(entityIdentifierName));

      builderClass = builderClass.AddMembers(
        MethodDeclaration(ParseTypeName("EntityObject"), "Build")
          .WithModifiers(
            TokenList(new[] { Token(SyntaxKind.PublicKeyword), Token(SyntaxKind.OverrideKeyword) })
          )
          .AddAttributeLists(AttributeList(SingletonSeparatedList(Attribute(IdentifierName("DebuggerStepThroughAttribute")))))
          .WithParameterList(
            ParameterList(
              SeparatedList<ParameterSyntax>(
                new SyntaxNodeOrToken[]{
                  Parameter(Identifier("userId")).WithType(IdentifierName("ISessionToken")),
                  Token(SyntaxKind.CommaToken),
                  Parameter(Identifier("session")).WithType(IdentifierName("IDataSession"))
                })))
          .WithBody(Block(buildMethodStatements))
      );

      return true;
    }

    private static ExpressionSyntax PropertyAccessUnwrappingNullables(PropertyToGenerate property)
    {
      if (!property.IsNullable || property.IsReferenceType)
      {
        return IdentifierName(property.FieldName);
      }
      else
      {
        return MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(property.FieldName), IdentifierName("Value"));
      }
    }

    private static ExpressionSyntax ThisPropertyAccessUnwrappingNullables(PropertyToGenerate property)
    {
      if (!property.IsNullable || property.IsReferenceType)
      {
        return MemberAccessExpression(
          SyntaxKind.SimpleMemberAccessExpression,
          ThisExpression(),
          IdentifierName(property.FieldName)
        );
      }
      else
      {
        return MemberAccessExpression(
          SyntaxKind.SimpleMemberAccessExpression, 
          ThisExpression(), 
          IdentifierName(property.FieldName + ".Value")
        );
      }
    }

    private static ExpressionStatementSyntax AddBuilderCallExpressionStatement(SyntaxToken identifierName, string propertyName, string fieldName)
    {
      return ExpressionStatement(
        AssignmentExpression(SyntaxKind.SimpleAssignmentExpression,
          MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, IdentifierName(identifierName), IdentifierName(propertyName)),
          InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(fieldName)))
          .WithArgumentList(
            ArgumentList(
              SeparatedList<ArgumentSyntax>(
                new SyntaxNodeOrToken[]{
                  Argument(IdentifierName("userId")),
                  Token(SyntaxKind.CommaToken),
                  Argument(IdentifierName("session"))
                }
      )))));
    }

    private static IfStatementSyntax AddIfPropertyAssignment(PropertyToGenerate property, ExpressionSyntax varName)
    {
      return IfStatement(
        BinaryExpression(
          SyntaxKind.LogicalAndExpression,
          AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.NullLiteralExpression),
          AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword))
        ),
        ExpressionStatement(
          AssignmentExpression(
            SyntaxKind.SimpleAssignmentExpression,
            varName,
            ThisPropertyAccessUnwrappingNullables(property)
          )
        )
      );
    }

    private static IfStatementSyntax AddIfReferenceEntitieAssignment(PropertyToGenerate property, ExpressionSyntax varName)
    {
      return IfStatement(
        BinaryExpression(
          SyntaxKind.LogicalAndExpression,
          AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.NullLiteralExpression),
          AddNotEqualsBinaryExpression(property.FieldName, SyntaxKind.DefaultLiteralExpression, Token(SyntaxKind.DefaultKeyword))
        ),
        Block(
          ExpressionStatement(
            AssignmentExpression(
              SyntaxKind.SimpleAssignmentExpression,
              varName,
              InvocationExpression(MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(property.FieldName)))
              .WithArgumentList(
                ArgumentList(
                  SeparatedList<ArgumentSyntax>(
                    new SyntaxNodeOrToken[]{
                      Argument(IdentifierName("userId")),
                      Token(SyntaxKind.CommaToken),
                      Argument(IdentifierName("session"))
                    })
      ))))));
    }

    private static BinaryExpressionSyntax AddNotEqualsBinaryExpression(string fieldName, SyntaxKind kind)
    {
      return BinaryExpression(
        SyntaxKind.NotEqualsExpression,
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(fieldName)),
        LiteralExpression(kind)
      );
    }

    private static BinaryExpressionSyntax AddNotEqualsBinaryExpression(string fieldName, SyntaxKind kind, SyntaxToken token)
    {
      return BinaryExpression(
        SyntaxKind.NotEqualsExpression,
        MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression, ThisExpression(), IdentifierName(fieldName)),
        LiteralExpression(kind, token)
      );
    }
  }
}
