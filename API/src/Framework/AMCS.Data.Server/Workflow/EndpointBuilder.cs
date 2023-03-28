namespace AMCS.Data.Server.Workflow
{
  using System.Collections.Generic;
  using System.Net.Http;
  using AMCS.PluginData.Data.MetadataRegistry.Workflows;
  using PluginData.Data.MetadataRegistry.Shared;
  using Entity;

  public class EndpointBuilder : Builder<Endpoint>
  {
    public Output ResponseOutput { get; private set; }
    
    public EndpointBuilder WithHttpMethod(HttpMethod value)
    {
      Entity.HttpMethod = value.Method;
      return this;
    }

    public EndpointBuilder WithUrl(string value)
    {
      Entity.Url = value;
      return this;
    }

    public EndpointBuilder UrlEncodedRequest()
    {
      Entity.RequestMimeType = "application/x-www-form-urlencoded";
      return this;
    }

    public EndpointBuilder JsonRequest()
    {
      Entity.RequestMimeType = "application/json";
      return this;
    }

    public EndpointBuilder JsonRequest(string value)
    {
      Entity.RequestTemplate = value;
      return JsonRequest();
    }

    public EndpointBuilder WithOutputParameter(string value)
    {
      Entity.OutputName = value;

      ResponseOutput = new Output
      {
        Name = value,
        Required = true,
        Type = DataType.String.DataTypeValue,
        Description = new Description
        {
          Value = new List<LocalisationValue>
          {
            new LocalisationValue
            {
              Language = "en",
              Text = "API Endpoint Result will be stored wholly in this parameter"
            }
          }
        }
      };
      
      return this;
    }
  }
}
