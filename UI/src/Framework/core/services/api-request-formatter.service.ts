import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import * as moment from 'moment-timezone';

export class APIRequestFormatterService {
    // Used for sending dates to API
    // This turns a typescript date/time with a timezone into UTC, it then adds the number
    // of hours to the UTC date/time to 'compensate' for when toJSON is called. In reality
    // this means a date chosen for 18/04/2019 in any country/timezone will result in
    // a string of '2019-04-18T00:00:00Z'
    static formatDateToUTC(date: Date): string {
        const copiedDate = AmcsDate.createFrom(date);
        if (isTruthy(copiedDate)) {
            copiedDate.setHours(date.getHours(), date.getMinutes() + -copiedDate.getTimezoneOffset(), date.getSeconds());
            return copiedDate.toJSON();
        } else {
            return null;
        }
    }

    // Used for parsing dates from API
    // This create a javascript date object and sets the times (if given) from the
    // api date. Note if the API has added a timezone onto this we are screwed. We should
    // never send offsets from the API otherwise 'new Date(date)' will use the api offset and give us a different
    // date/time value that's really in the string
    static formatDateFromUTC(date: Date | string | number): Date {
        const dateWithTime = AmcsDate.createFrom(date);
        if (isTruthy(dateWithTime)) {
            const temp = AmcsDate.convertObjectToDate(date);
            dateWithTime.setHours(temp.getHours());
            dateWithTime.setMinutes(temp.getMinutes());
            dateWithTime.setSeconds(temp.getSeconds());
            return dateWithTime;
        } else {
            return null;
        }
    }

    static parseMomentFromZonedDateTimeText(zonedDateTime: { DateTime: string; TimeZone: string }): moment.Moment {
        return moment(zonedDateTime.DateTime, 'YYYY-MM-DDTHH:mm:ssZ').tz(zonedDateTime.TimeZone);
    }

    static serialiseMomentToZonedDateTimeText(zonedMoment: moment.Moment): { DateTime: string; TimeZone: string } {
        if (!zonedMoment) {
            return null;
        }

        const isoString = zonedMoment.format('YYYY-MM-DDTHH:mm:ssZ');
        const timeZone = zonedMoment.tz();

        return { DateTime: isoString, TimeZone: timeZone };
    }

    static parseLocalDate(date: string): Date {
        return AmcsDate.createFrom(date);
    }

    static serialiseLocalDate(date: Date) {
        if (isTruthy(date)) {
            return date.toISOString().substring(0, 10);
        }
        return date;
    }
}
