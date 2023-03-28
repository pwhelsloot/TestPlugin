import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import * as moment from 'moment';

export class ScheduleDateUtil {
    /**
     * Combines a date and time into a datetime
     * @param date
     * @param time
     * @returns The given date with the given time added
     */
    static combineDateAndTime(date: Date, time: Date): Date {
    date.setHours(time.getHours());
    date.setMinutes(time.getMinutes());
    date.setSeconds(time.getSeconds());
    date.setMilliseconds(time.getMilliseconds());
    return date;
  }

  /**
   * Converts the given dates hours, minutes and seconds as a timespan in seconds
   * @param date
   * @returns Timespan in seconds
   */
  static getTimespanInSeconds(date: Date): number {
    return moment.duration(date.getHours(), 'hours')
    .add(moment.duration(date.getMinutes(), 'minutes'))
    .add(moment.duration(date.getSeconds(), 'seconds')).asSeconds();
  }

  /**
   * Converts seconds into a date
   * @param seconds
   * @returns Todays date with time set using the given seconds
   */
  static setTimespanInSeconds(seconds: number): Date {
      return new Date(AmcsDate.create().setSeconds(seconds));
  }
}
