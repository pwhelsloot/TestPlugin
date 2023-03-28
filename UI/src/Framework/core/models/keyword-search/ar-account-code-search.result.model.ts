import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ARAccountCodeSearchResult extends ApiBaseModel {

    @amcsJsonMember('ARAccountCode')
    arAccountCode: string;

    @amcsJsonMember('CustomerId')
    customerId: number;

    @amcsJsonMember('CustomerName')
    customerName: string;

    @amcsJsonMember('SiteName')
    siteName: string;

    @amcsJsonMember('HouseNumber')
    houseNumber: string;

    @amcsJsonMember('Address1')
    address1: string;

    @amcsJsonMember('Address2')
    address2: string;

    @amcsJsonMember('Address3')
    address3: string;

    @amcsJsonMember('Address4')
    address4: string;

    @amcsJsonMember('Address5')
    address5: string;

    @amcsJsonMember('Postcode')
    postCode: string;
}
