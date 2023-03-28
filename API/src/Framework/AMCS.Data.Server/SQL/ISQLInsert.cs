using System.Collections.Generic;

namespace AMCS.Data.Server.SQL
{
  public interface ISQLInsert : ISQLExecutable<ISQLInsert>
  {
    ISQLInsert IdentityInsert();

    ISQLInsert IdentityInsert(bool value);

    ISQLInsert SetIdentityInsertOn(bool value);

    ISQLInsert CreateAuditRecord();

    ISQLInsert CreateAuditRecord(bool value);

    ISQLInsert InsertOverridableDynamicColumns();

    ISQLInsert InsertOverridableDynamicColumns(bool value);

    ISQLInsert RestrictToFields(IList<string> fields);

    ISQLInsert TableName(string tableName);

    void Execute();

    int ExecuteReturnIdentity();
  }
}
