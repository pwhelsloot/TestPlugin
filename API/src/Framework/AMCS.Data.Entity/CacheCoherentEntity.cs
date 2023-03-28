namespace AMCS.Data.Entity
{
  using System;
  using System.Linq;

  [Serializable]
  public abstract class CacheCoherentEntity : EntityObject, ICacheCoherentEntity
  {
    public virtual bool IsEqualTo(object obj)
    {
      var type = GetType();

      var entityMembers = type.GetProperties()
        .Where(property => property.GetCustomAttributes(typeof(EntityMemberAttribute), true).Any())
        .ToList();

      foreach (var entityMember in entityMembers)
      {
        var selfValue = type.GetProperty(entityMember.Name).GetValue(this, null);
        var objValue = type.GetProperty(entityMember.Name).GetValue(obj, null);

        if(selfValue == null && objValue != null)
          return false;
        
        if (selfValue != null && !selfValue.Equals(objValue))
          return false;
      }

      return true;
    }
  }
}