namespace AMCS.PlatformFramework.Benchmarks
{
  using System;
  using System.Data;
  using BenchmarkDotNet.Attributes;
  using System.Data.SqlClient;
  using AMCS.Data.Configuration;
  
  [SimpleJob(launchCount: 1, warmupCount: 3, targetCount: 5)]
  public class GlossarySqlFunctionBenchmark
  {
    private string prefix;
    private SqlCommand sqlCommand7Code;
    private SqlCommand sqlCommand2Code;
    private SqlCommand sqlCommandBaseCode;

    private SqlCommand sqlCommand7CodeNoMatch;
    private SqlCommand sqlCommand2CodeNoMatch;
    private SqlCommand sqlCommandBaseCodeNoMatch;

    private volatile object result;
    
    [GlobalSetup]
    public void GlobalSetup()
    {
      prefix = Guid.NewGuid().ToString("N");
      var connectionString = ConnectionStringEncryption.DecryptFromConfiguration("PlatformFrameworkConnectionString");
      var sqlConnection = new SqlConnection(connectionString.GetConnectionString());

      var randomRow = new Random().Next(0, 1000);
      
      sqlCommand7Code = new SqlCommand($"SELECT [dbo].[fn_GlossaryTranslation]('{prefix}{randomRow}','en-en-en-en-en-en-en')", sqlConnection);
      sqlCommand2Code = new SqlCommand($"SELECT [dbo].[fn_GlossaryTranslation]('{prefix}{randomRow}','en-en')", sqlConnection);
      sqlCommandBaseCode = new SqlCommand($"SELECT [dbo].[fn_GlossaryTranslation]('{prefix}{randomRow}','en')", sqlConnection);

      sqlCommand7CodeNoMatch = new SqlCommand($"SELECT [dbo].[fn_GlossaryTranslation]('no-match','en-en-en-en-en-en-en')", sqlConnection);
      sqlCommand2CodeNoMatch = new SqlCommand($"SELECT [dbo].[fn_GlossaryTranslation]('no-match','en-en')", sqlConnection);
      sqlCommandBaseCodeNoMatch = new SqlCommand($"SELECT [dbo].[fn_GlossaryTranslation]('no-match','en')", sqlConnection);
      
      var dataTable = new DataTable();
      dataTable.Columns.AddRange(new []
      {
        new DataColumn("Original", typeof(string)),
        new DataColumn("Translated", typeof(string)),
        new DataColumn("LanguageCode", typeof(string))
      });

      for (var i = 0; i < 1000; i++)
        dataTable.Rows.Add($"{prefix}{i}", prefix, "en");
      
      sqlCommand7Code.Connection.Open();
      using var sqlBulkCopy = new SqlBulkCopy(sqlCommand7Code.Connection);
      sqlBulkCopy.DestinationTableName = "GlossaryInternalCache";

      foreach (DataColumn column in dataTable.Columns)
        sqlBulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
      
      sqlBulkCopy.WriteToServer(dataTable);
    }

    [GlobalCleanup]
    public void GlobalCleanup()
    {
      sqlCommand7Code.CommandText = $"DELETE [GlossaryInternalCache] WHERE [Translated] = '{prefix}'";
      sqlCommand7Code.ExecuteScalar();
      
      sqlCommand7Code.Connection.Close();
      sqlCommand7Code.Dispose();
      sqlCommand2Code.Dispose();
      sqlCommandBaseCode.Dispose();
      sqlCommand7CodeNoMatch.Dispose();
      sqlCommand2CodeNoMatch.Dispose();
      sqlCommandBaseCodeNoMatch.Dispose();
      result = null;
  }

    [Benchmark]
    public void GivenSevenLanguageCodes_WhenTranslatingGlossary_ThenTranslatedReturned()
    {
      result = sqlCommand7Code.ExecuteScalar();
    }
    
    [Benchmark]
    public void GivenTwoLanguageCodes_WhenTranslatingGlossary_ThenTranslatedReturned()
    {
      result = sqlCommand2Code.ExecuteScalar();
    }
    
    [Benchmark]
    public void GivenBaseLanguageCodes_WhenTranslatingGlossary_ThenTranslatedReturned()
    {
      result = sqlCommandBaseCode.ExecuteScalar();
    }
    
    [Benchmark]
    public void GivenSevenLanguageCodesWithNoMatch_WhenTranslatingGlossary_ThenInputReturned()
    {
      result = sqlCommand7CodeNoMatch.ExecuteScalar();
    }
    
    [Benchmark]
    public void GivenTwoLanguageCodesWithNoMatch_WhenTranslatingGlossary_ThenInputReturned()
    {
      result = sqlCommand2CodeNoMatch.ExecuteScalar();
    }
    
    [Benchmark]
    public void GivenBaseLanguageCodesWithNoMatch_WhenTranslatingGlossary_ThenInputReturned()
    {
      result = sqlCommandBaseCodeNoMatch.ExecuteScalar();
    }
  }
}