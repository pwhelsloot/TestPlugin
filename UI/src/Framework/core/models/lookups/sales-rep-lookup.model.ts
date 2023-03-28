import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class SalesRepLookup extends ApiBaseModel implements ILookupItem {

    @alias('SysUserId')
    @amcsJsonMember('SysUserId')
    id: number;

    @alias('ForenameSurname')
    @amcsJsonMember('ForenameSurname')
    description: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;
}
