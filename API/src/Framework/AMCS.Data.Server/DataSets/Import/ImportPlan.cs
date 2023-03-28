using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AMCS.Data.Server.DataSets.Import.Planner;
using AMCS.Data.Support;

namespace AMCS.Data.Server.DataSets.Import
{
  internal class ImportPlan
  {
    public static ImportPlan Build(IList<DataSetTable> tables, MessageCollection messages)
    {
      // The purpose of the import plan is to find a sequence of steps
      // that allow us to import the data in a valid manner. The main reason
      // this is needed is because of foreign keys.
      //
      // There are two ways in which foreign keys complicate the import: foreign
      // keys at all, and circular references. The simple case is normal foreign keys,
      // which just requires us to order the imports. The circular references (either
      // direct e.g. Customer.BillToCustomerId -> Customer.CustomerId or through other
      // tables) requires us to split the import. Because of this, we have two types
      // of imports: creates and updates. In the example above, the create will
      // do a partial import (i.e. all columns except the BillToCustomerId) and
      // the update import will just update the missing field.
      //
      // This only works if we can do the initial import without the field we're
      // delaying. If the field being delayed is a required field, we cannot build
      // an import plan and the import fails.

      return new ImportPlanBuilder(tables, messages).Build();
    }

    public IList<ImportStep> Steps { get; }

    public ImportPlan(IList<ImportStep> steps)
    {
      Steps = steps;
    }

    private class ImportPlanBuilder
    {
      private readonly IList<DataSetTable> tables;
      private readonly MessageCollection messages;
      private readonly List<ImportStep> steps = new List<ImportStep>();

      public ImportPlanBuilder(IList<DataSetTable> tables, MessageCollection messages)
      {
        this.tables = tables;
        this.messages = messages;
      }

      public ImportPlan Build()
      {
        var dataSets = tables.Select(p => p.DataSet).ToList();

        var plan = new PlanBuilder(dataSets).BuildPlan();
        if (plan == null)
        {
          messages.AddError("Failed to build an import plan because of circular foreign key dependencies. Please send this definition to DevOps for analysis.");
          return null;
        }

        // If we succeed, generate an import plan from the resolved plan.
        var delayedProperties = new HashSet<DataSetColumn>(plan.DelayedForeignKeys.Select(p => p.FromColumn));
        var delayedEntities = new HashSet<DataSet>(plan.DelayedForeignKeys.Select(p => p.From));

        foreach (var planDataSet in plan.LoadOrder)
        {
          var table = tables.Single(p => p.DataSet == planDataSet);
          var step = new ImportStep(table, false, !delayedEntities.Contains(planDataSet));
          steps.Add(step);

          step.Columns.AddRange(table.Columns.Where(p => !delayedProperties.Contains(p)));
        }

        foreach (var planDataSet in plan.DelayedForeignKeys
          .Select(p => p.From)
          .Distinct()
          .OrderBy(p => p.Name)
        )
        {
          var table = tables.Single(p => p.DataSet == planDataSet);
          var step = new ImportStep(table, true, true);
          steps.Add(step);

          step.Columns.AddRange(table.Columns.Where(p => delayedProperties.Contains(p)));
        }

        return new ImportPlan(steps);
      }
    }
  }
}
