import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
export class SeviceLocationSearchOrderResult {
    @alias('searchText')
    searchText: string;

    @alias('OrderNo')
    orderNo: string;

    @alias('ServiceName')
    serviceName: string;

    @alias('MaterialProfile')
    materialProfile: string;

    @alias('ContainerType')
    containerType: string;

    // populated via map
    displayOrder: string;
}
