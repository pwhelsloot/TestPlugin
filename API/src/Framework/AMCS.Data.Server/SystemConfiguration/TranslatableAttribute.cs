namespace AMCS.Data.Server.SystemConfiguration
{
  using System;

  /// <summary>
  /// Properties set with this attribute are translatable
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
  public class TranslatableAttribute : Attribute
  {
    public bool Translatable { get; set; }
    public string Name { get; set; }

    public TranslatableAttribute()
    {
      Translatable = true;
      Name = null;
    }
  }
}