using AMCS.Data.Server;
using AMCS.PlatformFramework.IntegrationTests.EntityMapping;
using AMCS.PlatformFramework.IntegrationTests.TestProperties;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using AMCS.Data.Entity;
using TechTalk.SpecFlow;

namespace AMCS.PlatformFramework.IntegrationTests.Steps
{
  [Binding]
  public class EntityMappingSteps: TestBase
  {
    private const string SourceEntityType = "SourceEntityType";
    private const string MappingEntityType = "MappingEntityType";
    private const string EntityObjectBuilder = "EntityObjectBuilder";
    private const string Target = "Target";
    private ScenarioContext scenarioContext;
    private string MappingType = string.Empty;

    public EntityMappingSteps(ScenarioContext scenarioContext)
    {
      this.scenarioContext = scenarioContext;
    }

    public SimpleEntity GetSimpleEntity()
    {
      return new SimpleEntity
      {
        GUID = Guid.NewGuid(),
        Value1 = "Value 1",
        Value2 = 2
      };
    }

    public SimpleTargetEntity GetEmptySimpleTargetEntity()
    {
      return new SimpleTargetEntity();
    }

    public SimpleTargetEntity GetSimpleTargetEntityWithValue3()
    {
      return new SimpleTargetEntity
      {
        Value3 = 42
      };
    }

    public SimpleTargetEntity GetSimpleTargetEntityWithValue2()
    {
      return new SimpleTargetEntity
      {
        Value2 = 21
      };
    }

    public WithNestedSourceEntity GetNestedSourceEntity()
    {
      return new WithNestedSourceEntity
      {
        Value1 = 1,
        NestedEntity = new WithNestedSourceEntity
        {
          Value1 = 2
        },
        NestedEntityList = new List<WithNestedSourceEntity>
        {
          new WithNestedSourceEntity
          {
            Value1 = 3,
            NestedEntity = new WithNestedSourceEntity
            {
              Value1 = 4
            }
          },
          new WithNestedSourceEntity
          {
            Value1 = 5
          },
        }
      };
    }
    [Given(@"entity (.*) mapping type (.*)")]
    public void GivenEntitySimpleEntitySameMappingType(string entityType, string mappingType)
    {
      MappingType = mappingType;
      switch (mappingType.ToUpperInvariant())
      {
        case "SAME":
          scenarioContext[EntityObjectBuilder] = new EntityObjectMapperBuilder()
                                                  .CreateMap<SimpleEntity, SimpleEntity>()
                                                  .Build();

          scenarioContext[SourceEntityType] = GetSimpleEntity();
          break;
        case "SIMPLE":
          scenarioContext[EntityObjectBuilder] = new EntityObjectMapperBuilder()
                                                  .CreateMap<SimpleEntity, SimpleTargetEntity>()
                                                  .Build();

          scenarioContext[SourceEntityType] = GetSimpleEntity();
          break;
        case "NULLABLE":
          scenarioContext[EntityObjectBuilder] = new EntityObjectMapperBuilder()
                                                  .CreateMap<SimpleTargetEntity, SimpleEntity>()
                                                  .Build();

          scenarioContext[SourceEntityType] = GetEmptySimpleTargetEntity();
          break;
        case "SPECIFICPROPERTY":
          scenarioContext[EntityObjectBuilder] = new EntityObjectMapperBuilder()
                                                  .CreateMap<SimpleTargetEntity, SimpleEntity>(p => p
                                                   .Map(nameof(SimpleEntity.Value3), p1 => p1.MapFrom(nameof(SimpleTargetEntity.Value3))))
                                                  .Build();

          scenarioContext[SourceEntityType] = GetSimpleTargetEntityWithValue3();
          break;
        case "BEFOREMAP":
          scenarioContext[EntityObjectBuilder] = new EntityObjectMapperBuilder()
                                                  .CreateMap<SimpleTargetEntity, SimpleEntity>(p => p
                                                  .BeforeMap((from, to) => to.Value2 = 42)
                                                  .Map(nameof(SimpleTargetEntity.Value2), p1 => p1.Ignore()))
                                                  .Build();

          scenarioContext[SourceEntityType] = GetEmptySimpleTargetEntity();
          break;
        case "AFTERMAP":
          scenarioContext[EntityObjectBuilder] = new EntityObjectMapperBuilder()
                                                  .CreateMap<SimpleTargetEntity, SimpleEntity>(p => p
                                                  .AfterMap((from, to) => to.Value2 = 42))
                                                  .Build();
          scenarioContext[SourceEntityType] = GetEmptySimpleTargetEntity();
          break;
        case "CALLBACK":
          scenarioContext[EntityObjectBuilder] = new EntityObjectMapperBuilder()
                                                  .CreateMap<SimpleTargetEntity, SimpleEntity>(p => p
                                                  .Map(nameof(SimpleTargetEntity.Value2), p1 => p1.MapFrom(p2 => p2.Value2 * 2)))
                                                  .Build();
          scenarioContext[SourceEntityType] = GetSimpleTargetEntityWithValue2();
          break;
        case "NESTED":
          scenarioContext[EntityObjectBuilder] = new EntityObjectMapperBuilder()
                                                  .CreateMap<WithNestedSourceEntity, WithNestedTargetEntity>()
                                                  .Build();
          scenarioContext[SourceEntityType] = GetNestedSourceEntity();
          break;
      }
    }

