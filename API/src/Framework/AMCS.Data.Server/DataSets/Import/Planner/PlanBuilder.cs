using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMCS.Data.Server.DataSets.Import.Planner
{
  internal class PlanBuilder
  {
    private readonly HashSet<DataSet> dataSets;
    private readonly List<DataSetRelationship> relationships;
    private Plan candidate;

    public PlanBuilder(IList<DataSet> dataSets)
    {
      this.dataSets = new HashSet<DataSet>(dataSets);

      relationships = dataSets
        .SelectMany(p => p.Relationships.Where(p1 => !p1.FromColumn.IsMandatory))
        .ToList();
    }

    public Plan BuildPlan()
    {
      var fullPlan = AttemptPlan(new HashSet<DataSetRelationship>(relationships));
      if (fullPlan == null)
        return null;

      candidate = fullPlan;

      Reduce(new HashSet<DataSetRelationship>(fullPlan.DelayedForeignKeys));

      return candidate;
    }

    private void Reduce(HashSet<DataSetRelationship> disabled)
    {
      var attemptDisabled = new HashSet<DataSetRelationship>(disabled);

      foreach (var foreignKey in disabled)
      {
        attemptDisabled.Remove(foreignKey);

        if (attemptDisabled.Count < candidate.DelayedForeignKeys.Count)
        {
          var attempt = AttemptPlan(attemptDisabled);
          if (attempt != null)
          {
            candidate = attempt;

            Reduce(attemptDisabled);
          }
        }

        attemptDisabled.Add(foreignKey);
      }
    }

    private Plan AttemptPlan(HashSet<DataSetRelationship> disabled)
    {
      return new PlanAttempt(this, disabled).BuildPlan();
    }

    private class PlanAttempt
    {
      private readonly PlanBuilder builder;
      private readonly HashSet<DataSetRelationship> disabled;
      private readonly HashSet<DataSet> seen = new HashSet<DataSet>();
      private readonly HashSet<DataSet> done = new HashSet<DataSet>();
      private readonly List<DataSet> steps = new List<DataSet>();

      public PlanAttempt(PlanBuilder builder, HashSet<DataSetRelationship> disabled)
      {
        this.builder = builder;
        this.disabled = disabled;
      }

      public Plan BuildPlan()
      {
        foreach (var dataSet in builder.dataSets.OrderBy(p => p.Name))
        {
          if (AddTable(dataSet) == Status.CircularDependency)
            return null;
        }

        var plan = new Plan();
        plan.LoadOrder.AddRange(steps);
        plan.DelayedForeignKeys.AddRange(disabled);
        return plan;
      }

      private Status AddTable(DataSet dataSet)
      {
        if (!seen.Add(dataSet))
          return Status.AlreadyAdded;

        // Process all outgoing foreign keys.

        foreach (var relationship in dataSet.Relationships)
        {
          // Is the target in our set?

          if (!builder.dataSets.Contains(relationship.To))
            continue;

          // If this foreign key is disabled, skip it.

          if (disabled.Contains(relationship))
            continue;

          // Don't attempt foreign keys that reference itself. The import table
          // reorderer already made an attempt at ordering the records
          // internally as to limit the risk of import errors. We don't
          // do anything here.

          if (dataSet == relationship.To)
            continue;

          // If we can't add the dependency, fail building the plan.

          if (!AddDependency(relationship.To))
            return Status.CircularDependency;
        }

        // Add a create step for this table.

        steps.Add(dataSet);
        done.Add(dataSet);

        return Status.Added;
      }

      private bool AddDependency(DataSet dataSet)
      {
        // If we already have this table, we're done.

        if (done.Contains(dataSet))
          return true;

        // Try adding the table. This returns false if we've already seen it,
        // which means we have a circular dependency.

        return AddTable(dataSet) == Status.Added;
      }

      private enum Status
      {
        AlreadyAdded,
        Added,
        CircularDependency
      }
    }
  }
}
