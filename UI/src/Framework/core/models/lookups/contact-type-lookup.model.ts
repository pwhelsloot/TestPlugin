import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ContactTypeLookup extends ApiBaseModel implements ILookupItem {
  @alias('ContactTypeId')
  @amcsJsonMember('ContactTypeId')
  id: number;

  @alias('Description')
  @amcsJsonMember('Description')
  description: string;

  @alias('GUID')
  @amcsJsonMember('GUID')
  guid?: string;
}
