import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated To be deleted
 */
export class CallTypeLookup {
    @alias('CallTypeId')
    id: number;

    @alias('Description')
    description: string;

    @alias('IsDefault')
    isDefault: boolean;
}
