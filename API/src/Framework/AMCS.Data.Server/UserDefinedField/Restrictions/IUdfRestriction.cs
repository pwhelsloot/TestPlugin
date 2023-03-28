namespace AMCS.Data.Server.UserDefinedField.Restrictions
{
  using AMCS.Data.Entity.UserDefinedField;

  public interface IUdfRestriction
  {
    void Execute(IUdfField udfField, IUdfResultObject resultObject, string metadata);
  }
}