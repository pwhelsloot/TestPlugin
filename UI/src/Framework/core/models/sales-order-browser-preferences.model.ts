import { alias } from '@core-module/config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class SalesOrderBrowserPreferences {

    @alias('outlets')
    outletIds: number[];

    @alias('status')
    status: number;

    @alias('dates')
    dates: [Date, Date];
}
