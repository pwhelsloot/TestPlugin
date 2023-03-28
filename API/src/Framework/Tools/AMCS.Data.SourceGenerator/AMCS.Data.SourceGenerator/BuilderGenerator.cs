using AMCS.Data.SourceGenerator.ExtensionMethods;
using Microsoft.CodeAnalysis;
using System;
using System.Text;

namespace AMCS.Data.SourceGenerator
{
  [Generator]
  public class BuilderGenerator : ISourceGenerator
  {
    public void Initialize(GeneratorInitializationContext context) { }

    public void Execute(GeneratorExecutionContext context)
    {
      try
      {
        var builders = context.DetermineBuildersToGenerate();
        var compilationUnits = builders.GenerateCompilationUnits(context);

        foreach (var (fileName, compilationUnit) in compilationUnits)
        {
          context.AddSource(fileName, compilationUnit.GetText(Encoding.UTF8));
        }
      }
      catch (Exception ex)
      {
        // Report a diagnostic if an exception occurs while generating code; allows consumers to know what is going on
        string message = $"Exception: {ex.Message} - {ex.StackTrace}";
        context.ReportDiagnostic(Diagnostic.Create(
          new DiagnosticDescriptor
          (
            "DBG000",
            message,
            message,
            "BuilderGenerator",
            DiagnosticSeverity.Error,
            isEnabledByDefault: true
          ), Location.None));
      }
    }
  }
}
