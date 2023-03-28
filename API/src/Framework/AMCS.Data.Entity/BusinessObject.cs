using System.Xml.Serialization;

namespace AMCS.Data.Entity
{
  public class BusinessObject
  {
    [XmlAttribute]
    public string Name { get; set; }

    [XmlAttribute]
    public bool AllowWebHooks { get; set; }

    [XmlAttribute]
    public bool AllowUserDefinedFields { get; set; }

    [XmlAttribute]
    public string MappedApiEntity { get; set; }

    [XmlAttribute]
    public string TableName { get; set; }
  }
}