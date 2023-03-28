import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ContainerSearchResult extends ApiBaseModel {

  @amcsJsonMember('ContainerId')
  containerId: number;

  @amcsJsonMember('SerialNo')
  serialNo: string;

  @amcsJsonMember('Tag')
  tag: string;

  @amcsJsonMember('ContainerType')
  containerType: string;

  @amcsJsonMember('Status')
  status: string;

  @amcsJsonMember('CustomerId')
  customerId?: number;

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

  @amcsJsonMember('CompanyOutlet')
  companyOutlet: string;

  @amcsJsonMember('OrderNo')
  orderNo: string;

  @amcsJsonMember('PictureBase64')
  pictureBase64: string;

  @amcsJsonMember('SerialAndTag')
  serialAndTag: string;

  formattedAddress: string;
}
