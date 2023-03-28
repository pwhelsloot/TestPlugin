import { DateValidationHelper } from '@core-module/helpers/date-validation-helper';
import { isNullOrUndefined, isTruthy } from '@core-module/helpers/is-truthy.function';

export const compareDates = (xValue: Date, yValue: Date) => {
    if (isNullOrUndefined(xValue) && isNullOrUndefined(yValue)) {
        return 0;
    } else if (isTruthy(xValue) && isNullOrUndefined(yValue)) {
        return 1;
    } else if (isNullOrUndefined(xValue) && isTruthy(yValue)) {
        return -1;
    } else if (xValue > yValue) {
        return 1;
    } else if (xValue < yValue) {
        return -1;
    } else {
        return 0;
    }
};

export class AmcsDate {
    static create(): Date {
        const date = new Date();
        const dateWithoutTime = new Date(date.getFullYear(), date.getMonth(), date.getDate());
        return dateWithoutTime;
    }

    static createFrom(date: Date | string | number): Date {
        if (isTruthy(date)) {
            const temp = this.convertObjectToDate(date);
            const dateWithoutTime = new Date(temp.getFullYear(), temp.getMonth(), temp.getDate());
            if (DateValidationHelper.isValidDate(dateWithoutTime)) {
                return dateWithoutTime;
            } else {
                return null;
            }
        } else {
            return null;
        }
    }

    static createFromKeepTime(date: Date | string | number): Date {
        const dateWithTime = this.createFrom(date);
        if (isTruthy(dateWithTime)) {
            const temp = this.convertObjectToDate(date);
            dateWithTime.setHours(temp.getHours());
            dateWithTime.setMinutes(temp.getMinutes());
            dateWithTime.setSeconds(temp.getSeconds());
            dateWithTime.setMilliseconds(temp.getMilliseconds());
            return dateWithTime;
        } else {
            return null;
        }
    }

    static createFor(year: number, month: number, date?: number) {
        const dateWithoutTime = this.create();
        dateWithoutTime.setFullYear(year);
        if (isTruthy(date)) {
            dateWithoutTime.setMonth(month, date);
        } else {
            dateWithoutTime.setMonth(month);
        }
        return dateWithoutTime;
    }

    static addDays(date: Date | string, days: number): Date {
        const dateWithoutTime = this.createFrom(date);
        if (isTruthy(dateWithoutTime)) {
            dateWithoutTime.setDate(dateWithoutTime.getDate() + days);
            return dateWithoutTime;
        } else {
            return null;
        }
    }

    static now(): Date {
        return new Date();
    }

    static removeTime(date: Date): Date {
        return new Date(date.getFullYear(), date.getMonth(), date.getDate());
    }

    static convertObjectToDate(value: any) {
        let dateValue: Date = null;
        if (isTruthy(value)) {
            if (typeof value === 'string') {
                // Attempt to convert with current format
                dateValue = new Date(value);

                // If unable to convert, attempt to use iOS format
                if (isNullOrUndefined(dateValue) || isNaN(dateValue.valueOf())) {
                    let temp = value.toString();
                    if (temp.length <= 'YYYY-MM-DD'.length) {
                        temp = temp + ' 00:00:00';
                    }
                    temp = temp.replace(' ', 'T');
                    dateValue = new Date(temp);

                    // If unable to convert with iOS format, set date to null
                    if (isNullOrUndefined(dateValue) || isNaN(dateValue.valueOf())) {
                        dateValue = null;
                    }
                }
            } else if (typeof value === 'number') {
                dateValue = new Date(value);
            } else {
                dateValue = value as Date;
            }
        }
        return dateValue;
    }
}
