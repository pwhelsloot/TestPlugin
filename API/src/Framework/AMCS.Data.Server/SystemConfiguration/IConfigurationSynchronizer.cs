using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.SystemConfiguration
{
  public interface IConfigurationSynchronizer<T>
  {
    IList<T> GetAll(ISessionToken userId, IDataSession dataSession);

    void Save(ISessionToken userId, IList<T> elements, Transform transform, IDataSession dataSession);

    IList<string> Validate(IList<T> elements);
  }
}
