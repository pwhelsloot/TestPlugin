using AMCS.Data;
using AMCS.Data.Server;
using AMCS.PlatformFramework.Entity.Api.Recipe;
using System.Threading;

namespace AMCS.PlatformFramework.Server.Api.Recipe
{
  public class ApiRecipeStepperSaveService : EntityObjectService<ApiRecipeStepperSave>
  {
    public ApiRecipeStepperSaveService(IEntityObjectAccess<ApiRecipeStepperSave> dataAccess)
    : base(dataAccess)
    {
    }

    public override int? Save(ISessionToken userId, ApiRecipeStepperSave entity, IDataSession existingDataSession = null)
    {
      Thread.Sleep(2000);
      return 1;
    }
  }
}
