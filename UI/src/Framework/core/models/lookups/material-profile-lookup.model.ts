import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsApiUrl, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ISelectableItem } from '../iselectable-item.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI + ScaleUI
 */
@amcsJsonObject()
@amcsApiUrl('settings/MaterialProfiles')
export class MaterialProfileLookup extends ApiBaseModel implements ILookupItem, ISelectableItem {
  @alias('MaterialId')
  @amcsJsonMember('MaterialId')
  id: number;

  @alias('Description')
  @amcsJsonMember('Description')
  description: string;

  @alias('MaterialClassId')
  @amcsJsonMember('MaterialClassId')
  materialClassId: number;

  @alias('EwcCodeId')
  @amcsJsonMember('EwcCodeId')
  ewcCodeId: number;

  @alias('IsPricedMaterial')
  @amcsJsonMember('IsPricedMaterial')
  isPricedMaterial: boolean;

  @alias('GroupName')
  @amcsJsonMember('GroupName')
  groupName: string;

  isSelected: boolean;

  forceDisabled: boolean;
}
