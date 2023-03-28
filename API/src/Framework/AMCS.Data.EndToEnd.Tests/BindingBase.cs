using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data;
using AMCS.Data.Server;
using AMCS.Data.Support.Security;
using Bogus;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace AMCS.Data.EndToEnd.Tests
{
  public abstract class BindingBase
  {
    public static readonly Faker Faker = new Faker();

    private const string TestCasePrefix = "tc:";

    protected ScenarioContext Context { get; }

    protected string Tag
    {
      get => Context.Get<string>("Tag");
      private set => Context.Set<string>(value, "Tag");
    }

    protected IDataSession Session
    {
      get => Context.Get<IDataSession>();
      private set => Context.Set<IDataSession>(value);
    }

    protected ISessionToken AdminUserId
    {
      get => Context.Get<ISessionToken>();
      private set => Context.Set<ISessionToken>(value);
    }

    protected string AdminPrivateKey
    {
      get => Context.Get<string>("AdminPrivateKey");
      private set => Context.Set<string>(value, "AdminPrivateKey");
    }

    protected string ErrorMessage
    {
      get
      {
        Context.TryGetValue<string>("ErrorMessage", out string errorMessage);
        return errorMessage;
      }
      private set => Context.Set<string>(value, "ErrorMessage");
    }

    public BindingBase(ScenarioContext context)
    {
      Context = context;
    }

    protected void StartSession(ISessionToken adminUserId, string adminPrivateKey)
    {
      Session = BslDataSessionFactory.GetDataSession(adminUserId);
      AdminUserId = adminUserId;
      AdminPrivateKey = adminPrivateKey;
    }

    protected void SetTag()
    {
      if (!Context.ScenarioInfo.Tags[0].StartsWith(TestCasePrefix))
      {
        throw new InvalidOperationException($"Scenario must have a {TestCasePrefix}<testcase number> as first tag");
      }
      Tag = " " + Context.ScenarioInfo.Tags[0] + "-" + SharedRandom.GetRandom().Next(100_000, 999_999);
    }

    protected void DisposeSession()
    {
      Session.Dispose();
    }

    protected void CaptureExeption(Action action)
    {
      Assert.IsNull(ErrorMessage, "CaptureExeption called while errorMessage is set");
      try
      {
        action();
      }
      catch (Exception ex)
      {
        ErrorMessage = ex.Message;
      }
    }

    protected void AssertErrorMessage(string errorMessage)
    {
      Assert.IsNotNull(ErrorMessage);
      Assert.IsTrue(ErrorMessage.Contains(errorMessage));
      ErrorMessage = null;
    }

    protected void AssertHasErrorMessage()
    {
      Assert.IsNotNull(ErrorMessage);
      ErrorMessage = null;
    }

    protected void AssertHasNoErrorMessage()
    {
      Assert.IsNull(ErrorMessage);
    }

    protected void RegisterCleanup(Action action)
    {
      if (!Context.TryGetValue<List<Action>>("CleanupActions", out var actions))
      {
        actions = new List<Action>();
        Context.Add("CleanupActions", actions);
      }

      actions.Add(action);
    }
  }
}
