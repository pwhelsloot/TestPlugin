using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AMCS.Data.Schema.Discovery
{
  public class RelationshipAnalyser : IRelationshipAnalyser
  {
    private ITableSchemaDiscoverer _tableSchemaDiscoverer;
    private Dictionary<string, IList<IColumn>> _tableDependentColumns = new Dictionary<string, IList<IColumn>>();
    private IColumn _headColumn;

    public RelationshipAnalyser(ITableSchemaDiscoverer tableSchemaDiscoverer)
    {
      _tableSchemaDiscoverer = tableSchemaDiscoverer;
    }

    public void Analyse(string schema, IColumn headColumn)
    {
      _headColumn = headColumn;

      if (!IsSchemaCache(schema))
        CreateSchemaCache(schema);

      IList<ITable> schemaTables = GetSchemaFromCache(schema);
      DiscoverCoreDependencies(schemaTables, headColumn);
      DiscoverDeeperDependencies(schemaTables, headColumn);
    }

    /*
    public IEnumerable<IList<IColumn>> GetGuaranteedRoutesToColumn(string fromTable, IColumn fromColumn = null)
    {
      if (!_tableDependentColumns.ContainsKey(fromTable))
        yield break;

      List<IColumn> route = new List<IColumn>();
      foreach (IColumn column in _tableDependentColumns[fromTable].Where(c => !c.IsNullable))
      {
        route.Add(column);
        IForeignKey fk = null;
        if (column.Keys != null)
          // FK's are listed both when the key exists on this column and when another key points to it.  Only interested here in the keys on the actual column
          fk = (IForeignKey)column.Keys.FirstOrDefault(k => k is IForeignKey && fromTable == string.Format("{0}.{1}", k.TableSchema, k.TableName));
        
        // keep things simple and only consider non-composite FK's to PK's (for now at least)
        if(fk != null && fk.ColumnNames != null && fk.ColumnNames.Count == 1 && fk.ReferencedKey.ColumnNames != null && fk.ReferencedKey.ColumnNames.Count == 1 && fk.ReferencedKey is IPrimaryKey)
        {
          string referencedTable = string.Format("{0}.{1}", fk.ReferencedKey.TableSchema, fk.ReferencedKey.TableName);
          string referencedColumn = fk.ReferencedKey.ColumnNames[0];
          if (!_tableDependentColumns.ContainsKey(referencedTable))
            throw new Exception("Did not find expected table on route to column");

          IList<IColumn> referencedTableColumns = _tableDependentColumns[referencedTable];
          IColumn refCol = referencedTableColumns.FirstOrDefault(c => c.Name == referencedColumn);
          if(refCol != null)
          {
            if(!refCol.IsNullable)
            {
              route.Add()
            }
             
          }
        }
      }
    }
    */
    private bool IsSchemaCache(string schema)
    {
      return File.Exists(string.Format("c:\\temp\\{0}schema.bin", schema));
    }

    private void CreateSchemaCache(string schema)
    {
      Console.WriteLine("Working...");

      IList<ITable> tables = _tableSchemaDiscoverer.Discover(schema);
      Console.WriteLine("Found {0} tables", tables.Count);

      var json = JsonConvert.SerializeObject(tables);
      var jsonBytes = Encoding.UTF8.GetBytes(json);

      using (FileStream fs = new FileStream(string.Format("c:\\temp\\{0}schema.bin", schema), FileMode.Create))
      {
        using (var bw = new BinaryWriter(fs, Encoding.UTF8))
        {
          bw.Write(jsonBytes);
        }
      }
      Console.WriteLine("Saved to disk");
    }

    private IList<ITable> GetSchemaFromCache(string schema)
    {
      using (FileStream fs = new FileStream(string.Format("c:\\temp\\{0}schema.bin", schema), FileMode.Open))
      {
        using (var br = new BinaryReader(fs))
        {
          string json = Encoding.UTF8.GetString(br.ReadBytes((int)fs.Length));
          return JsonConvert.DeserializeObject<IList<ITable>>(json);
        }
      }
    }

    private void DiscoverCoreDependencies(IList<ITable> tables, IColumn headColumn)
    {
      foreach (ITable table in tables)
      {
        if (table.Name == string.Format("{0}.{1}", headColumn.ParentTableSchema, headColumn.ParentTableName))
          continue;

        foreach (IColumn column in table.Columns)
        {
          if (column.Keys != null)
          {
            foreach (IKey key in column.Keys)
            {
              IForeignKey fk = key as IForeignKey;
              if (fk != null)
              {
                IKey reference = fk.ReferencedKey;
                if (reference.TableSchema == headColumn.ParentTableSchema && reference.TableName == headColumn.ParentTableName)
                {
                  if (!_tableDependentColumns.ContainsKey(table.Name))
                    _tableDependentColumns.Add(table.Name, new List<IColumn>());
                  _tableDependentColumns[table.Name].Add(column);
                  //break;
                }
              }
            }
          }
        }
      }
    }

    private void DiscoverDeeperDependencies(IList<ITable> tables, IColumn headColumn)
    {
      Dictionary<string, IList<string>> dependentTables = new Dictionary<string, IList<string>>();
      foreach (ITable table in tables)
      {
        if (table.Name == string.Format("{0}.{1}", headColumn.ParentTableSchema, headColumn.ParentTableName))// || _tableDependentColumns.ContainsKey(table.Name))
          continue;

        foreach (IColumn column in table.Columns)//.Where(c=>!c.IsNullable))
        {
          if (column.Keys != null)
          {
            foreach (IKey key in column.Keys)
            {
              IForeignKey fk = key as IForeignKey;
              if (fk != null && table.Name != string.Format("{0}.{1}", fk.ReferencedKey.TableSchema, fk.ReferencedKey.TableName)) // ignore self lookups
              {
                IKey reference = fk.ReferencedKey;
                string referenceTable = string.Format("{0}.{1}", reference.TableSchema, reference.TableName);
                if (_tableDependentColumns.ContainsKey(referenceTable))
                {
                  if (!_tableDependentColumns.ContainsKey(table.Name))
                    _tableDependentColumns.Add(table.Name, new List<IColumn>());
                  _tableDependentColumns[table.Name].Add(column);
                  //break;
                }
              }
            }
          }
        }
      }
    }
  }
}
