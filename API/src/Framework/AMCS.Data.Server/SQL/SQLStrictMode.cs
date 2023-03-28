namespace AMCS.Data.Server.SQL
{
  internal static class SQLStrictMode
  {
    public static void ValidateInTransaction(IDataSession dataSession)
    {
      if (StrictMode.IsRequireTransaction && !dataSession.IsTransaction())
        throw new StrictModeException("Transactions must be created explicitly with every data session");
    }
  }
}
