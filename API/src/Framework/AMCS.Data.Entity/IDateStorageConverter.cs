using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodaTime;

namespace AMCS.Data.Entity
{
  public interface IDateStorageConverter
  {
    object FromStorage(object value, DateTimeZone timeZone);

    object ToStorage(object value, DateTimeZone timeZone);
  }
}
