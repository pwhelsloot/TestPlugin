import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { alias } from '@coreconfig/api-dto-alias.function';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DirectDebitAccountTypeLookup extends ApiBaseModel implements ILookupItem {

    @amcsJsonMember('DirectDebitAccountTypeId')
    @alias('DirectDebitAccountTypeId')
    id: number;

    @amcsJsonMember('Description')
    @alias('Description')
    description: string;
}
