using System;

namespace AMCS.Data.Entity
{
  /// <summary>
  /// Unless overridden, properties set with this attribute will not be persisted to the database
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
  public class DynamicColumn : Attribute
  {
    public string Name { get; set; }

    // set if there are certain circumstances when you might want to write the decorated property
    // to the DB
    public bool IsOverridable { get; set; }

    public DynamicColumn()
    {
    }

    public DynamicColumn(string name)
    {
      this.Name = name;
    }
  }
}