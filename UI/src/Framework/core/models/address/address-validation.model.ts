import { amcsJsonArrayMember, amcsJsonMember, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { AddressFormatter } from '@core-module/models/external-dependencies/address-formatter';
import { postAlias } from '@coreconfig/alias-to-api.function';
import { alias } from '@coreconfig/api-dto-alias.function';
import { IPropertiesMetadata } from '@shared-module/models/properties-metadata.interface';
import { PropertyMetadata } from '@shared-module/models/property-metadata.model';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
export abstract class AddressValidation extends ApiBaseModel implements IPropertiesMetadata {

    @alias('HouseNumber')
    @postAlias('HouseNumber')
    @amcsJsonMember('HouseNumber')
    houseNumber: string;

    @alias('Address1')
    @postAlias('Address1')
    @amcsJsonMember('Address1')
    address1: string;

    @alias('Address2')
    @postAlias('Address2')
    @amcsJsonMember('Address2')
    address2: string;

    @alias('Address3')
    @postAlias('Address3')
    @amcsJsonMember('Address3')
    address3: string;

    @alias('Address4')
    @postAlias('Address4')
    @amcsJsonMember('Address4')
    address4: string;

    @alias('Address5')
    @postAlias('Address5')
    @amcsJsonMember('Address5')
    address5: string;

    @alias('Address6')
    @postAlias('Address6')
    @amcsJsonMember('Address6')
    address6: string;

    @alias('Address7')
    @postAlias('Address7')
    @amcsJsonMember('Address7')
    address7: string;

    @alias('Address8')
    @postAlias('Address8')
    @amcsJsonMember('Address8')
    address8: string;

    @amcsJsonMember('Address9')
    @alias('Address9')
    @postAlias('Address9')
    address9: string;

    @alias('Postcode')
    @postAlias('Postcode')
    @amcsJsonMember('Postcode')
    postcode: string;

    @alias('Latitude')
    @postAlias('Latitude')
    @amcsJsonMember('Latitude')
    latitude: string;

    @alias('Longitude')
    @postAlias('Longitude')
    @amcsJsonMember('Longitude')
    longitude: string;

    federalId?: string;

    @alias('PropertiesMetadata')
    @amcsJsonArrayMember('PropertiesMetadata', PropertyMetadata)
    propertiesMetadata: PropertyMetadata[];

    fullAddress(): string {
        return AddressFormatter.getFullGenericAddress(
            this.houseNumber,
            this.address1,
            this.address2,
            this.address3,
            this.address4,
            this.address5,
            this.address6,
            this.address7,
            this.address8,
            this.address9,
            this.postcode);
    }
}
