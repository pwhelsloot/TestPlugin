using AMCS.Data.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Entity;

namespace AMCS.Data.Server.SQL.Fetch
{
  internal static class FetchInfoCreator
  {
    public static FetchInfo Create(Type entityType, IList<string> fetchPaths)
    {
      var mainAccessor = EntityObjectAccessor.ForType(entityType);
      var fetchChains = CreateFetchChains(mainAccessor, fetchPaths);
      var fetchChainGroups = GroupFetchChains(fetchChains);
      var fetchJoins = CreateFetchJoins(mainAccessor, fetchChainGroups);
      return new FetchInfo(fetchJoins, fetchChainGroups.Count);
    }

    private static List<FetchChain> CreateFetchChains(EntityObjectAccessor mainAccessor, IList<string> fetchPaths)
    {
      List<FetchChain> sortedFetchChains = fetchPaths
        .Select(fetchPath => new FetchChain(mainAccessor, fetchPath))
        .OrderBy(fetchChain => fetchChain)
        .RemoveRedundant()
        .ToList();

      return sortedFetchChains;
    }

    private static IEnumerable<FetchChain> RemoveRedundant(this IEnumerable<FetchChain> sortedChains)
    {
      FetchChain prevChain = null;
      foreach (FetchChain fetchChain in sortedChains)
      {
        if (prevChain != null && fetchChain.IsSubChainOf(prevChain))
          continue;
        yield return fetchChain;
        prevChain = fetchChain;
      }
    }

    private static List<List<FetchChain>> GroupFetchChains(List<FetchChain> sortedFetchChains)
    {
      // Put into groups
      var fetchChainGroupsWithDuplicates = new List<List<FetchChain>>();
      foreach (FetchChain fetchChain in sortedFetchChains)
      {
        bool foundGroup = false;
        foreach (List<FetchChain> group in fetchChainGroupsWithDuplicates)
        {
          if (fetchChain.IsInSameGroupAs(group.First()))
          {
            group.Add(fetchChain);
            foundGroup = true;
          }
        }

        if (!foundGroup)
        {
          fetchChainGroupsWithDuplicates.Add(new List<FetchChain> { fetchChain });
        }
      }

      if (fetchChainGroupsWithDuplicates.Count == 1)
        return fetchChainGroupsWithDuplicates;

      // Sort longest groups first
      fetchChainGroupsWithDuplicates = fetchChainGroupsWithDuplicates
        .OrderByDescending(group => group.Count())
        .ToList();

      // Remove duplicates
      var includedChains = new HashSet<FetchChain>();
      var fetchChainGroups = new List<List<FetchChain>>();
      foreach (List<FetchChain> group in fetchChainGroupsWithDuplicates)
      {
        List<FetchChain> cleanedGroup = group
          .Where(chain => !includedChains.Contains(chain))
          .ToList();

        includedChains.AddRange(cleanedGroup);
        fetchChainGroups.Add(cleanedGroup);
      }
      return fetchChainGroups;
    }

    private static List<FetchJoin> CreateFetchJoins(EntityObjectAccessor mainAccessor, List<List<FetchChain>> fetchChainGroups)
    {
      var joins = new List<FetchJoin>();
      joins.Add(new FetchJoin(mainAccessor, fetchChainGroups.Count));

      var joinIndexes = new Dictionary<string, int>();
      joinIndexes.Add(mainAccessor.TableName, 0);

      for (int groupIndex = 0; groupIndex < fetchChainGroups.Count; ++groupIndex)
      {
        var fetchGroup = fetchChainGroups[groupIndex];
        int prevGroupJoinCount = (groupIndex == 0) ? 0 : joins.Count;

        foreach (var fetchChain in fetchGroup)
        {
          string joinKey = mainAccessor.TableName;

          foreach (var reference in fetchChain.References)
          {
            int joinParentIndex = joinIndexes[joinKey];
            joinKey += $".{reference.PropertyName}";
            int index;
            if (!joinIndexes.TryGetValue(joinKey, out index))
            {
              index = joins.Count;
              joins.Add(new FetchJoin(index, joinParentIndex, reference));
              joinIndexes.Add(joinKey, index);
            }
            bool parentWasInPrevGroup = (joinParentIndex < prevGroupJoinCount);
            joins[index].SetJoinStateForGroup(groupIndex, parentWasInPrevGroup ? GroupJoinState.InnerJoin : GroupJoinState.LeftJoin);
          };
        }
      }

      return joins;
    }
  }
}
