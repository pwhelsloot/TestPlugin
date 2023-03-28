
import { alias } from '@coreconfig/api-dto-alias.function';
/**
 * @deprecated Move to PlatformUI
 */
export class ContractSearchOrderResult {

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
