import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class SalesTerritoryLookup extends ApiBaseModel implements ILookupItem {

    @alias('SalesTerritoryId')
    @amcsJsonMember('SalesTerritoryId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;
}
