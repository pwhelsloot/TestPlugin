namespace AMCS.Data.Server.WebHook.BslTrigger
{
  using System;
  using AMCS.Data.Entity;

  internal class BslTriggerActionDto
  {
    public bool Create { get; set; }

    public bool Update { get; set; }

    public bool Delete { get; set; }

    public bool BeforeCreate { get; set; }

    public bool BeforeUpdate { get; set; }

    public bool BeforeDelete { get; set; }

    public bool NoActionsAvailable => !Create && !Update && !Delete && !BeforeCreate && !BeforeUpdate && !BeforeDelete;

    public bool SupportsAction(BslAction action)
    {
      switch (action)
      {
        case BslAction.Create:
          return Create;
        case BslAction.Update:
          return Update;
        case BslAction.Delete:
          return Delete;
        case BslAction.BeforeCreate:
          return BeforeCreate;
        case BslAction.BeforeDelete:
          return BeforeDelete;
        case BslAction.BeforeUpdate:
          return BeforeUpdate;
        default:
          throw new ArgumentOutOfRangeException(nameof(action));
      }
    }
  }
}