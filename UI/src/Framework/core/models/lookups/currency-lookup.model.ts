import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';
import { alias } from '../../config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class CurrencyLookup extends ApiBaseModel implements ILookupItem {
    @alias('CurrencyId')
    @amcsJsonMember('CurrencyId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;
}
