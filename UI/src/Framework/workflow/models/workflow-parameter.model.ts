import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class WorkflowParameter extends ApiBaseModel {

  @amcsJsonMember('Name')
  name: string;

  @amcsJsonMember('Value')
  value: string;

}
