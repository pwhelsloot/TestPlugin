using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity
{
  public class EntityObjectColumn
  {
    public string PropertyName { get; }
    public string ColumnName { get; }
    public bool CanRead { get; }
    public bool CanWrite { get; }
    public DateStorage DateStorage { get; }
    public IDateStorageConverter DateStorageConverter { get; }
    public string TimeZoneMember { get; }

    internal EntityObjectColumn(string propertyName, string columnName, bool canRead, bool canWrite, DateStorage dateStorage, IDateStorageConverter dateStorageConverter, string timeZoneMember)
    {
      PropertyName = propertyName;
      ColumnName = columnName;
      CanRead = canRead;
      CanWrite = canWrite;
      DateStorage = dateStorage;
      DateStorageConverter = dateStorageConverter;
      TimeZoneMember = timeZoneMember;
    }
  }
}
