namespace AMCS.Data.Server.Configuration.DynamicAppConfig
{
  using System.Collections.Generic;

  public interface IValueCombiner<T>
  {
    T Combine(IList<T> values);
  }

  public static class ValueCombiners<T>
  {
    public static readonly IValueCombiner<T> LeastPrivileged = new LeastPrivilegedCombiner();
    public static readonly IValueCombiner<T> MostPrivileged = new MostPrivilegedCombiner();

    private class LeastPrivilegedCombiner : IValueCombiner<T>
    {
      public T Combine(IList<T> values) => values[0];
    }

    private class MostPrivilegedCombiner : IValueCombiner<T>
    {
      public T Combine(IList<T> values) => values[values.Count - 1];
    }
  }

  public static class ValueCombinersBool
  {
    public static readonly IValueCombiner<bool> And = new AndCombiner();
    public static readonly IValueCombiner<bool> Or = new OrCombiner();

    private class AndCombiner : IValueCombiner<bool>
    {
      public bool Combine(IList<bool> values)
      {
        foreach (bool value in values)
        {
          if (!value)
            return false;
        }

        return true;
      }
    }

    private class OrCombiner : IValueCombiner<bool>
    {
      public bool Combine(IList<bool> values)
      {
        foreach (bool value in values)
        {
          if (value)
            return true;
        }

        return false;
      }
    }
  }
}
