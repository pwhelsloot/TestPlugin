import { isTruthy } from '@core-module/helpers/is-truthy.function';

export enum AmcsDateRangeConfigFilterType {
  today = 1,
  todayRange = 2,
  month = 3,
  monthRange = 4,
  date = 5,
  dateRange = 6,
  custom = 7
}

export class AmcsDateRangeConfigFilter {

  constructor(public type: AmcsDateRangeConfigFilterType,
    public startN?: number,
    public endN?: number,
    public sharedTranslationKey?: string,
    public minDate?: Date,
    public maxDate?: Date) {
    if (!isTruthy(this.startN)) {
      this.startN = 0;
    }
    if (!isTruthy(this.endN)) {
      this.endN = 0;
    }
    if (!isTruthy(this.sharedTranslationKey)) {
      this.setSharedTranslationKey();
    }
  }

  private setSharedTranslationKey() {
    switch (this.type) {
      case AmcsDateRangeConfigFilterType.today:
        switch (this.startN) {
          case 0:
            this.sharedTranslationKey = 'dateRangeFilter.today';
            break;
          case -1:
            this.sharedTranslationKey = 'dateRangeFilter.yesterday';
            break;
        }
        break;

      case AmcsDateRangeConfigFilterType.todayRange:
        if (this.startN === -7 && this.endN === 0) {
          this.sharedTranslationKey = 'dateRangeFilter.lastSevenDays';
        } else if (this.startN === -30 && this.endN === 0) {
          this.sharedTranslationKey = 'dateRangeFilter.lastThirtyDays';
        } else if (this.startN === -60 && this.endN === 0) {
          this.sharedTranslationKey = 'dateRangeFilter.last60Days';
        } else if (this.startN === -120 && this.endN === 0) {
          this.sharedTranslationKey = 'dateRangeFilter.last120Days';
        } else if (this.startN === 0 && this.endN === 7) {
          this.sharedTranslationKey = 'dateRangeFilter.nextSevenDays';
        } else if (this.startN === -90 && this.endN === 0) {
          this.sharedTranslationKey = 'dateRangeFilter.lastThreeMonths';
        }
        break;

      case AmcsDateRangeConfigFilterType.month:
        switch (this.startN) {
          case 0:
            this.sharedTranslationKey = 'dateRangeFilter.thisMonth';
            break;
          case -1:
            this.sharedTranslationKey = 'dateRangeFilter.lastMonth';
            break;
          case +1:
            this.sharedTranslationKey = 'dateRangeFilter.nextMonth';
            break;
        }
        break;

      case AmcsDateRangeConfigFilterType.date:
        switch (this.startN) {
          case 0:
            this.sharedTranslationKey = 'dateRangeFilter.requestedDate';
            break;
        }
        break;

      case AmcsDateRangeConfigFilterType.dateRange:
        if (this.startN === 0 && this.endN === 7) {
          this.sharedTranslationKey = 'dateRangeFilter.requestedDatePlusSeven';
        } else if (this.startN === 0 && this.endN === 30) {
          this.sharedTranslationKey = 'dateRangeFilter.requestedDatePlusThirty';
        }
        break;

      case AmcsDateRangeConfigFilterType.custom:
        this.sharedTranslationKey = 'dateRangeFilter.custom';
        break;
    }
  }
}
