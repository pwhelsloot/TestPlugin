namespace AMCS.Data.Util.IO
{
  using System;
  using System.IO;
  using System.Text;
  using System.Xml;
  using System.Xml.XPath;
  using System.Xml.Xsl;

  public class XmlExporter
  {
    /// <summary>
    /// 
    /// </summary>
    /// <param name="xmlToExport"></param>
    /// <param name="xslStylesheet"></param>
    /// <param name="saveAsFilePath"></param>
    public static void Export(string xmlToExport, string xslStylesheet, string saveAsFilePath)
    {
      using (StringReader xmlStream = new StringReader(xmlToExport))
      {
        using (StringReader xslStream = new StringReader(xslStylesheet))
        {
          using (XmlTextReader xmlReader = new XmlTextReader(xmlStream))
          {
            using (XmlTextReader styleSheetReader = new XmlTextReader(xslStream))
            {
              XslCompiledTransform xslTransform = new XslCompiledTransform();
              xslTransform.Load(styleSheetReader);
              if (xslTransform.OutputSettings != null)
              {
                //Ensure output method of XmlTextWriter is the same as the XSLT (especially for method="text")!
                XmlWriterSettings ws = xslTransform.OutputSettings.Clone();
                ws.CheckCharacters = false;
                using (XmlWriter xmlWriter = XmlWriter.Create(saveAsFilePath, ws))
                {
                  XPathDocument xpathDoc = new XPathDocument(xmlReader);
                  xslTransform.Transform(xpathDoc, null, xmlWriter);
                }

              }
              else
              {
                using (XmlTextWriter xmlWriter = new XmlTextWriter(saveAsFilePath, null))
                {
                  XPathDocument xpathDoc = new XPathDocument(xmlReader);
                  xslTransform.Transform(xpathDoc, null, xmlWriter);
                }
              }
            }
          }
        }
      }
    }

    public static string ExportToString(string xmlToExport, string xslStylesheet)
    {
      using (StringReader xmlStream = new StringReader(xmlToExport))
      {
        using (StringReader xslStream = new StringReader(xslStylesheet))
        {
          using (XmlTextReader xmlReader = new XmlTextReader(xmlStream))
          {
            using (XmlTextReader styleSheetReader = new XmlTextReader(xslStream))
            {
              XslCompiledTransform xslTransform = new XslCompiledTransform();
              xslTransform.Load(styleSheetReader);
              if (xslTransform.OutputSettings != null)
              {
                //Ensure output method of XmlTextWriter is the same as the XSLT (especially for method="text")!
                XmlWriterSettings ws = xslTransform.OutputSettings.Clone();
                ws.CheckCharacters = false;
                using (StringWriter stringWriter = new StringWriter())
                {
                  using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, ws))
                  {
                    XPathDocument xpathDoc = new XPathDocument(xmlReader);
                    xslTransform.Transform(xpathDoc, null, xmlWriter);
                  }
                  return stringWriter.ToString();
                }
              }
              else
              {
                using (StringWriter stringWriter = new StringWriter())
                {
                  using (XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter))
                  {
                    XPathDocument xpathDoc = new XPathDocument(xmlReader);
                    xslTransform.Transform(xpathDoc, null, xmlWriter);
                  }
                  return stringWriter.ToString();
                }
              }
            }
          }
        }
      }
    }
  }
}
