import { ApiBaseModel } from '@core-module/models/api/api-base.model';
import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class CustomerStateLookup extends ApiBaseModel implements ILookupItem {
    @alias('CustomerStateId')
    @amcsJsonMember('CustomerStateId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;
}
