using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Validation
{
  public static class RegEx
  {
    public const string Email = @"^\s*[A-Za-z0-9_\-\.]+@(([A-Za-z0-9\-])+\.)+([A-Za-z\-])+\s*$"; // accepts leading / trailing whitespace - the email is trimmed on save.
    public const string SimpleEmail = @"^[^@\s]+@[^@\s]+\.[^@\s]+$"; // a simplified version for more relaxed email checking. This is used when registering/creating contacts+customers+sites on portal
  }
}
