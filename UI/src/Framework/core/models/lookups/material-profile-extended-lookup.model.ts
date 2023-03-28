import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI + ScaleUI
 */
@amcsJsonObject()
export class MaterialProfileExtendedLookup extends ApiBaseModel implements ILookupItem {

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

    @alias('MaterialClass')
    @amcsJsonMember('MaterialClass')
    materialClass: string;

    @alias('ExtendedDescription')
    @amcsJsonMember('ExtendedDescription')
    extendedDescription: string;

    @alias('ApplyProcessCosts')
    @amcsJsonMember('ApplyProcessCosts')
    applyProcessCosts: boolean;

    @alias('IsCompost')
    @amcsJsonMember('IsCompost')
    isCompost: boolean;
}
