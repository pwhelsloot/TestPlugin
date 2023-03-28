using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AMCS.Data.Server.SystemConfiguration
{
  [XmlType(IncludeInSchema = false)]
  public abstract class ConfigurationCollection
  {
    [XmlIgnore]
    public abstract Transform Transform { get; set; }

    public abstract Type GetItemType();

    public abstract IList GetItems();

    public abstract void SetItems(IList items);
  }

  public abstract class ConfigurationCollection<T> : ConfigurationCollection
  {
    [XmlAttribute]
    [DefaultValue(Transform.Default)]
    public override Transform Transform { get; set; }

    [XmlIgnore]
    public abstract List<T> Items { get; set; }

    protected ConfigurationCollection()
    {
      Items = new List<T>();
    }

    public override Type GetItemType() => typeof(T);

    public override IList GetItems() => Items;

    public override void SetItems(IList items) => Items = (List<T>)items;
  }
}
