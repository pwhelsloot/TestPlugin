namespace AMCS.Data
{
  using System;

  [Flags]
  public enum StrictModeType
  {
    None = 0,
    RequireTransaction = 1 << 0,
    DisableNeutralTimeZone = 1 << 1,

    Minimal = RequireTransaction,
    All = -1
  }
}