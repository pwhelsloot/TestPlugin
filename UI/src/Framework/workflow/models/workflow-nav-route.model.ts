import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class WorkflowNavRoute extends ApiBaseModel {

  @amcsJsonMember('Name')
  name: string;

  @amcsJsonMember('Url')
  url: string;

}
