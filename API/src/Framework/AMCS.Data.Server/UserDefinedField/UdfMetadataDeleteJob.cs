namespace AMCS.Data.Server.UserDefinedField
{
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Threading;
  using AMCS.Data.Server.Plugin;
  using AMCS.Data.Entity.UserDefinedField;
  using AMCS.JobSystem;
  using Dapper;

  [DisplayName("UDF Metadata Clean-up")]
  [Job(DuplicateMode = DuplicateMode.Deny)]
  public class UdfMetadataDeleteJob : JobHandler<UdfMetadataDeleteJobRequest>
  {
    private readonly IUdfMetadataService udfMetadataService;

    private const int DefaultMaxDeletedRowsAllowed = 10000;

    private static readonly string DeleteDataQuery = $@"
      DECLARE @DeleteCount INT;

      WITH DeleteStringCTE AS
      (
        SELECT TOP @MaxDeleteCount [UdfFieldId] FROM [ext].[UdfDataString] WHERE [UdfFieldId] IN @UdfFieldId
      )
      DELETE FROM DeleteStringCTE

      SELECT @DeleteCount = @@ROWCOUNT
      IF @DeleteCount >= @MaxDeleteCount
        RETURN;

      WITH DeleteTextCTE AS
      (
        SELECT TOP @MaxDeleteCount [UdfFieldId] FROM [ext].[UdfDataText] WHERE [UdfFieldId] IN @UdfFieldId
      )
      DELETE FROM DeleteTextCTE

      SELECT @DeleteCount = @DeleteCount + @@ROWCOUNT
      IF @DeleteCount >= @MaxDeleteCount
        RETURN;

      WITH DeleteIntegerCTE AS
      (
        SELECT TOP @MaxDeleteCount [UdfFieldId] FROM [ext].[UdfDataInteger] WHERE [UdfFieldId] IN @UdfFieldId
      )
      DELETE FROM DeleteIntegerCTE

      SELECT @DeleteCount = @DeleteCount + @@ROWCOUNT
      IF @DeleteCount >= @MaxDeleteCount
        RETURN;

      WITH DeleteDecimalCTE AS
      (
        SELECT TOP @MaxDeleteCount [UdfFieldId] FROM [ext].[UdfDataDecimal] WHERE [UdfFieldId] IN @UdfFieldId
      )
      DELETE FROM DeleteDecimalCTE";

    public UdfMetadataDeleteJob()
    {
      udfMetadataService = DataServices.Resolve<IUdfMetadataService>();
    }

    protected override void Execute(IJobContext context, ISessionToken userId, UdfMetadataDeleteJobRequest request)
    {
      using (var timeoutToken = CancellationTokenSource.CreateLinkedTokenSource(context.CancellationToken))
      using (IDataSession session = BslDataSessionFactory.GetDataSession(userId))
      using (var transaction = session.CreateTransaction())
      {
        var currentRowsDeleted = 0;
        var maxDeleteAllowed = request.MaximumDeleteAmount ?? DefaultMaxDeletedRowsAllowed;
        var deleteDataQuery = DeleteDataQuery.Replace("@MaxDeleteCount", maxDeleteAllowed.ToString());
        
        try
        {
          timeoutToken.CancelAfter(request.MaximumRuntimeMinutes ?? TimeSpan.FromMinutes(5));

          while (currentRowsDeleted < maxDeleteAllowed)
          {            
            // Need to fetch this on each iteration to ensure that if we don't have anything to delete, then break out
            // of the loop. Otherwise we wait on the max rows deleted to be flagged
            var udfMetadata = session.GetAll<UdfMetadataEntity>(userId, true);
            var udfMetadataToDelete = udfMetadata.Where(metadata => metadata.IsDeletePending).ToList();

            if (!udfMetadataToDelete.Any())
              break;

            var parameters = new DynamicParameters();

            parameters.Add("@UdfFieldId", udfMetadataToDelete.Select(metadata => metadata.UdfMetadataId));
            var currentAmountDeleted =
              session.GetConnection().Execute(deleteDataQuery, parameters, session.GetTransaction());

            currentRowsDeleted += currentAmountDeleted;

            var nonDeletedItems = udfMetadata.Where(metadata => !metadata.IsDeletePending);
            var groupedItems = nonDeletedItems.GroupBy(metadata => metadata.Namespace);
            foreach (var groupedItem in groupedItems)
            {
              udfMetadataService.SaveUdfMetadata(groupedItem.ToList(), groupedItem.Key, userId, session);
            }
          }

          transaction.Commit();
        }
        catch (OperationCanceledException)
        {
          transaction.Commit();
        }
      }
    }
  }
}