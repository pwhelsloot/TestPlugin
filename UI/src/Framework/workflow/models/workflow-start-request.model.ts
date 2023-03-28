import { amcsApiUrl, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
@amcsApiUrl('workflow/start')
export class WorkflowStartRequest extends ApiBaseModel {

  @amcsJsonMember('SchemeName')
  schemeName: string;

}
