import { alias } from '@core-module/config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
export class BrregAddressSearchResult {
    @alias('SicCode')
    sicCode: string;

    @alias('FormattedAddress')
    formattedAddress: string;

    @alias('MunicipalityNo')
    municipalityNo: string;

    @alias('SiteName')
    siteName: string;

    @alias('HouseNumber')
    houseNumber: string;

    @alias('Address1')
    address1: string;

    @alias('Address2')
    address2: string;

    @alias('Address3')
    address3: string;

    @alias('Address4')
    address4: string;

    @alias('Address5')
    address5: string;

    @alias('Postcode')
    postcode: string;

    @alias('Latitude')
    latitude: number;

    @alias('Longitude')
    longitude: number;

    @alias('Href')
    href: string;

    @alias('CountryId')
    countryId: number;
}
