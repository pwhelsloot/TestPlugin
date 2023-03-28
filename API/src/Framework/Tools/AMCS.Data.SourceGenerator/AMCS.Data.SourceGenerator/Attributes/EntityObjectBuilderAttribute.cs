using System;
using System.Diagnostics;

namespace AMCS.Data.SourceGenerator.Attributes
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  [Conditional("CodeGeneration")]
  public sealed class EntityObjectBuilderAttribute : Attribute
  {
    /// <summary>
    /// This tells the Source Code Generator for which entity object the builder class has to be generated.<br/>
    /// All the properties that will be generated will come from this class.<br/>
    /// The property in the EntityType object has to be Public with an attribute of: DataMember or EntityMember.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// This is an optional parameter which tells the Source Code Generator<br/>
    /// to setup up default value (mandatory values) while generating the entity object.<br/>
    /// This will be used by GetNew method in Build method. eg: session.GetNew(userId, description)
    /// </summary>
    public string[]? DefaultValues { get; set; }

    /// <summary>
    /// This is an optional parameter used by the Build method.<br/>
    /// This parameter is used to tell Source Code Generator not to create any duplicate data's which already exists in database.<br/>
    /// If present, creates a private partial method FindExistingEntity with entity as return type(refer the Partial methods section for the usage).<br/>
    /// If an object is returned, the Save method of IDataSession will be skipped.
    /// </summary>
    public string[]? DeDuplicate { get; set; }

    public EntityObjectBuilderAttribute(Type entityType)
    {
      EntityType = entityType;
    }
  }
}
