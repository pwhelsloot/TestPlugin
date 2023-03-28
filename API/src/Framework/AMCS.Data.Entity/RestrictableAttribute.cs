using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  /// <summary>
  /// If this attribute decorates a boolean property then it will be compatable with the
  /// Access Restrictions interface in the security plugin - basically an boolean GUI component will
  /// display on the UI for this property allowing for it to be toggled.  The "Description" is a user friendly name
  /// which will be displayed so that the user knows what they are toggling
  /// </summary>
  [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
  public class Restrictable : Attribute
  {
    public string Description { get; private set; }

    public Restrictable(string description)
    {
      Description = description;
    }
  }
}