    [When(@"entity is mapped with target (.*) mapping entity")]
    public void WhenEntityIsMappedWithTargetSimpleEntityMappingEntity(string mappingEntity)
    {
      switch (mappingEntity.ToUpperInvariant())
      {
        case "SAME":
        case "CALLBACK":
        case "NULLABLE":
        case "SPECIFICPROPERTY":
        case "BEFOREMAP":
        case "AFTERMAP":
          scenarioContext[Target] = scenarioContext.Get<IEntityObjectMapper>(EntityObjectBuilder).Map<SimpleEntity>(scenarioContext.Get<object>(SourceEntityType));
          break;
        case "SIMPLE":
          scenarioContext[Target] = scenarioContext.Get<IEntityObjectMapper>(EntityObjectBuilder).Map<SimpleTargetEntity>(scenarioContext.Get<object>(SourceEntityType));
          break;
        case "NESTED":
          scenarioContext[Target] = scenarioContext.Get<IEntityObjectMapper>(EntityObjectBuilder).Map<WithNestedTargetEntity>(scenarioContext.Get<object>(SourceEntityType));
          break;
      }
    }

    [Then(@"correct results are shown")]
    public void ThenCorrectResultsAreShown()
    {
      switch (MappingType.ToUpperInvariant())
      {
        case "SAME":
          Assert.AreEqual(scenarioContext.Get<SimpleEntity>(SourceEntityType).GUID, scenarioContext.Get<SimpleEntity>(Target).GUID);
          Assert.AreEqual(scenarioContext.Get<SimpleEntity>(SourceEntityType).Value1, scenarioContext.Get<SimpleEntity>(Target).Value1);
          Assert.AreEqual(scenarioContext.Get<SimpleEntity>(SourceEntityType).Value2, scenarioContext.Get<SimpleEntity>(Target).Value2);
          break;
        case "SIMPLE":
          Assert.AreEqual(scenarioContext.Get<SimpleEntity>(SourceEntityType).GUID, scenarioContext.Get<SimpleTargetEntity>(Target).GUID);
          Assert.AreEqual(scenarioContext.Get<SimpleEntity>(SourceEntityType).Value1, scenarioContext.Get<SimpleTargetEntity>(Target).Value1);
          Assert.AreEqual(scenarioContext.Get<SimpleEntity>(SourceEntityType).Value2, scenarioContext.Get<SimpleTargetEntity>(Target).Value2);
          break;
        case "NULLABLE":
          Assert.AreEqual(scenarioContext.Get<SimpleTargetEntity>(SourceEntityType).Value3, scenarioContext.Get<SimpleEntity>(Target).Value3);
          break;
        case "SPECIFICPROPERTY":
          Assert.AreEqual(scenarioContext.Get<SimpleTargetEntity>(SourceEntityType).Value3, scenarioContext.Get<SimpleEntity>(Target).Value3);
          break;
        case "BEFOREMAP":
        case "AFTERMAP":
        case "CALLBACK":
          Assert.AreEqual(42, scenarioContext.Get<SimpleEntity>(Target).Value2);
          break;
        case "NESTED":
          Assert.AreEqual(scenarioContext.Get<WithNestedSourceEntity>(SourceEntityType).Value1, scenarioContext.Get<WithNestedTargetEntity>(Target).Value1);
          Assert.AreEqual(scenarioContext.Get<WithNestedSourceEntity>(SourceEntityType).NestedEntity.Value1, scenarioContext.Get<WithNestedTargetEntity>(Target).NestedEntity.Value1);
          Assert.AreEqual(scenarioContext.Get<WithNestedSourceEntity>(SourceEntityType).NestedEntityList.Count, scenarioContext.Get<WithNestedTargetEntity>(Target).NestedEntityList.Count);
          Assert.AreEqual(scenarioContext.Get<WithNestedSourceEntity>(SourceEntityType).NestedEntityList[0].Value1, scenarioContext.Get<WithNestedTargetEntity>(Target).NestedEntityList[0].Value1);
          Assert.AreEqual(scenarioContext.Get<WithNestedSourceEntity>(SourceEntityType).NestedEntityList[0].NestedEntity.Value1, scenarioContext.Get<WithNestedTargetEntity>(Target).NestedEntityList[0].NestedEntity.Value1);
          Assert.AreEqual(scenarioContext.Get<WithNestedSourceEntity>(SourceEntityType).NestedEntityList[1].Value1, scenarioContext.Get<WithNestedTargetEntity>(Target).NestedEntityList[1].Value1);
          break;
      }
    }
  }
}
