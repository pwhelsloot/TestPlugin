using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Services
{
  public enum AuthenticationStatus
  {
    InvalidCredentials,
    Unknown,
    Locked,
    DuplicateEmail
  }
}