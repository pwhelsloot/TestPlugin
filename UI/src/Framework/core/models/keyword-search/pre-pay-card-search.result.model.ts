import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class PrePayCardSearchResult extends ApiBaseModel {

    @amcsJsonMember('CustomerName')
    customerName: string;

    @amcsJsonMember('SiteHouseNo')
    siteHouseNo: string;

    @amcsJsonMember('SiteAddress1')
    siteAddress1: string;

    @amcsJsonMember('SiteAddress2')
    siteAddress2: string;

    @amcsJsonMember('SiteAddress3')
    siteAddress3: string;

    @amcsJsonMember('SiteAddress4')
    siteAddress4: string;

    @amcsJsonMember('SiteAddress5')
    siteAddress5: string;

    @amcsJsonMember('SitePostcode')
    sitePostcode: string;

    @amcsJsonMember('PhoneNumber')
    phoneNumber: string;

    @amcsJsonMember('CardNumber')
    cardNumber: string;

    @amcsJsonMember('IsActive')
    isActive: boolean;

    @amcsJsonMember('OnHold')
    onHold: boolean;

    @amcsJsonMember('DeactivatedDate', true)
    deactivatedDate?: Date;

    formattedAddress: string;

    status: string;
}
