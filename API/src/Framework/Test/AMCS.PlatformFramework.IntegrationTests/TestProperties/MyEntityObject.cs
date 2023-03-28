using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;
using AMCS.Data.Server;
using AMCS.Data.Server.SQL;

namespace AMCS.PlatformFramework.IntegrationTests.TestProperties
{
  public abstract class MyEntityObject : EntityObject
  {
    public int MyId { get; set; }
  }

  public class MyEntityObject1 : MyEntityObject
  {
  }

  public class MyEntityObject2 : MyEntityObject
  {
  }

  public class MyEntityObject3 : MyEntityObject
  {
  }

  // The generic service class. If there's no specific one (i.e. 1 and 2),
  // this one is used.

  public class MyEntityObjectService<T> : EntityObjectService<T>
    where T : MyEntityObject
  {
    public MyEntityObjectService(IMyEntityObjectAccess<T> dataAccess)
      : base(dataAccess)
    {
    }

    public virtual T MyMethod()
    {
      return ((IMyEntityObjectAccess<T>)DataAccess).MyMethod();
    }
  }

  // A custom implementation for 3.

  public class MyEntityObjectService3 : MyEntityObjectService<MyEntityObject3>
  {
    public MyEntityObjectService3(IMyEntityObjectAccess<MyEntityObject3> dataAccess)
      : base(dataAccess)
    {
    }

    public override MyEntityObject3 MyMethod()
    {
      var result = base.MyMethod();
      result.MyId = 44;
      return result;
    }
  }

  // The generic access class. If there's no specific one (i.e. 2 and 3),
  // this one is used.

  public interface IMyEntityObjectAccess<T> : IEntityObjectAccess<T>
    where T : MyEntityObject
  {
    T MyMethod();
  }

  public class MySQLEntityObjectAccess<T> : SQLEntityObjectAccess<T>, IMyEntityObjectAccess<T>
    where T : MyEntityObject
  {
    public virtual T MyMethod()
    {
      var result = (T)Activator.CreateInstance(typeof(T));
      result.MyId = 42;
      return result;
    }
  }

  // A custom implementation for 1.

  public class MySQLEntityObjectAccess1 : MySQLEntityObjectAccess<MyEntityObject1>
  {
    public override MyEntityObject1 MyMethod()
    {
      var result = base.MyMethod();
      result.MyId = 43;
      return result;
    }
  }
}
