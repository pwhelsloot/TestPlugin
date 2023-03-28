import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { WorkflowParameter } from './workflow-parameter.model';

@amcsJsonObject()
export class WorkflowResponse extends ApiBaseModel {

  @amcsJsonMember('InstanceGuid')
  instanceGuid: string;

  @amcsJsonMember('CurrentStep')
  currentStep: string;

  @amcsJsonArrayMember('Parameters', WorkflowParameter)
  parameters: WorkflowParameter[];

  @amcsJsonMember('ViewData')
  viewData: string;

}
