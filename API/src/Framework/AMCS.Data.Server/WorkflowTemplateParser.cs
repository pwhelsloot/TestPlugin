namespace AMCS.Data.Server
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Text.RegularExpressions;
  using AMCS.Data.Entity;

  public class WorkflowTemplateParser
  {
    private readonly Dictionary<string, object> parameters =
      new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

    private const string DefaultValuePipe = "default";
    private const string QuotePipe = "quote";

    private readonly string text;

    private WorkflowTemplateParser(string text)
    {
      this.text = text;
    }

    public static WorkflowTemplateParser Create(string json)
    {
      return new WorkflowTemplateParser(json);
    }

    public string Build()
    {
      const string templatePattern = @"[\""\']?\{\{.*?\}\}[\""\']?";

      var parsedTemplate = Regex.Replace(text, templatePattern, match =>
      {
        var value = Regex.Replace(match.Value, @"[\""\{\}\']", "");
        var templates = value.Split('|');

        // First "pipe" will always be variable replacement
        var variablePipe = templates.FirstOrDefault();
        if (string.IsNullOrWhiteSpace(variablePipe))
          throw new InvalidOperationException("Invalid template variable");

        // If no input variable found, skip processing
        if(!parameters.TryGetValue(variablePipe, out var result))
          return match.Value;
        
        foreach (var additionalPipe in templates.Skip(1))
        {
          if (!additionalPipe.StartsWith(DefaultValuePipe) && additionalPipe != QuotePipe)
            throw new InvalidOperationException($"Unknown pipe {additionalPipe} supplied to template");

          if (additionalPipe.StartsWith(DefaultValuePipe) && result == null)
          {
            var split = additionalPipe.Split(new[] { ':' }, 2);

            if (split.Length != 2 || string.IsNullOrWhiteSpace(split.Last()))
              throw new InvalidOperationException("Expecting parameter with default pipe");

            result = split.Last();
          }
          else if (additionalPipe == QuotePipe)
          {
            result = $"\"{result}\"";
          }
        }

        return result?.ToString() ?? "null";
      });

      return parsedTemplate;
    }

    public WorkflowTemplateParser AddParameter(string variableName, object value)
    {
      parameters.Add(variableName, value);
      return this;
    }
  }
}