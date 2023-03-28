import { Component, ElementRef, Input, OnInit, Renderer2, ViewEncapsulation } from '@angular/core';
import { DateAdapter } from '@angular/material/core';
import { DateValidationHelper } from '@core-module/helpers/date-validation-helper';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { AmcsDateRangeFilterService } from '@coreservices/amcs-date-range-filter.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { take } from 'rxjs/operators';
import { AmcsDateRangeConfigFilter, AmcsDateRangeConfigFilterType } from './amcs-date-range-config-filter.model';

/**
 * @todo Use https://material.angular.io/components/datepicker/overview#comparison-ranges instead of mat-calendar
 */
@Component({
    selector: 'app-amcs-date-range-filter',
    templateUrl: './amcs-date-range-filter.component.html',
    styleUrls: ['./amcs-date-range-filter.component.scss'],
    // view encapsulation is switched off here so we can style the embedded mat-calendar controls
    encapsulation: ViewEncapsulation.None
})
export class AmcsDateRangeFilterComponent extends AutomationLocatorDirective implements OnInit {

    @Input('isSecondaryUi') isSecondaryUi = false;
    @Input() restrictCustomDateRange = false;
    @Input() dateRangeSelectorRight = false;
    @Input() isDisabled = false;

    // the current from and to date as published by the service.
    from: Date;
    to: Date;

    // the from and to date as displayed on the custom date pickers (can be different from the above until apply is clicked.)
    customFrom: Date;
    customTo: Date;
    canApply = true;
    custom = false;
    minDate: Date;
    maxDate: Date;

    expanded: boolean;
    selectedFilter: AmcsDateRangeConfigFilter;
    AmcsDateRangeConfigFilterType = AmcsDateRangeConfigFilterType;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        public dateRangeFilterService: AmcsDateRangeFilterService,
        private adapter: DateAdapter<any>,
        private localSettings: TranslateSettingService) {
        super(elRef, renderer);
        this.localSettings.selectedLanguage.pipe(take(1)).subscribe(language => {
            this.adapter.setLocale(language);
        });
    }

    private now = AmcsDate.create();
    private requestedDate: Date;

    ngOnInit() {
        this.dateRangeFilterService.dateRange$.subscribe((dateRange: [Date, Date]) => {
            this.from = dateRange[0];
            this.to = dateRange[1];
            this.customFrom = this.from;
            this.customTo = this.to;
        });

        this.dateRangeFilterService.requestedDate$.subscribe((value: Date) => {
            this.requestedDate = value;
            this.applyPredefinedDateRange();
        });

        this.dateRangeFilterService.filter$.subscribe((filter: AmcsDateRangeConfigFilter) => {
            const filters = this.dateRangeFilterService.config.filters.filter(x =>
                x.type === filter.type &&
                x.startN === filter.startN &&
                x.endN === filter.endN
            );
            if (isTruthy(filters) && filters.length > 0) {
                this.select(filters[0]);
            }
        });
    }

    select(filter: AmcsDateRangeConfigFilter) {
        this.selectedFilter = filter;

        if (this.selectedFilter.type === AmcsDateRangeConfigFilterType.custom) {
            this.custom = true;
            this.minDate = filter.minDate;
            this.maxDate = filter.maxDate;
        } else {
            this.applyPredefinedDateRange();
            this.expanded = false;
            this.custom = false;
        }
    }

    expand() {
        if (this.expanded) {
            this.cancel();
        } else if (!this.isDisabled) {
            this.expanded = true;
        }
    }

    cancel() {
        this.expanded = false;
        if (isTruthy(this.selectedFilter) && this.selectedFilter.type === AmcsDateRangeConfigFilterType.custom) {
            this.customFrom = this.from;
            this.customTo = this.to;
        }
    }

    cancelOutside(event: any) {
        if (event.target && typeof event.target.className === 'string' && event.target.className.indexOf('mat-calendar-body-cell-content') === -1 && this.expanded) {
            this.cancel();
        }
    }

    apply() {
        this.dateRangeFilterService.updateDateRange(this.customFrom, this.customTo);
        this.expanded = false;
    }

    updateFromDate(newDate) {
        this.customFrom = newDate;
        this.canApply = DateValidationHelper.IsRangeValid(this.customFrom, this.customTo);
    }

    updateToDate(newDate) {
        this.customTo = newDate;
        this.canApply = DateValidationHelper.IsRangeValid(this.customFrom, this.customTo);
    }

    private applyPredefinedDateRange() {
        let from: Date, to: Date;

        if (isTruthy(this.selectedFilter) && this.selectedFilter.type !== AmcsDateRangeConfigFilterType.custom) {
            switch (this.selectedFilter.type) {
                case AmcsDateRangeConfigFilterType.today:
                    from = this.createDate(this.selectedFilter.startN);
                    to = this.createDate(this.selectedFilter.startN);
                    break;
                case AmcsDateRangeConfigFilterType.todayRange:
                    from = this.createDate(this.selectedFilter.startN);
                    to = this.createDate(this.selectedFilter.endN);
                    break;
                case AmcsDateRangeConfigFilterType.month:
                    from = AmcsDate.createFor(this.now.getFullYear(), this.now.getMonth() + this.selectedFilter.startN, 1);
                    to = AmcsDate.createFor(this.now.getFullYear(), this.now.getMonth() + this.selectedFilter.startN + 1, 0);
                    break;
                case AmcsDateRangeConfigFilterType.monthRange:
                    from = AmcsDate.createFor(this.now.getFullYear(), this.now.getMonth() + this.selectedFilter.startN, 1);
                    to = AmcsDate.createFor(this.now.getFullYear(), this.now.getMonth() + this.selectedFilter.endN + 1, 0);
                    break;
                case AmcsDateRangeConfigFilterType.date:
                    from = this.createDateFromRequestedDate(this.selectedFilter.startN);
                    to = this.createDateFromRequestedDate(this.selectedFilter.startN);
                    break;
                case AmcsDateRangeConfigFilterType.dateRange:
                    from = this.createDateFromRequestedDate(this.selectedFilter.startN);
                    to = this.createDateFromRequestedDate(this.selectedFilter.endN);
                    break;
            }

            this.dateRangeFilterService.updateDateRange(from, to);
        }
    }

    private createDate(offsetDays?: number) {
        offsetDays = offsetDays ? offsetDays : 0;
        return AmcsDate.createFor(this.now.getFullYear(), this.now.getMonth(), this.now.getDate() + offsetDays);
    }

    private createDateFromRequestedDate(offsetDays?: number) {
        const date = isTruthy(this.requestedDate) ? this.requestedDate : this.now;
        offsetDays = offsetDays ? offsetDays : 0;
        return AmcsDate.createFor(date.getFullYear(), date.getMonth(), date.getDate() + offsetDays);
    }
}
