import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI + ScaleUI ( https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268) + TDM
 */
@amcsJsonObject()
export class Country extends ApiBaseModel {

    @alias('CountryId')
    @amcsJsonMember('CountryId')
    countryId: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('Latitude')
    @amcsJsonMember('Latitude')
    latitude: string;

    @alias('Longitude')
    @amcsJsonMember('Longitude')
    longitude: string;

    @alias('IsDefault')
    @amcsJsonMember('IsDefault')
    isDefault: boolean;

    @alias('ISOTwoChar')
    @amcsJsonMember('ISOTwoChar')
    iSOTwoChar: string;

    id: number;
}
