using System;
using System.Collections.Generic;

namespace AMCS.Data.Entity.Workflow
{
  [EntityTable("NA", "NA")]
  [ApiExplorer(Mode = ApiMode.External)]
  public class ApiWorkflowInstanceState : ApiWorkflowInstance
  {
    [EntityMember]
    public string Key { get; set; }

    [EntityMember]
    public string Value { get; set; }

    [EntityMember]
    public string DataType { get; set; }
  }
}