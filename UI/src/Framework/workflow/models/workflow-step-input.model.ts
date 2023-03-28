import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { WorkflowParameter } from './workflow-parameter.model';

@amcsJsonObject()
export class WorkflowStepInput extends ApiBaseModel {

  @amcsJsonMember('StepName')
  stepName: string;

  @amcsJsonMember('SelectedCommand')
  selectedCommand: string;

  @amcsJsonArrayMember('Parameters', WorkflowParameter)
  parameters: WorkflowParameter[];

}
