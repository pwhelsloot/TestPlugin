using System;
using System.Collections.Generic;
using System.Linq;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.SQL.Fetch
{
  public class FetchChain : IComparable<FetchChain>
  {
    private readonly IReadOnlyList<IEntityObjectReference> references;
    private readonly string fetchPath;
    private readonly string mergePath;

    internal IReadOnlyList<IEntityObjectReference> References => references;

    public FetchChain(EntityObjectAccessor mainAccessor, string fetchPath)
    {
      this.references = FetchPathToReferences(mainAccessor, fetchPath)
        .ToList()
        .AsReadOnly();
      // Rebuild the fetch path in case there are some white-space or case differences (not sure if there can be)
      this.fetchPath = string.Join(".", references.Select(reference => reference.PropertyName));
      this.mergePath = BuildMergePath(this.references);
    }

    public bool IsSubChainOf(FetchChain other) => IsSubPathOf(this.fetchPath, other.fetchPath);
    public bool IsInSameGroupAs(FetchChain other) => IsSubPathOf(this.mergePath, other.mergePath);

    private static bool IsSubPathOf(string subPath, string mainPath)
    {
      if (subPath.Length == 0)
        return true;
      if (!mainPath.StartsWith(subPath))
        return false;
      if (subPath.Length < mainPath.Length && mainPath[subPath.Length] != '.')
        return false;
      return true;
    }

    private static IEnumerable<IEntityObjectReference> FetchPathToReferences(EntityObjectAccessor mainAccessor, string fetchPath)
    {
      EntityObjectAccessor accessor = mainAccessor;
      foreach (string propertyName in fetchPath.Split('.'))
      {
        IEntityObjectReference reference = accessor.GetReference(propertyName);
        if (reference == null)
          throw new ArgumentException($"'{propertyName}' (from the Fetch '{fetchPath}') is not a reference property name.");
        yield return reference;
        accessor = reference.TargetAccessor;
      }
    }

    private static string BuildMergePath(IReadOnlyList<IEntityObjectReference> references)
    {
      int lastExplosionIndex = GetLastExplosionIndex(references);
      if (lastExplosionIndex < 0)
        return "";
      return string.Join(".", references.Take(lastExplosionIndex + 1).Select(reference => reference.PropertyName));
    }

    private static int GetLastExplosionIndex(IReadOnlyList<IEntityObjectReference> references)
    {
      for (int i = references.Count - 1; i >= 0; i--)
      {
        if (references[i].IsCartesianExplosionRisk)
          return i;
      }
      return -1;
    }

    public int CompareTo(FetchChain other)
    {
      // First compare the merge path descending (so we get the larger path first)
      int mergeDiff = this.mergePath.CompareTo(other.mergePath);
      if (mergeDiff != 0)
        return -mergeDiff;

      // Next compare the fetch path (descending to keep this consistent)
      return -this.fetchPath.CompareTo(other.fetchPath); 
    }

    public override string ToString()
    {
      return $"({mergePath}){fetchPath.Substring(mergePath.Length)}";
    }
  }
}
