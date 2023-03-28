import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { alias } from '@coreconfig/api-dto-alias.function';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DirectDebitRunConfigurationLookup extends ApiBaseModel implements ILookupItem {

    @amcsJsonMember('DirectDebitRunConfigurationId')
    @alias('DirectDebitRunConfigurationId')
    id: number;

    @amcsJsonMember('Description')
    @alias('Description')
    description: string;

    @amcsJsonMember('MinimumChequeAmount')
    @alias('MinimumChequeAmount')
    minimumChequeAmount: number;
}
