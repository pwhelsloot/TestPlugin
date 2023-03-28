//-----------------------------------------------------------------------------
// <copyright file="MappingManagerAccessor.cs" company="AMCS Group">
//   Copyright © 2010-12 AMCS Group. All rights reserved.
// </copyright>
//
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Server
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using AMCS.Data.Configuration;
    using AMCS.Data.Configuration.Mapping;
    using AMCS.Data.Configuration.Mapping.IO.Xml;
    using AMCS.Data.Configuration.Mapping.Manager;
    using AMCS.Data.Entity;
    using AMCS.Data.Entity.Validation;
    using AMCS.Data.Server.Settings;

    /// <summary>
    /// Mapping Manager Accessor.
    /// </summary>
    public class MappingManagerAccessor : IMappingManagerAccessor, IDelayedStartup
    {
    private readonly string projectId;
    private readonly ILanguageResources languageResources;
    private readonly TypeManager entityTypes;

    #region Constructors

    /// <summary>
    /// Prevents a default instance of the <see cref="MappingManagerAccessor"/> class from being created.
    /// </summary>
    internal MappingManagerAccessor(string projectId, ILanguageResources languageResources, TypeManager entityTypes)
    {
      this.projectId = projectId;
      this.languageResources = languageResources;
      this.entityTypes = entityTypes;
    }

    void IDelayedStartup.Start()
    {
      using (var session = BslDataSessionFactory.GetDataSession())
      using (var transaction = session.CreateTransaction())
      {
        var builder = ((EntityObjectValidator)DataServices.Resolve<IEntityObjectValidator>()).CreateBuilder();

        var databaseOverrides = DataServices.Resolve<ITranslationMappingOverrideService>().GetAllByProjectIdentifier(projectId, session);

        EntityObjectMappingManager = new MappingManager(projectId, new XmlEntityObjectMappingImporter(languageResources.Assembly, languageResources.EntityResourcesNamespace, entityTypes, builder), databaseOverrides);
        EntityObjectMappingManager.ImportAll();

        SearchResultMappingManager = new MappingManager(projectId, new XmlSearchResultMappingImporter(languageResources.Assembly, languageResources.SearchResourcesNamespace), databaseOverrides);
        SearchResultMappingManager.ImportAll();

        OverwriteEntityObjectValidationMappings(EntityObjectMappingManager, entityTypes, builder, session);

        builder.Build();

        transaction.Commit();
      }
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// Gets the business object string mapping manager.
    /// </summary>
    /// <value>
    /// The business object string mapping manager.
    /// </value>
    public IMappingManager BusinessObjectStringMappingManager { get; }

    /// <summary>
    /// Gets the entity object mapping manager.
    /// </summary>
    /// <value>
    /// The entity object mapping manager.
    /// </value>
    public IMappingManager EntityObjectMappingManager { get; private set; }

    /// <summary>
    /// Gets the search result mapping manager.
    /// </summary>
    /// <value>
    /// The search result mapping manager.
    /// </value>
    public IMappingManager SearchResultMappingManager { get; private set; }

    #endregion Properties

    #region Methods

    /// <summary>
    /// Overwrites the entity object validation mappings.
    /// </summary>
    /// <param name="entityObjectMappingManager">The entity object mapping manager.</param>
    /// <param name="entityTypes"></param>
    /// <param name="builder"></param>
    /// <param name="session"></param>
    private void OverwriteEntityObjectValidationMappings(IMappingManager entityObjectMappingManager, TypeManager entityTypes, IEntityObjectValidatorBuilder builder, IDataSession session)
    {
      IEntityPropertyValidationService entityPropertyValidationService = DataServices.Resolve<IEntityPropertyValidationService>();

      // Overwrite entityObjectMappings with validation rules from EntityPropertyValidationService
      IList<EntityPropertyValidationEntity> allEntityValidationRules = entityPropertyValidationService.GetAllByClassNameAndPropertyName(existingDataSession: session);

      var entityObjectXmlMappings = entityObjectMappingManager.GetMappings();
      foreach (EntityPropertyValidationEntity entityValidationRule in allEntityValidationRules)
      {
        if (entityValidationRule.ValidationContextId == null)
        {
          // Create mapping based entityValidationRule
          EntityObjectMapping ruleMapping = new EntityObjectMapping();
          ruleMapping.Id = entityValidationRule.ClassName;
          ruleMapping.Properties = new List<IEntityObjectProperty>()
          {
            new EntityObjectMappingProperty()
            {
              PropertyName = entityValidationRule.PropertyName,
              FailValidationTest = entityValidationRule.FailValidationTest,
              String = new EntityObjectTranslatableLocaleString()
              {
                ErrorText = entityValidationRule.ErrorText,
                Value = "N/A"
              }
            }
          };

          // Update mapping
          if (entityObjectXmlMappings.ContainsKey(entityValidationRule.ClassName))
          {
            entityObjectXmlMappings[entityValidationRule.ClassName].MergeWith(ruleMapping);
          }
          else
          {
            // TODO: I don't like this - these incomplete rules may be risky
            entityObjectXmlMappings.Add(entityValidationRule.ClassName, ruleMapping);
          }
        }

        // Update EntityObjectValidator
        Type entityType = entityTypes.GetType(entityValidationRule.ClassName);
        if (entityValidationRule.ValidationContextId != null)
        {
          builder.AddPropertyFailValidationTest(entityType, entityValidationRule, true);
        }
        else
        {
          builder.AddPropertyFailValidationTest(entityType, entityValidationRule.PropertyName, entityValidationRule.FailValidationTest, entityValidationRule.ErrorText, true);
        }
      }
    }

    #endregion Methods
  }
}