import { amcsApiUrl, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { WorkflowStepInput } from './workflow-step-input.model';

@amcsJsonObject()
@amcsApiUrl('workflow/run')
export class WorkflowRunRequest extends ApiBaseModel {

  @amcsJsonMember('InstanceGuid')
  instanceGuid: string;

  @amcsJsonMember('StepInput')
  stepInput: WorkflowStepInput;

}
