using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Entity.Search
{
  [DataContract]
  [Serializable]
  public class SerializableDynamicObject : IDynamicMetaObjectProvider, INotifyPropertyChanged
  {
    [DataMember]
    private IDictionary<string, object> data = new Dictionary<string, object>();

    #region IDynamicMetaObjectProvider implementation

    public DynamicMetaObject GetMetaObject(Expression expression)
    {
      return new SerializableDynamicMetaObject(expression,
        BindingRestrictions.GetInstanceRestriction(expression, this), this);
    }

    #endregion IDynamicMetaObjectProvider implementation

    #region Helper methods for dynamic meta object support

    internal object setValue(string name, object value)
    {
      if (!data.ContainsKey(name))
      {
        data.Add(name, value);
      }
      else
      {
        data[name] = value;
      }
      return value;
    }

    internal object getValue(string name)
    {
      object value;
      if (!data.TryGetValue(name, out value))
      {
        value = null;
      }
      return value;
    }

    internal IEnumerable<string> getDynamicMemberNames()
    {
      return data.Keys;
    }

    #endregion Helper methods for dynamic meta object support

    public object this[string columnName]
    {
      get
      {
        if (data.ContainsKey(columnName))
        {
          return data[columnName];
        }

        return null;
      }
      set
      {
        if (!data.ContainsKey(columnName))
        {
          data.Add(columnName, value);

          OnPropertyChanged(columnName);
        }
        else
        {
          if (data[columnName] != value)
          {
            data[columnName] = value;

            OnPropertyChanged(columnName);
          }
        }
      }
    }

    private void OnPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
      {
        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    public event PropertyChangedEventHandler PropertyChanged;
  }

  public class SerializableDynamicMetaObject : DynamicMetaObject
  {
    private Type objType;

    public SerializableDynamicMetaObject(System.Linq.Expressions.Expression expression, BindingRestrictions restrictions, object value)
      : base(expression, restrictions, value)
    {
      objType = value.GetType();
    }

    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    {
      var self = this.Expression;
      var dynObj = (SerializableDynamicObject)this.Value;
      var keyExpr = Expression.Constant(binder.Name);
      var getMethod = objType.GetMethod("getValue", BindingFlags.NonPublic | BindingFlags.Instance);
      var target = Expression.Call(Expression.Convert(self, objType),
                                   getMethod,
                                   keyExpr);
      return new DynamicMetaObject(target,
        BindingRestrictions.GetTypeRestriction(self, objType));
    }

    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    {
      var self = this.Expression;
      var keyExpr = Expression.Constant(binder.Name);
      var valueExpr = Expression.Convert(value.Expression, typeof(object));
      var setMethod = objType.GetMethod("setValue", BindingFlags.NonPublic | BindingFlags.Instance);
      var target = Expression.Call(Expression.Convert(self, objType),
      setMethod,
      keyExpr,
      valueExpr);
      return new DynamicMetaObject(target,
        BindingRestrictions.GetTypeRestriction(self, objType));
    }

    public override IEnumerable<string> GetDynamicMemberNames()
    {
      var dynObj = (SerializableDynamicObject)this.Value;
      return dynObj.getDynamicMemberNames();
    }
  }
}