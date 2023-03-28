import { alias } from '@coreconfig/api-dto-alias.function';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to IMMUI
 */
@amcsJsonObject()
export class MaterialClassGroupLookup extends ApiBaseModel implements ILookupItem {

  @alias('MaterialClassGroupId')
  @amcsJsonMember('MaterialClassGroupId')
  id: number;

  @alias('Description')
  @amcsJsonMember('Description')
  description: string;
}
