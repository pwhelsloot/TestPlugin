import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class WorkflowStartResponse extends ApiBaseModel {

  @amcsJsonMember('InstanceGuid')
  instanceGuid: string;

  @amcsJsonMember('CurrentStep')
  currentStep: string;

  @amcsJsonArrayMember('Commands', String)
  commands: string[];

  @amcsJsonMember('ViewData')
  viewData: string;

}
