namespace AMCS.Data.Server
{
  using System;

  public interface IEntityObjectMapperBuilder
  {
    EntityObjectMapperBuilder CreateMap<TFrom, TTo>(Func<IEntityObjectEntityMapperBuilder<TFrom, TTo>, IEntityObjectEntityMapperBuilder<TFrom, TTo>> configure = null);
  }
}