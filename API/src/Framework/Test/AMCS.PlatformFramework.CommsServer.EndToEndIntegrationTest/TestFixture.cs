namespace AMCS.PlatformFramework.CommsServer.EndToEndIntegrationTest
{
  using System;
  using AzureServiceBusSupport.RetryUtils;
  using Client;
  using Data;
  using NUnit.Framework;
  using Server.CommsServer;

#if DEBUG
  [Ignore("Tests require azure connection string. This is set in pipeline, but will always fail locally unless manually set.")]
#endif
  public class TestFixture : TestBase
  {
    private static readonly BackoffProfile DefaultBackoffProfile = new BackoffProfile(TimeSpan.FromSeconds(10), 3, TimeSpan.FromMinutes(2));

    [Test]
    public void SendMessageForExistingTenant()
    {
      // existing tenant that has protocol registered on startup
      var tenantId = "4C0FFDF1-CC0A-4C1F-A023-7B2C0354AE98";

      Retryer.Retry(() =>
      {
        var protocol = DataServices.Resolve<IPlatformFrameworkProtocolManager>().FindByTenant(tenantId);
        if (protocol == null)
        {
          throw new Exception($"Failed to get protocol for {tenantId}");
        }

        Assert.IsNotNull(protocol);
      }, DefaultBackoffProfile);

      new Client.Client(new Arguments { Endpoint = "http://localhost:43603" }, TimeSpan.FromMinutes(2)).Run(tenantId);
    }

    [Test]
    public void RegisterTenantAndSendMessage()
    {
      var tenantId = "tenant-X";
      DataServices.Resolve<IPlatformFrameworkProtocolManager>().AddProtocolForTenant(tenantId);

      Retryer.Retry(() =>
      {
        var protocol = DataServices.Resolve<IPlatformFrameworkProtocolManager>().FindByTenant(tenantId);
        if (protocol == null)
        {
          throw new Exception($"Failed to get protocol for {tenantId}");
        }

        Assert.IsNotNull(protocol);
      }, DefaultBackoffProfile);

      new Client.Client(new Arguments { Endpoint = "http://localhost:43603" }, TimeSpan.FromMinutes(2)).Run(tenantId);
    }

    [Test]
    public void RegisterTenantRemoveTenantAndSendMessage()
    {
      var tenantId = "tenant-Y";
      DataServices.Resolve<IPlatformFrameworkProtocolManager>().AddProtocolForTenant(tenantId);

      Retryer.Retry(() =>
      {
        var protocol = DataServices.Resolve<IPlatformFrameworkProtocolManager>().FindByTenant(tenantId);
        if (protocol == null)
        {
          throw new Exception($"Failed to get protocol for {tenantId}");
        }

        Assert.IsNotNull(protocol);
      }, DefaultBackoffProfile);

      new Client.Client(new Arguments { Endpoint = "http://localhost:43603" }, TimeSpan.FromMinutes(2)).Run(tenantId);

      DataServices.Resolve<IPlatformFrameworkProtocolManager>().RemoveProtocolForTenant(tenantId);

      Retryer.Retry(() =>
      {
        var protocol = DataServices.Resolve<IPlatformFrameworkProtocolManager>().FindByTenant(tenantId);
        if (protocol != null)
        {
          throw new Exception($"Failed to get protocol for {tenantId}");
        }

        Assert.IsNull(protocol);
      }, DefaultBackoffProfile);

      Assert.Throws<TimeoutException>(() =>
      {
        new Client.Client(new Arguments { Endpoint = "http://localhost:43603" }, TimeSpan.FromMinutes(2)).Run(tenantId);
      });
    }

    [Test]
    public void ConfigureMultipleTenants()
    {
      var tenantA = "tenant-A";
      var tenantB = "tenant-B";

      DataServices.Resolve<IPlatformFrameworkProtocolManager>().AddProtocolForTenant(tenantA);
      DataServices.Resolve<IPlatformFrameworkProtocolManager>().AddProtocolForTenant(tenantB);

      Retryer.Retry(() =>
      {
        var protocol = DataServices.Resolve<IPlatformFrameworkProtocolManager>().FindByTenant(tenantA);
        if (protocol == null)
        {
          throw new Exception($"Failed to get protocol for {tenantA}");
        }

        Assert.IsNotNull(protocol);
      }, DefaultBackoffProfile);

      Retryer.Retry(() =>
      {
        var protocol = DataServices.Resolve<IPlatformFrameworkProtocolManager>().FindByTenant(tenantB);
        if (protocol == null)
        {
          throw new Exception($"Failed to get protocol for {tenantB}");
        }

        Assert.IsNotNull(protocol);
      }, DefaultBackoffProfile);

      new Client.Client(new Arguments { Endpoint = "http://localhost:43603" }, TimeSpan.FromMinutes(2)).Run(tenantA);
    }
  }
}