import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class BusinessSectorLookup extends ApiBaseModel implements ILookupItem {

  @alias('BusinessSectorId')
  @amcsJsonMember('BusinessSectorId')
  id: number;

  @alias('Description')
  @amcsJsonMember('Description')
  description: string;

  @alias('MarketId')
  @amcsJsonMember('MarketId')
  marketId: number;

  @alias('GUID')
  @amcsJsonMember('GUID')
  guid: string;

}
