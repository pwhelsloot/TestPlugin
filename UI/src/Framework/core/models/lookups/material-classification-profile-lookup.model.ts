import { alias } from '@core-module/config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class MaterialClassificationProfileLookup {

    @alias('MaterialId')
    id: number;

    @alias('Description')
    description: string;

    @alias('MaterialClassId')
    materialClassId: number;
}
