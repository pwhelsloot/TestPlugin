using System;
using System.Collections;
using System.Collections.Generic;
using AMCS.Data.Configuration;
using AMCS.Data.Entity;
using Autofac;

namespace AMCS.Data.Server.Configuration
{
  partial class DataConfigurationExtensions
  {
    public static void ConfigureEntityObjectMapper(this DataConfiguration self,
      Action<IEntityObjectMapperBuilder> builder)
    {
      self.ContainerBuilder
        .Register(p => new EntityObjectMapperBuilderRegistrationService(builder))
        .AutoActivate()
        .SingleInstance()
        .AsSelf();
    }

    public static void ConfigureEntityObjectMapperService(this DataConfiguration self)
    {
      self.ContainerBuilder
        .RegisterType<EntityObjectMapperBuilder>()
        .AutoActivate()
        .SingleInstance()
        .AsSelf()
        .As<IEntityObjectMapperBuilder>();

      self.ContainerBuilder
        .Register(p => new EntityObjectMapperService(p.Resolve<ISetupService>()))
        .AutoActivate()
        .SingleInstance()
        .AsSelf()
        .As<IEntityObjectMapper>();
    }

    private class EntityObjectMapperBuilderRegistrationService : IDelayedStartup
    {
      private readonly Action<EntityObjectMapperBuilder> action;

      public EntityObjectMapperBuilderRegistrationService(Action<EntityObjectMapperBuilder> action)
      {
        this.action = action;
      }

      public void Start()
      {
        DataServices.Resolve<EntityObjectMapperBuilder>().AddAction(action);
      }
    }

    private class EntityObjectMapperService : IEntityObjectMapper
    {
      private IEntityObjectMapper owner;

      public EntityObjectMapperService(ISetupService setupService)
      {
        setupService.RegisterSetup(Start, 1000);
      }

      private void Start()
      {
        owner = DataServices.Resolve<EntityObjectMapperBuilder>().Build();
      }

      public T Map<T>(object value)
      {
        return owner.Map<T>(value);
      }

      public object Map(object value, Type targetType)
      {
        return owner.Map(value, targetType);
      }

      public void Map(object value, object target)
      {
        owner.Map(value, target);
      }

      public IList<T> MapList<T>(IEnumerable values)
      {
        return owner.MapList<T>(values);
      }

      public IList<T> MapList<T>(IEnumerable values, Type targetType)
      {
        return owner.MapList<T>(values, targetType);
      }

      public IList MapList(IEnumerable values, Type targetType)
      {
        return owner.MapList(values, targetType);
      }

      public void MapList(IEnumerable values, Type targetType, IList target)
      {
        owner.MapList(values, targetType, target);
      }

      public void MapArray(Array values, Type targetType, Array target)
      {
        owner.MapArray(values, targetType, target);
      }

      public bool TryMapProperty(EntityObjectProperty property, Type targetType, out EntityObjectProperty result)
      {
        return owner.TryMapProperty(property, targetType, out result);
      }
    }
  }
}