import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class FederalIdSearchResult extends ApiBaseModel {

  @amcsJsonMember('CustomerId')
  customerId: number;

  @amcsJsonMember('CustomerName')
  customerName: string;

  @amcsJsonMember('CustomerSiteId')
  customerSiteId: number;

  @amcsJsonMember('SiteName')
  siteName: string;

  @amcsJsonMember('FederalId')
  federalId: string;

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

  formattedAddress: string;
}
