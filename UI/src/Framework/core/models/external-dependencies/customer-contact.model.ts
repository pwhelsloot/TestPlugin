import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class CustomerContact extends ApiBaseModel {

    @alias('CustomerId')
    @amcsJsonMember('CustomerId')
    customerId: number;

    @alias('ContactId')
    @amcsJsonMember('ContactId')
    contactId: number;

    @alias('CustomerSiteId')
    @amcsJsonMember('CustomerSiteId')
    customerSiteId: number;

    @alias('LocationId')
    @amcsJsonMember('LocationId')
    locationId: number;

    @alias('Forename')
    @amcsJsonMember('Forename')
    foreName: string;

    @alias('Surname')
    @amcsJsonMember('Surname')
    surName: string;

    @alias('JobTitle')
    @amcsJsonMember('JobTitle')
    jobTitle: string;

    @alias('TelNo')
    @amcsJsonMember('TelNo')
    telNo: string;

    @alias('TelExtension')
    @amcsJsonMember('TelExtension')
    telExtension: string;

    @alias('OtherTelNo')
    @amcsJsonMember('OtherTelNo')
    otherTelNo: string;

    @alias('Email')
    @amcsJsonMember('Email')
    email: string;

    @alias('SiteName')
    @amcsJsonMember('SiteName')
    siteName: string;

    getContactName(): string {
        if (this.foreName) {
            return `${this.foreName} ${this.surName}`;
        } else {
            return `${this.surName}`;
        }
    }
}
