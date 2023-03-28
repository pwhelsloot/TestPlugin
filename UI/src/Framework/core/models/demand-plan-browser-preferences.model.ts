import { alias } from '../config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class DemandPlanBrowserPreferences {

    @alias('dateRange')
    dateRange: [Date, Date];

    @alias('status')
    status: number;

    @alias('outlets')
    outlets: number[];
}
