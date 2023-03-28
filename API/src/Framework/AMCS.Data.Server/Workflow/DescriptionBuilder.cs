namespace AMCS.Data.Server.Workflow
{
  using System;
  using System.Linq;
  using System.Collections.Generic;
  using AMCS.PluginData.Data.MetadataRegistry.Shared;
  using AMCS.Data.Configuration.Resource;

  public class DescriptionBuilder : Builder<Description>
  {
    public DescriptionBuilder WithText(string text)
    {
      var translations = DataServices.Resolve<ILocalisedStringResourceCache>().GetStrings(text);

      foreach (var (language, value) in translations)
      {
        Entity.Value.Add(new LocalisationValue
        {
          Language = language,
          Text = value
        });
      }

      return this;
    }
  }
}
