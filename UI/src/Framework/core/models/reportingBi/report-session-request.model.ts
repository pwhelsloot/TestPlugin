import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/245825
 */
@amcsJsonObject()
export class ReportSessionRequest extends ApiBaseModel {

  @amcsJsonMember('Config')
  config: string;

  @amcsJsonMember('ReportPath')
  reportPath: string;

  @amcsJsonMember('ReportFilter')
  reportFilter: string;

  @amcsJsonMember('Action')
  action: number;

  @amcsJsonMember('ReportId')
  reportId: number;

  @amcsJsonMember('Description')
  description: string;

  @amcsJsonMember('Keywords')
  keywords: string;
}
