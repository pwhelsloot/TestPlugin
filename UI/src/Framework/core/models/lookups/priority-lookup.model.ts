import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated To be deleted
 */
export class PriorityLookup {

    @alias('PriorityId')
    id: number;

    @alias('Description')
    description: string;
}
