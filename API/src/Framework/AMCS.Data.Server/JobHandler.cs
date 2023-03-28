namespace AMCS.Data.Server
{
  using System.Globalization;
  using AMCS.JobSystem;
  using Newtonsoft.Json;
  using Services;
  using System;
  using log4net;

  public static class JobHandler
  {
    private static readonly ILog Logger = LogManager.GetLogger(typeof(JobHandler));

    private const string SystemUserNamePrefix = "(SYSTEM:";
    private const string SystemUserName = "(SYSTEM)";

    internal static ISessionToken ParseJobUserId(string userId)
    {
      var userService = DataServices.Resolve<IUserService>();

      if (userId.StartsWith(SystemUserNamePrefix))
      {
        var tenantId = userId.Substring(SystemUserNamePrefix.Length, userId.Length - (SystemUserNamePrefix.Length + 1));
        return userService.CreateSystemSessionToken(tenantId);
      }

      if (userId == SystemUserName)
        return userService.SystemUserSessionKey;

      IAuthenticationResult authenticationResult;

      if (TryDeserializeJobHandlerPlatformCredentials(userId, out var jobHandlerPlatformCredentials))
      {
        CultureInfo.CurrentUICulture = CultureInfo.CreateSpecificCulture(jobHandlerPlatformCredentials.Language);
        var userName = jobHandlerPlatformCredentials.UserName;
        var tenantId = jobHandlerPlatformCredentials.TenantId;

        if (userName == SystemUserName)
          return userService.CreateSystemSessionToken(tenantId);

        if (userName.StartsWith("app:"))
        {
          authenticationResult = userService.Authenticate(new AppCredentials(userName, tenantId));
        }
        else
        {
          authenticationResult = userService.Authenticate(new UserNameCredentials(userName, tenantId));
        }
      }
      else
      {
        authenticationResult = userService.Authenticate(new UserNameCredentials(userId));
      }

      switch (authenticationResult)
      {
        case AuthenticationUser user:
          return userService.CreateSessionToken(user);
        case AuthenticationFailure failure:
          string status;

          switch (failure.Status)
          {
            case AuthenticationStatus.InvalidCredentials:
              status = "invalidCredentials";
              break;

            case AuthenticationStatus.Locked:
              status = "accountLocked";
              break;

            case AuthenticationStatus.DuplicateEmail:
              status = "duplicateEmail";
              break;

            default:
              status = "unknown";
              break;
          }

          Logger.Error($"Authentication failed {status}. UserID used {userId}.");
          throw new UnauthorizedAccessException($"Authentication failed {status}. UserID used {userId}.");

        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public static string GetJobUserId(ISessionToken userId)
    {
      var obj = new JobHandlerPlatformCredentials
      {
        UserName = GenerateUserName(userId),
        TenantId = userId.TenantId,
        Language = userId.Language?.Name ?? CultureInfo.CurrentUICulture.Name,
      };

      string GenerateUserName(ISessionToken token)
      {
        if (DataServices.Resolve<IUserService>().IsSystemSessionToken(userId) || string.IsNullOrEmpty(token.UserName))
          return SystemUserName;

        return token.UserName;
      }

      return JsonConvert.SerializeObject(obj);
    }
    
    private static bool TryDeserializeJobHandlerPlatformCredentials(string value, out JobHandlerPlatformCredentials jobHandlerPlatformCredentials)
    {
      try
      {
        jobHandlerPlatformCredentials = JsonConvert.DeserializeObject<JobHandlerPlatformCredentials>(value);
        return true;
      }
      catch
      {
        jobHandlerPlatformCredentials = null;
        return false;
      }
    }
  }

  public abstract class JobHandler<TRequest> : IJobHandler<TRequest>
  {
    public void Execute(IJobContext context, TRequest request)
    {
      Execute(context, JobHandler.ParseJobUserId(context.UserId), request);
    }

    protected abstract void Execute(IJobContext context, ISessionToken userId, TRequest request);
  }

  public abstract class JobHandler<TRequest, TResponse> : IJobHandler<TRequest, TResponse>
  {
    public TResponse Execute(IJobContext context, TRequest request)
    {
      return Execute(context, JobHandler.ParseJobUserId(context.UserId), request);
    }

    protected abstract TResponse Execute(IJobContext context, ISessionToken userId, TRequest request);
  }
}
