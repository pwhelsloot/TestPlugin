namespace AMCS.Data.Server.Workflow
{
  using System;
  using System.Collections.Generic;
  using AMCS.PluginData.Data.MetadataRegistry.Shared;
  using AMCS.PluginData.Data.MetadataRegistry.Workflows;

  public class AsyncRestActivityBuilder : Builder<AsyncRestWorkflowActivity>
  {
    public AsyncRestActivityBuilder WithName(string value)
    {
      Entity.Name = value;
      return this;
    }

    public AsyncRestActivityBuilder AddDescription(Action<DescriptionBuilder> action)
    {
      var childBuilder = new DescriptionBuilder();
      Entity.Description = childBuilder.Build();
      action?.Invoke(childBuilder);
      return this;
    }

    public AsyncRestActivityBuilder WithDocumentation(string value)
    {
      Entity.Documentation = value;
      return this;
    }

    public AsyncRestActivityBuilder AddDescription(string value)
    {
      Entity.Description = new DescriptionBuilder()
        .WithText(value)
        .Build();
      
      return this;
    }

    public AsyncRestActivityBuilder AddEndpoint(Action<EndpointBuilder> action)
    {
      var childBuilder = new EndpointBuilder();
      Entity.Endpoint = childBuilder.Build();
      action?.Invoke(childBuilder);
      return this;
    }

    public AsyncRestActivityBuilder AddInput(Action<InputOutputBuilder<Input>> builder)
    {
      if (Entity.Inputs == null)
        Entity.Inputs = new List<Input>();

      var childBuilder = new InputOutputBuilder<Input>();
      Entity.Inputs.Add(childBuilder.Build());
      builder?.Invoke(childBuilder);
      return this;
    }

    public AsyncRestActivityBuilder AddOutput(Action<InputOutputBuilder<Output>> builder)
    {
      if (Entity.Outputs == null)
        Entity.Outputs = new List<Output>();

      var childBuilder = new InputOutputBuilder<Output>();
      Entity.Outputs.Add(childBuilder.Build());
      builder?.Invoke(childBuilder);
      return this;
    }
  }
}
