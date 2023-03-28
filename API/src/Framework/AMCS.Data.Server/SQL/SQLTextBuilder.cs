using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.SQL
{
  public class SQLTextBuilder
  {
    private readonly StringBuilder sb = new StringBuilder();

    public int Length => sb.Length;

    public void Clear()
    {
      sb.Clear();
    }

    public SQLTextBuilder Text(char c)
    {
      sb.Append(c);
      return this;
    }

    public SQLTextBuilder Text(string text)
    {
      sb.Append(text);
      return this;
    }

    public SQLTextBuilder Text(string format, params object[] args)
    {
      sb.AppendFormat(format, args);
      return this;
    }

    public SQLTextBuilder Name(string name)
    {
      if (name.Length > 2 && name[0] == '[' && name[name.Length - 1] == ']')
      {
        sb.Append(name);
      }
      else
      {
        // We need to guess whether the name starts with a schema because sometimes
        // we get passed one even though it's presented as e.g. a table name.

        int pos = name.IndexOf('.');
        if (pos != -1)
        {
          sb
            .Append('[').Append(name, 0, pos).Append("].[")
            .Append(name, pos + 1, name.Length - (pos + 1)).Append(']');
        }
        else
        {
          sb.Append('[').Append(name).Append(']');
        }
      }

      return this;
    }

    public SQLTextBuilder SearchRankedNames(params string[] columns)
    {
      sb.Append("(");

      int count = 0;

      foreach (var name in columns)
      {
        count++;

        Name(name);

        if (count < columns.Length)
        {
          sb.Append(",");

        }
      }

      sb.Append(")");

      return this;
    }

    public SQLTextBuilder ParameterName(IDataParameter parameter)
    {
      Text(parameter.ParameterName);
      return this;
    }

    public SQLTextBuilder ParameterName(string name)
    {
      if (!name.StartsWith("@"))
        Text("@");
      Text(name);
      return this;
    }

    public SQLTextBuilder ParameterLiteral(string name)
    {
      Text("{=");
      if (name.StartsWith("@"))
        Text(name.Substring(1));
      else
        Text(name);
      Text("}");
      return this;
    }

    public SQLTextBuilder TableName(EntityObjectAccessor accessor)
    {
      return TableName(accessor.SchemaName, accessor.TableName);
    }

    public SQLTextBuilder TableName(EntityObject entityObject)
    {
      return TableName(entityObject.GetSchemaName(), entityObject.GetTableName());
    }

    public SQLTextBuilder TableName(string schema, string tableName)
    {
      Name(schema);
      Text('.');
      Name(tableName);
      return this;
    }

    public override string ToString()
    {
      return sb.ToString();
    }
  }
}
