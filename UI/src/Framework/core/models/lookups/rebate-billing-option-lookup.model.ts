import { alias } from '../../config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class RebateBillingOptionLookup extends ApiBaseModel {

    @alias('RebateBillingOptionId')
    @amcsJsonMember('RebateBillingOptionId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;
}
