import { alias } from '@coreconfig/api-dto-alias.function';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI + IMMUI
 */
@amcsJsonObject()
export class MaterialClassLookup extends ApiBaseModel implements ILookupItem {

  @alias('MaterialClassId')
  @amcsJsonMember('MaterialClassId')
  id: number;

  @alias('MaterialClassGroupId')
  @amcsJsonMember('MaterialClassGroupId')
  materialClassGroupId: number;

  @alias('Description')
  @amcsJsonMember('Description')
  description: string;

  @alias('IsPricedMaterial')
  @amcsJsonMember('IsPricedMaterial')
  isPricedMaterial: boolean;

  @alias('GroupName')
  @amcsJsonMember('GroupName')
  groupName: string;

  @alias('AnalysisCodePlusDescription')
  @amcsJsonMember('AnalysisCodePlusDescription')
  analysisCodePlusDescription: string;
}
