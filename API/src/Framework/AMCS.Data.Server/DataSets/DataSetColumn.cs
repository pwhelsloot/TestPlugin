using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.DataSets
{
  [DebuggerDisplay("Label = {Label}, Property = {Property.Name}, IsDefault = {IsDefault}, IsMandatory = {IsMandatory}")]
  public class DataSetColumn
  {
    public string Label { get; }

    public EntityObjectProperty Property { get; }

    public bool IsReadOnly { get; }

    public bool IsMandatory { get; }

    public bool IsDefault { get; }

    public DataSetColumn(string label, EntityObjectProperty property, bool isReadOnly, bool isMandatory, bool isDefault)
    {
      Label = label;
      Property = property;
      IsReadOnly = isReadOnly;
      IsMandatory = isMandatory;
      IsDefault = isDefault;
    }
  }
}
