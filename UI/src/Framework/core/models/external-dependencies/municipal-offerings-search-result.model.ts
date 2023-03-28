import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
export class MunicipalOfferingsSearchResult {

  @alias('MunicipalAccountId')
  municipalAccountId: number;

  @alias('searchText')
  searchText: string;

  @alias('CustomerSiteId')
  customerSiteId: number;

  @alias('BusinessTypeId')
  businessTypeId: number;

  @alias('BusinessType')
  businessType: string;

  @alias('FunctionalId')
  functionalId: string;

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

  @alias('LocalAuthority')
  localAuthority: string;

  @alias('MunicipalAccountState')
  municipalAccountState: string;

  @alias('CustomerStateId')
  customerStateId: number;

  @alias('CustomerState')
  customerState: string;

  @alias('SiteName')
  siteName: string;

  @alias('MunicipalAccountBuildingType')
  municipalAccountBuildingType: string;

  displayAddress: string;
}
