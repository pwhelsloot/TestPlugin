import { ErpDayOfWeekEnum } from '../models/erp-day-of-week.enum';
import { TypescriptDayOfWeekEnum } from '../models/ts-day-of-week.enum';

/**
 * @deprecated Move to PlatformUI
 */
export function convertErpDayOfWeekToTypescript(erpDayOfWeek: ErpDayOfWeekEnum): TypescriptDayOfWeekEnum {
    switch (erpDayOfWeek) {
        case ErpDayOfWeekEnum.Monday:
            return TypescriptDayOfWeekEnum.Monday;

        case ErpDayOfWeekEnum.Tuesday:
            return TypescriptDayOfWeekEnum.Tuesday;

        case ErpDayOfWeekEnum.Wednesday:
            return TypescriptDayOfWeekEnum.Wednesday;

        case ErpDayOfWeekEnum.Thursday:
            return TypescriptDayOfWeekEnum.Thursday;

        case ErpDayOfWeekEnum.Friday:
            return TypescriptDayOfWeekEnum.Friday;

        case ErpDayOfWeekEnum.Saturday:
            return TypescriptDayOfWeekEnum.Saturday;

        case ErpDayOfWeekEnum.Sunday:
            return TypescriptDayOfWeekEnum.Sunday;
    }
}
