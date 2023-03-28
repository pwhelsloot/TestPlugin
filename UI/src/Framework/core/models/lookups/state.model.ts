import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class State extends ApiBaseModel {
    @alias('StateId')
    @amcsJsonMember('StateId')
    stateId: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('CountryId')
    @amcsJsonMember('CountryId')
    countryId: number;
}
