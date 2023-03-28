import { isTruthy } from './is-truthy.function';

export class DateValidationHelper {

    static IsRangeValid(fromDate: Date, toDate: Date): boolean {
        if (!fromDate || !toDate) {
            return false;
        } else {
            return fromDate <= toDate;
        }
    }

    static isValidDate(date: any) {
        return isTruthy(date) && Object.prototype.toString.call(date) === '[object Date]' && !isNaN(date);
    }
}
