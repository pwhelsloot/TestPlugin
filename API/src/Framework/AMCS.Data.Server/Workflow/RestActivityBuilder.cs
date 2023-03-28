namespace AMCS.Data.Server.Workflow
{
  using System;
  using System.Collections.Generic;
  using AMCS.PluginData.Data.MetadataRegistry.Shared;
  using AMCS.PluginData.Data.MetadataRegistry.Workflows;

  public class RestActivityBuilder : Builder<RestWorkflowActivity>
  {
    public RestActivityBuilder WithName(string value)
    {
      Entity.Name = value;
      return this;
    }

    public RestActivityBuilder WithDocumentation(string value)
    {
      Entity.Documentation = value;
      return this;
    }

    public RestActivityBuilder AddDescription(Action<DescriptionBuilder> action)
    {
      var childBuilder = new DescriptionBuilder();
      Entity.Description = childBuilder.Build();
      action?.Invoke(childBuilder);
      return this;
    }

    public RestActivityBuilder AddDescription(string value)
    {
      Entity.Description = new DescriptionBuilder()
        .WithText(value)
        .Build();
      
      return this;
    }

    public RestActivityBuilder AddEndpoint(Action<EndpointBuilder> action)
    {
      var childBuilder = new EndpointBuilder();
      Entity.Endpoint = childBuilder.Build();
      action?.Invoke(childBuilder);

      if (childBuilder.ResponseOutput != null)
      {
        if (Entity.Outputs == null)
          Entity.Outputs = new List<Output>();
        
        Entity.Outputs.Add(childBuilder.ResponseOutput);
      }
      
      return this;
    }

    public RestActivityBuilder AddInput(Action<InputOutputBuilder<Input>> builder)
    {
      if (Entity.Inputs == null)
        Entity.Inputs = new List<Input>();

      var childBuilder = new InputOutputBuilder<Input>();
      Entity.Inputs.Add(childBuilder.Build());
      builder?.Invoke(childBuilder);
      return this;
    }

    public RestActivityBuilder AddOutput(Action<InputOutputBuilder<Output>> builder)
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
