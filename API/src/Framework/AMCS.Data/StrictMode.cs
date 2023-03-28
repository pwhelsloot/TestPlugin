namespace AMCS.Data
{
  using System.Diagnostics;

  internal static class StrictMode
  {
    private static StrictModeType? strictMode;

    internal static void SetStrictMode(StrictModeType strictModeType)
    {
      Debug.Assert(!strictMode.HasValue);
      strictMode = strictModeType;
    }

    public static bool IsRequireTransaction => strictMode.Value.HasFlag(StrictModeType.RequireTransaction);
    public static bool IsDisableNeutralTimeZone => strictMode.Value.HasFlag(StrictModeType.DisableNeutralTimeZone);
  }
}
