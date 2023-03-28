using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.Util
{
  public interface IPasswordHashingAlgorithm
  {
    bool IsMatch(string hashedPassword);

    string Hash(string userName, string password);

    bool Verify(string userName, string password, string hashedPassword);
  }
}
