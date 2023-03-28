
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class TimeZoneLookup extends ApiBaseModel {

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonMember('CountryDescription')
    countryDescription: string;

    @amcsJsonMember('CountryId')
    countryId: number;
}
