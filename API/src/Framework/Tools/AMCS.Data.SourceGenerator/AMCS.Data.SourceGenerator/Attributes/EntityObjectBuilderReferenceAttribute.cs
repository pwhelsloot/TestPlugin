using System;
using System.Diagnostics;

namespace AMCS.Data.SourceGenerator.Attributes
{
  /// <summary>
  /// This is an optional attribute.
  /// As our entity objects doesn't have reference object, we cannot create the With method for the references (foreign keys).
  /// To over come this, this attribute is defined at the class level on the manually created partial builder class.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
  [Conditional("CodeGeneration")]
  public sealed class EntityObjectBuilderReferenceAttribute : Attribute
  {
    public string Name { get; }

    public Type Builder { get; }

    public EntityObjectBuilderReferenceAttribute(string name, Type builder)
    {
      Name = name;
      Builder = builder;
    }
  }
}
