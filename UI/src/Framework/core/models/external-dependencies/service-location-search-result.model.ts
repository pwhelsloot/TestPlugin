import { alias } from '@coreconfig/api-dto-alias.function';
import { SeviceLocationSearchOrderResult } from './service-location-search-order-result.model';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
export class ServiceLocationSearchResult {

    @alias('searchText')
    searchText: string;

    @alias('SiteName')
    siteName: string;

    @alias('CustomerId')
    customerId: number;

    @alias('CustomerSiteId')
    customerSiteId: number;

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

    @alias('Orders')
    orders: SeviceLocationSearchOrderResult[];

    @alias('Rank')
    rank: number;

    @alias('CustomerName')
    customerName: string;

    @alias('CustomerSiteStateId')
    customerSiteStateId: number;

    @alias('IsNonServiceSite')
    isNonServiceSite: boolean;

    displayOrders: SeviceLocationSearchOrderResult[];

    isOpen = false;

    // populated via map
    displayAddress: string;

    constructor() {
        this.orders = [];
        this.displayOrders = [];
    }
}
