import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated To be deleted
 */
// eslint-disable-next-line max-classes-per-file
export class Region {

    @alias('id')
    id: number;

    @alias('description')
    description: string;

    @alias('description')
    descriptionAlt: string;

    @alias('stateId')
    stateId: number;
}
