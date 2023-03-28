using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Search
{
  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public enum GridColumnTypeEnum
  {
    [EnumMember]
    ctNormal = 0,

    [EnumMember]
    ctBoolImage = 1,

    [EnumMember]
    ctTrendIndicator = 2
  }

  [Serializable]
  [DataContract(Namespace = "http://www.amcsgroup.com/elemos")]
  public class GridColumnDefinition
  {
    private GridColumnTypeEnum _columnType = GridColumnTypeEnum.ctNormal;
    private string[] _images;

    [DataMember]
    public string Id { get; set; }

    [DataMember]
    public string ParentId { get; set; }

    [DataMember]
    public string Path { get; set; }

    [DataMember]
    public string DataType { get; set; }

    [DataMember]
    public string Header { get; set; }

    [DataMember]
    public string DataFormatString { get; set; }

    [DataMember]
    public int Width { get; set; }

    [DataMember]
    public bool Visible { get; set; }

    [DataMember]
    public GridColumnTypeEnum ColumnType { get { return _columnType; } set { _columnType = value; } }

    [DataMember]
    public string[] Images { get { return _images; } set { _images = value; } }

    [DataMember]
    public int ImageHeight { get; set; }

    [DataMember]
    public int ImageWidth { get; set; }

    [DataMember]
    public bool Editable { get; set; }

    [DataMember]
    public AggregateFunctionDescriptor[] AggregateFunctions { get; set; }

    public GridColumnDefinition Copy()
    {
      return (GridColumnDefinition)this.MemberwiseClone();
    }
  }

  public class AggregateFunctionDescriptor
  {
    public string Type { get; set; }
    public string Caption { get; set; }
    public string StringFormat { get; set; }
  }
}