
import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class AddressValidationSearchResult {

    @alias('FormattedAddress')
    formattedAddress: string;

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

    @alias('Address6')
    address6: string;

    @alias('Address7')
    address7: string;

    @alias('Address8')
    address8: string;

    @alias('Address9')
    address9: string;

    @alias('Postcode')
    postcode: string;

    @alias('Latitude')
    latitude: string;

    @alias('Longitude')
    longitude: string;

    @alias('SiteName')
    siteName: string;

    @alias('FederalId')
    federalId: string;

    // some providers will supply an href on search results rather than populating all address fields.
    @alias('Href')
    href: string;

    municipalityNo: string;

    sicCode: string;
}
