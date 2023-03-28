namespace AMCS.Data.Server.SystemConfiguration
{
  using System;
  using System.Collections.Generic;
  using System.Xml.Serialization;

  public class ConfigurationElement
  {
    [XmlAttribute]
    public string ImportError { get; set; }

    public void Translate(Dictionary<string, string> translations)
    {
      // Iterate all attributes of this object
      foreach (var property in this.GetType().GetProperties())
      {
        // Check to see if it has the Translatable attribute
        var attribute = (TranslatableAttribute)Attribute.GetCustomAttribute(property, typeof(TranslatableAttribute));

        // If not or it has but it's set to false, skip
        if (attribute == null || !attribute.Translatable)
          continue;

        string className = this.GetType().Name;
        string propertyName = property.Name;
        var value = property.GetValue(this, null);
        string key = $"{className}:{propertyName}:False:{value}";

        if (translations.TryGetValue(key, out string translatedValue) && translatedValue != (string)value)
          property.SetValue(this, translatedValue);
      }
    }
  }
}
