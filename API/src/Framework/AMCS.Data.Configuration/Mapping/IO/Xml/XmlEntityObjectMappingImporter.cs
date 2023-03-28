//-----------------------------------------------------------------------------
// <copyright file="XmlEntityObjectMappingImporter.cs" company="AMCS Group">
//   Copyright © 2010-11 AMCS Group. All rights reserved.
// </copyright>
// 
// PROJECT: P142 - Elemos
//
// AMCS Elemos Project
//
//-----------------------------------------------------------------------------

namespace AMCS.Data.Configuration.Mapping.IO.Xml
{
  using System;
  using System.Collections.Generic;
  using System.Globalization;
  using System.Linq;
  using System.Reflection;
  using System.Text;
  using System.Threading.Tasks;
  using System.Xml.Linq;
  using AMCS.Data.Configuration.Mapping.Translate;
  using AMCS.Data.Configuration.Resource;
  using AMCS.Data.Entity.Validation;

  public class XmlEntityObjectMappingImporter : XmlMappingImporter
  {
    private readonly TypeManager entityTypes;
    private readonly IEntityObjectValidatorBuilder builder;
    private readonly ILocalisedStringResourceCache stringCache;

    public XmlEntityObjectMappingImporter(Assembly entityXmlAssembly, string entityXmlNamespace, TypeManager entityTypes, IEntityObjectValidatorBuilder builder, bool ignoreTranslations = false)
      : base(entityXmlAssembly, entityXmlNamespace)
    {
      this.entityTypes = entityTypes;
      this.builder = builder;
      if (!ignoreTranslations)
      {
        this.stringCache = DataServices.Resolve<ILocalisedStringResourceCache>();
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="filename"></param>
    /// <returns></returns>
    public override IMapping Import(string filename)
    {
      var stream = this.XmlAssembly.GetManifestResourceStream(filename);
      if (stream == null)
      {
        throw new InvalidOperationException($"The mapping file '{filename}' could not be loaded.");
      }
      
      return this.ImportXML(XElement.Load(stream));
    }

    /// <summary>
    /// We've moved to RESX files for the translations however there has been so much stuff added around these mapping files over the years
    /// that it's very hard to get rid of them.  For this reason we're sticking with them for now but rather than holding translations in the 
    /// actual XML files they're being pulled from RESX and stuck into the "mapping" objects, so higher levels aren't aware of any difference.
    /// </summary>
    /// <param name="entityTypeName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private IList<ILocaleString> GetTranslations(string entityTypeName, string defaultValue)
    {
      IList<ILocaleString> result = null;

      string valueTran = null;
      if (!string.IsNullOrWhiteSpace(defaultValue))
      {
        string valuePrimaryKey = string.Format("{0}#{1}:{2}", ResourceStringOwnerType.Entity, entityTypeName, defaultValue);
        valueTran = this.stringCache.GetString(valuePrimaryKey, defaultValue);
      }

      if (!string.IsNullOrWhiteSpace(valueTran))
      {
        EntityObjectTranslation entStringTranslation = new EntityObjectTranslation()
        {
          Locale = CultureInfo.CurrentUICulture.Name,
          Value = valueTran
        };

        // we don't need to pull in strings from all locales because we're only interested in the current one for the life of the process
        result = new List<ILocaleString>() { entStringTranslation };
      }

      return result;
    }

    public override IMapping ImportXML(XElement root)
    {
      EntityObjectMapping mapping = new EntityObjectMapping();
      mapping.Id = (string)root.Attribute("id");
      mapping.Table = (string)root.Attribute("table");

      Guid fileId = Guid.Empty;
      if (!Guid.TryParse((string)root.Attribute("fileId"), out fileId))
      {
        fileId = Guid.NewGuid();
      }

      mapping.FileId = fileId;

      mapping.AuditTable = (string)root.Attribute("auditTable");
      mapping.AuditQuery = (string)root.Attribute("auditQuery");
      mapping.AuditKeyField = (string)root.Attribute("auditKeyField");

      var properties = root.Element(XmlNs + "properties");
      if (properties == null)
      {
        mapping.Properties = new List<IEntityObjectProperty>();
      }
      else
      {
        mapping.Properties =
            (from propertyElement in root.Element(XmlNs + "properties").Descendants(XmlNs + "property")
             select new EntityObjectMappingProperty
             {
               PropertyName = (string)propertyElement.Attribute("name"),
               ColumnName = (string)propertyElement.Attribute("column"),
               IsKey = Convert.ToBoolean((string)propertyElement.Attribute("isKey")),
               IsDynamic = Convert.ToBoolean((string)propertyElement.Attribute("IsDynamic")),
               StringFormat = (string)propertyElement.Attribute("stringFormat"),
               Width = Convert.ToInt32((string)propertyElement.Attribute("width")),
               FailValidationTest = (string)propertyElement.Attribute("failValidationTest"),
               String =
                (from stringElement in propertyElement.Descendants(XmlNs + "string")
                 select new EntityObjectTranslatableLocaleString
                 {
                   Locale = (string)stringElement.Attribute("locale"),
                   Value = (string)stringElement.Attribute("value"),
                   ErrorText = (string)stringElement.Attribute("errorText")
                 }).SingleOrDefault()
             }).ToList<IEntityObjectProperty>();
      }

      if (this.stringCache != null && mapping.Properties != null)
      {
        foreach (IEntityObjectProperty property in mapping.Properties.Where(p => p.String is EntityObjectTranslatableLocaleString))
        {
          EntityObjectTranslatableLocaleString propString = property.String as EntityObjectTranslatableLocaleString;
          propString.Translations = this.GetTranslations(mapping.Id, propString.Value);
        }
      }

      // Setup validation for the EntityObject that's been mapped.
      // TODO: Consider moving this somehwere else, I don't really like it being here.
      IEnumerable<IEntityObjectProperty> validatedProperties = mapping.Properties.Where(m => !string.IsNullOrWhiteSpace(m.FailValidationTest));
      if (validatedProperties.Any())
      {
        Type entityType = this.entityTypes.GetType(mapping.Id);

        foreach (IEntityObjectProperty validatedProperty in validatedProperties)
        {
          string errorMessage = string.Format("{0} Error", validatedProperty.PropertyName);
          IEntityObjectLocaleString locString = validatedProperty.String as IEntityObjectLocaleString;
          if (locString != null && !string.IsNullOrWhiteSpace(locString.ErrorText))
          {
            errorMessage = locString.ErrorText;
          }

          this.builder.AddPropertyFailValidationTest(entityType, validatedProperty.PropertyName, validatedProperty.FailValidationTest, errorMessage);
        }
      }

      return mapping;
    }
  }
}
