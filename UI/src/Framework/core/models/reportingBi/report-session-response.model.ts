import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/245825
 */
@amcsJsonObject()
export class ReportSessionResponse extends ApiBaseModel {

  @amcsJsonMember('Url')
  url: string;
}
