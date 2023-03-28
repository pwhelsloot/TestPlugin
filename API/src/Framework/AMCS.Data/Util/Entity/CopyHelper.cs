namespace AMCS.Data.Util.Entity
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Reflection;
  using System.Text;

  public class CopyHelper
  {
    public static void CopyEntity(object source, object destination)
    {
      PropertyInfo[] srcProps = source.GetType().GetProperties();
      Type destType = destination.GetType();
      foreach (PropertyInfo srcProp in srcProps)
      {
        if (srcProp.GetGetMethod() != null)
        {
          PropertyInfo destProp = destType.GetProperty(srcProp.Name);
          if (destProp != null && destProp.GetSetMethod() != null && destProp.PropertyType.FullName == srcProp.PropertyType.FullName)
          {
            object srcPropValue = srcProp.GetValue(source, null);
            destProp.SetValue(destination, srcPropValue, null);
          }
        }
      }
    }
  }
}
