import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ZoneLookup extends ApiBaseModel implements ILookupItem {
  @alias('CustomerTemplateId')
  @amcsJsonMember('CustomerTemplateId')
  customerTemplateId: number;

  @alias('ZoneId')
  @amcsJsonMember('ZoneId')
  id: number;

  @alias('Description')
  @amcsJsonMember('Description')
  description: string;
}
