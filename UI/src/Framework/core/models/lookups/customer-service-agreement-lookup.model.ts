import { alias } from '@coreconfig/api-dto-alias.function';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated To be deleted
 */
export class CustomerServiceAgreementLookup implements ILookupItem {
    @alias('ServiceAgreementId')
    id: number;

    @alias('Description')
    description: string;

    @alias('PrimaryServiceId')
    primaryServiceId: number;
}
