using Microsoft.Owin;
using Microsoft.Owin.Security.Notifications;
using Moq;

namespace AMCS.ApiService.Tests.Fixtures
{
  internal class RedirectToIdentityProviderNotificationFixture<TMessage, TOptions>
  {
    private TOptions options;

    private TMessage message;

    private IOwinContext context;

    public RedirectToIdentityProviderNotificationFixture<TMessage, TOptions> SetupOptions(TOptions options)
    {
      this.options = options;
      return this;
    }

    public RedirectToIdentityProviderNotificationFixture<TMessage, TOptions> SetupProtocolMessage(TMessage message)
    {
      this.message = message;
      return this;
    }

    public RedirectToIdentityProviderNotificationFixture<TMessage, TOptions> SetupOwinContext(IOwinContext context)
    {
      this.context = context;
      return this;
    }

    public RedirectToIdentityProviderNotification<TMessage, TOptions> Create()
      => new RedirectToIdentityProviderNotification<TMessage, TOptions>(context ?? Mock.Of<IOwinContext>(), options)
      {
        ProtocolMessage = message
      };
  }
}
