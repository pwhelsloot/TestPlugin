import { Observable, ReplaySubject } from 'rxjs';
import { AmcsDateRangeConfig } from '@shared-module/components/amcs-date-range-filter/amcs-date-range-config.model';
import { AmcsDateRangeConfigFilter } from '@shared-module/components/amcs-date-range-filter/amcs-date-range-config-filter.model';
import { Injectable } from '@angular/core';

@Injectable()
export class AmcsDateRangeFilterService {

  dateRange$: Observable<[Date, Date]>;
  requestedDate$: Observable<Date>;
  filter$: Observable<AmcsDateRangeConfigFilter>;

  config = AmcsDateRangeConfig.getDefaultOperationsConfig();

  constructor() {
    this.dateRange = new ReplaySubject<[Date, Date]>(1);
    this.dateRange$ = this.dateRange.asObservable();

    this.requestedDate = new ReplaySubject<Date>(1);
    this.requestedDate$ = this.requestedDate.asObservable();

    this.filter = new ReplaySubject<AmcsDateRangeConfigFilter>(1);
    this.filter$ = this.filter.asObservable();
  }

  private dateRange: ReplaySubject<[Date, Date]>;
  private requestedDate: ReplaySubject<Date>;
  private filter: ReplaySubject<AmcsDateRangeConfigFilter>;

  updateDateRange(from: Date, to: Date) {
    this.dateRange.next([from, to]);
  }

  updateRequestedDate(requestedDate: Date) {
    this.requestedDate.next(requestedDate);
  }

  updateDateRangeFilter(filter: AmcsDateRangeConfigFilter) {
    this.filter.next(filter);
  }
}
