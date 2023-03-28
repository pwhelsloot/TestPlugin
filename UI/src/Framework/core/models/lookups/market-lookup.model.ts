import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated To be deleted
 */
@amcsJsonObject()
export class MarketLookup extends ApiBaseModel implements ILookupItem {

  @alias('MarketId')
  @amcsJsonMember('MarketId')
  id: number;

  @alias('Description')
  @amcsJsonMember('Description')
  description: string;

}
