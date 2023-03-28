namespace AMCS.Data.Server.Workflow
{
  using System;
  using System.Collections.Generic;
  using Entity;
  using AMCS.PluginData.Data.MetadataRegistry.Shared;
  
  public class InputOutputBuilder<T> : Builder<T> where T : InputOutputBase
  {
    public InputOutputBuilder<T> WithName(string value)
    {
      Entity.Name = value;
      return this;
    }

    public InputOutputBuilder<T> WithType(DataType value)
    {
      Entity.Type = value.DataTypeValue;
      return this;
    }
    
    public InputOutputBuilder<T> IsRequired()
    {
      Entity.Required = true;
      return this;
    }

    public InputOutputBuilder<T> AddDescription(Action<DescriptionBuilder> action)
    {
      var childBuilder = new DescriptionBuilder();
      Entity.Description = childBuilder.Build();
      action?.Invoke(childBuilder);
      return this;
    }

    public InputOutputBuilder<T> AddDescription(string value)
    {
      Entity.Description = new DescriptionBuilder()
        .WithText(value)
        .Build();
      
      return this;
    }
  }
}
