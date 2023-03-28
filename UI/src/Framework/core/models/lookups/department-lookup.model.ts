import { alias } from '@core-module/config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class DepartmentLookup {
    @alias('DepartmentId')
    id: number;

    @alias('Description')
    description: string;
}
