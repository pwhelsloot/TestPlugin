//-----------------------------------------------------------------------------
// <copyright file="XmlSearchResultMappingImporter.cs" company="AMCS Group">
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

  public class XmlSearchResultMappingImporter : XmlMappingImporter
  {
    private readonly ILocalisedStringResourceCache stringCache;

    public XmlSearchResultMappingImporter(Assembly searchXmlAssembly, string searchXmlNamespace, bool ignoreTranslations = false)
      : base(searchXmlAssembly, searchXmlNamespace)
    {
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

    public override IMapping ImportXML(XElement root)
    {
      SearchResultMapping mapping = new SearchResultMapping();
      mapping.Id = (string)root.Attribute("id");

      Guid fileId = Guid.Empty;
      if (!Guid.TryParse((string)root.Attribute("fileId"), out fileId))
      {
        fileId = Guid.NewGuid();
      }

      mapping.FileId = fileId;

      mapping.AuditTable = (string)root.Attribute("auditTable");
      mapping.AuditQuery = (string)root.Attribute("auditQuery");
      mapping.AuditKeyField = (string)root.Attribute("auditKeyField");

      var columns = root.Element(XmlNs + "columns");
      if (columns == null)
      {
        mapping.Columns = new List<ISearchResultColumn>();
      }
      else
      {
        mapping.Columns =
        (from columnElement in root.Element(XmlNs + "columns").Descendants(XmlNs + "column")
         select new SearchResultColumn
         {
           ColumnName = (string)columnElement.Attribute("name"),
           IsKey = Convert.ToBoolean((string)columnElement.Attribute("isKey")),
           Width = (int?)columnElement.Attribute("width"),
           StringFormat = (string)columnElement.Attribute("stringFormat"),
           DisplayType = (int?)columnElement.Attribute("displayType"),
           ImageReferenceOne = (string)columnElement.Attribute("imageReferenceOne"),
           ImageReferenceTwo = (string)columnElement.Attribute("imageReferenceTwo"),
           ImageHeight = (int?)columnElement.Attribute("imageHeight"),
           ImageWidth = (int?)columnElement.Attribute("imageWidth"),
           Editable = Convert.ToBoolean((string)columnElement.Attribute("editable")),
           String = this.GetTranslatableLocaleString(mapping.Id, columnElement),
           AggregateFunctions =
           (from aggregateFunctionElement in columnElement.Elements(XmlNs + "aggregateFunction")
            select new TranslatableAggregate
            {
              Type = (string)aggregateFunctionElement.Attribute("type"),
              StringFormat = (string)aggregateFunctionElement.Attribute("stringFormat"),
              Caption = this.GetTranslatableLocaleString(mapping.Id, aggregateFunctionElement)
            }).ToArray<ITranslatableAggregate>()
         }).ToList<ISearchResultColumn>();
      }

      return mapping;
    }

    /// <summary>
    /// We've moved to RESX files for the translations however there has been so much stuff added around these mapping files over the years
    /// that it's very hard to get rid of them.  For this reason we're sticking with them for now but rather than holding translations in the 
    /// actual XML files they're being pulled from RESX and stuck into the "mapping" objects, so higher levels aren't aware of any difference.
    /// </summary>
    /// <param name="searchName"></param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    private IList<ILocaleString> GetTranslations(string searchName, string defaultValue)
    {
      IList<ILocaleString> result = null;
      if (this.stringCache != null && !string.IsNullOrWhiteSpace(defaultValue))
      {
        string valuePrimaryKey = string.Format("{0}#{1}:{2}", ResourceStringOwnerType.Search, searchName, defaultValue);
        string valueTran = this.stringCache.GetString(valuePrimaryKey, defaultValue);

        LocaleString stringTranslation = new LocaleString()
        {
          Locale = CultureInfo.CurrentUICulture.Name,
          Value = valueTran,
        };

        // we don't need to pull in strings from all locales because we're only interested in the current one for the life of the process
        result = new List<ILocaleString>() { stringTranslation };
      }

      return result;
    }

    private ITranslatableLocaleString GetTranslatableLocaleString(string searchName, XElement baseElement)
    {
      ITranslatableLocaleString result =
        (from stringElement in baseElement.Elements(XmlNs + "string")
         select new TranslatableLocaleString
         {
           Locale = (string)stringElement.Attribute("locale"),
           Value = (string)stringElement.Attribute("value"),
         }).SingleOrDefault();

      if (result != null && !string.IsNullOrWhiteSpace(result.Value))
      {
        result.Translations = this.GetTranslations(searchName, result.Value);
      }

      return result;
    }
  }
}
