using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AMCS.Data.Server;
using AMCS.Data.Server.SystemConfiguration;
using AMCS.PlatformFramework.Entity;
using AMCS.PlatformFramework.Server.SystemConfiguration;
using NUnit.Framework;
using SystemConfigurationService = AMCS.Data.Server.SystemConfiguration.SystemConfigurationService;

namespace AMCS.PlatformFramework.IntegrationTests.SystemConfiguration
{
  [TestFixture]
  public class SystemConfigurationFixture : TestBase
  {
    const string ProfileName = "TestProfile";

    [Test]
    public void GenerateSchema()
    {
      Assert.IsNotNull(GetService().XsdSchema);
    }

    private static SystemConfigurationService GetService()
    {
      return new SystemConfigurationService(typeof(Configuration));
    }

    [Test]
    public void WriteContent()
    {
      var configuration = new Configuration
      {
        ProfileName = ProfileName,
        SystemConfigurations =
        {
          Items =
          {
            new PlatformFramework.Server.SystemConfiguration.SystemConfiguration
            {
              Name = "Name1",
              Value = "Value1"
            },
            new PlatformFramework.Server.SystemConfiguration.SystemConfiguration
            {
              Name = "Name2",
              Value = "Value2"
            }
          }
        },
        UserGroups =
        {
          Items =
          {
            new UserGroup
            {
              Name = "UserGroup1"
            }
          }
        }
      };

      using (var writer = new StringWriter())
      {
        new XmlSerializer(typeof(Configuration)).Serialize(writer, configuration);

        Assert.IsNotNull(writer.ToString());
      }
    }

    [Test]
    public void LoadConfiguration()
    {
      Assert.IsNotNull(GetService().LoadConfiguration(AdminUserId));
    }

    [Test]
    public void SaveConfiguration()
    {
      var name1 = Guid.NewGuid().ToString();
      var name2 = Guid.NewGuid().ToString();
      var configuration = new Configuration
      {
        ProfileName = ProfileName,
        SystemConfigurations =
        {
          Items =
          {
            new PlatformFramework.Server.SystemConfiguration.SystemConfiguration
            {
              Name = name1,
              Value = Guid.NewGuid().ToString()
            },
            new PlatformFramework.Server.SystemConfiguration.SystemConfiguration
            {
              Name = name2,
              Value = Guid.NewGuid().ToString()
            }
          }
        },
        UserGroups =
        {
          Items =
          {
            new UserGroup
            {
              Name = Guid.NewGuid().ToString()
            }
          }
        }
      };

      string xml;

      using (var stream = new MemoryStream())
      {
        using (var writer = new StreamWriter(stream))
        {
          new XmlSerializer(typeof(Configuration)).Serialize(writer, configuration);
        }

        xml = Encoding.UTF8.GetString(stream.ToArray());
      }

      using (var dataSession = BslDataSessionFactory.GetDataSession())
      using (var transaction = dataSession.CreateTransaction())
      {
        EnsureSystemConfiguration(name1, dataSession);
        EnsureSystemConfiguration(name2, dataSession);

        transaction.Commit();
      }

      var result = GetService().SaveConfiguration(AdminUserId, xml);
      Assert.AreEqual(typeof(SaveResultSuccess), result.GetType());
    }

    [Test]
    public void SaveConfigurationFailure()
    {
      var configuration = new Configuration
      {
        ProfileName = ProfileName,
        SystemConfigurations =
        {
          Items =
          {
            new PlatformFramework.Server.SystemConfiguration.SystemConfiguration
            {
              Name = "Name1",
              Value = "Value1"
            },
            new PlatformFramework.Server.SystemConfiguration.SystemConfiguration
            {
              Name = "Name2",
              Value = "Value2"
            },
            new PlatformFramework.Server.SystemConfiguration.SystemConfiguration
            {
              Name = "Name3",
              Value = "Value3"
            }
          }
        },
        UserGroups =
        {
          Items =
          {
            new UserGroup
            {
              Name = "UserGroup1"
            }
          }
        }
      };

      string xml;

      using (var stream = new MemoryStream())
      {
        using (var writer = new StreamWriter(stream))
        {
          new XmlSerializer(typeof(Configuration)).Serialize(writer, configuration);
        }

        xml = Encoding.UTF8.GetString(stream.ToArray());
      }

      using (var dataSession = BslDataSessionFactory.GetDataSession())
      using (var transaction = dataSession.CreateTransaction())
      {
        EnsureSystemConfiguration("Name1", dataSession);
        EnsureSystemConfiguration("Name2", dataSession);

        transaction.Commit();
      }

      var result = GetService().SaveConfiguration(AdminUserId, xml);
      Assert.AreEqual(typeof(SaveResultImportFailure), result.GetType());
      Assert.NotNull(((SaveResultImportFailure)result).Xml);
    }

    [Test]
    public void SaveConfigurationFailureOnReplaceSystemConfigurations()
    {
      var configuration = new Configuration
      {
        ProfileName = ProfileName,
        SystemConfigurations =
        {
          Transform = Transform.Replace,
          Items =
          {
            new PlatformFramework.Server.SystemConfiguration.SystemConfiguration
            {
              Name = "Name1",
              Value = "Value1"
            }
          }
        }
      };

      string xml;

      using (var stream = new MemoryStream())
      {
        using (var writer = new StreamWriter(stream))
        {
          new XmlSerializer(typeof(Configuration)).Serialize(writer, configuration);
        }

        xml = Encoding.UTF8.GetString(stream.ToArray());
      }

      Assert.Throws(
        typeof(InvalidOperationException),
        () => GetService().SaveConfiguration(AdminUserId, xml),
        "System configuration parameters cannot be removed; invalid transform mode");
    }

    private void EnsureSystemConfiguration(string name, IDataSession dataSession)
    {
      var entity = dataSession
        .GetAll<SystemConfigurationEntity>(AdminUserId, false)
        .SingleOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));

      if (entity == null)
      {
        dataSession.Save(
          AdminUserId,
          new SystemConfigurationEntity
          {
            Name = name
          }
        );
      }
    }
  }
}
