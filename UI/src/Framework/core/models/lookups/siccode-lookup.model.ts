import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class SicCodeLookup extends ApiBaseModel implements ILookupItem {

    @alias('SICCodeId')
    @amcsJsonMember('SICCodeId')
    id: number;

    @alias('Code2007')
    @amcsJsonMember('Code2007')
    code2007: string;

    @alias('Description2007')
    @amcsJsonMember('Description2007')
    description2007: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;

    description: string;
}
