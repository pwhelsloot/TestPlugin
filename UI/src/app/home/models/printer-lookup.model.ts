import { ILookupItem } from '../../../Framework/core/models/lookups/lookup-item.interface';
import { amcsJsonMember, ApiBaseModel, amcsJsonObject, amcsApiUrl } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
@amcsApiUrl('/Printers')
export class PrinterLookup extends ApiBaseModel implements ILookupItem {
  @amcsJsonMember('IntegrationClientDeviceId')
  id: number;

  @amcsJsonMember('Name')
  description: string;
}
