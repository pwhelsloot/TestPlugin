import { alias } from '@coreconfig/api-dto-alias.function';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
export class OutletServiceAgreementLookup implements ILookupItem {

    @alias('ServiceAgreementId')
    id: number;

    @alias('Description')
    description: string;

    @alias('PrimaryServiceId')
    primaryServiceId: number;
}
