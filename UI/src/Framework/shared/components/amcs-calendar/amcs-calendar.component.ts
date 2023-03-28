import { MediaMatcher } from '@angular/cdk/layout';
// eslint-disable-next-line max-len
import {
  ChangeDetectorRef,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  NgZone,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  QueryList,
  Renderer2,
  SimpleChanges,
  ViewChild,
  ViewChildren,
  ViewEncapsulation
} from '@angular/core';
import { NgModel } from '@angular/forms';
import { DateAdapter, MatOption } from '@angular/material/core';
import { MatCalendar } from '@angular/material/datepicker';
import { MatListOption, MatSelectionList } from '@angular/material/list';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { CoreUserPreferenceKeys } from '@core-module/models/preferences/core-user-preference-keys.model';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { MediaSizes } from '@coremodels/media-sizes.constants';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';
import { TrackingEventType } from '@coreservices/logging/tracking-event-type.enum';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ICalendarEventType } from '@shared-module/models/icalendar-event-type.model';
import { ICalendarEvent } from '@shared-module/models/icalendar-event.model';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { CalendarEvent } from 'angular-calendar';
import { MonthViewDay } from 'calendar-utils';
import { isSameDay, isWithinInterval } from 'date-fns';
import { take, withLatestFrom } from 'rxjs/operators';
import { SharedTranslationsService } from '../../services/shared-translations.service';
import { AmcsSwitchConfig } from '../amcs-switch/amcs-switch-config.model';
import { AmcsCalendarMultiDayService } from './amcs-calendar-multi-day.service';

@Component({
  selector: 'app-amcs-calendar',
  templateUrl: './amcs-calendar.component.html',
  styleUrls: ['./amcs-calendar.component.scss'],
  providers: [AmcsCalendarMultiDayService],
  encapsulation: ViewEncapsulation.None
})
export class AmcsCalendarComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {
  @Input('events') events: ICalendarEvent[] = [];
  @Input('eventTypes') eventTypes: ICalendarEventType[] = [];
  @Input('eventTypesList') eventTypesList: any[] = [];
  @Input('loading') loading = false;
  @Input('noMargin') noMargin = false;
  @Input('searchPlaceHolderText') searchPlaceHolderText;
  @Input('placeHolderList') placeHolderList;
  @Input('showSidebar') showSidebar = true;
  @Input('selectAllEnabled') selectAllEnabled = false;
  @Input('selectAll') selectAll = true;
  @Input('initialView') initialView: string;
  @Input('filterPersist') filterPersist: boolean;
  @Input('viewDate') viewDate: Date = AmcsDate.create();
  @Input('excludeDays') excludeDays: number[] = [];
  @Input('tooltipPlacement') tooltipPlacement = 'bottom';
  @Input('allowFilterToggle') allowFilterToggle = false;

  @Output('eventSelection') eventSelection: EventEmitter<number[]> = new EventEmitter<number[]>();
  @Output('dateChanged') dateChanged: EventEmitter<Date> = new EventEmitter<Date>();
  @Output('viewChanged') viewChanged: EventEmitter<string> = new EventEmitter<string>();
  @Output('filterChanged') filterChanged: EventEmitter<number[]> = new EventEmitter<number[]>();
  @Output('activitySelected') activitySelected: EventEmitter<ICalendarEvent> = new EventEmitter<ICalendarEvent>();
  @Output('activityEdited') activityEdited: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityDeleted') activityDeleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityCompleted') activityCompleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('onCalendarEventsChanged') onCalendarEventsChanged: EventEmitter<CalendarEvent[]> = new EventEmitter<CalendarEvent[]>();

  @ViewChild('calendar') calendar: MatCalendar<any>;
  @ViewChild('eventTypeList') eventTypeList: MatSelectionList;
  @ViewChild('mobileEventTypeList') mobileEventTypeList: NgModel;
  @ViewChildren('eventItems') eventItems: QueryList<MatListOption>;
  @ViewChildren('eventItemsMobile') eventItemsMobile: QueryList<MatOption>;

  view: string;
  tileViewDate: Date;
  calendarEvents: CalendarEvent[];
  mobileQuery: MediaQueryList;
  isActiveDayIsOpen = false;
  mobileSelectedEventsDisplay: string;
  dayOpen: Partial<MonthViewDay>;
  initialViewSet = false;

  selectedEventTypes: number[] = [];
  persistEventTypes: number[] = [];
  filterSegmentIndex: number;

  filteredCalendarEvents: ICalendarEvent[] = [];
  filteredEventTypes: ICalendarEventType[] = [];
  filteredEventTypesList: any[] = [];
  latestFilteredCalendarEvents: ICalendarEvent[] = [];

  switchConfig: AmcsSwitchConfig;
  enablePersist = false;
  isPersistSet = false;

  showFilter = false;
  colourTheCells = false;
  // color for amcs-label-grey (no html / css provided)
  selectAllType: ICalendarEventType = { id: 0, description: '', color: '#A3A4A5' };

  constructor(
    protected elRef: ElementRef,
    protected renderer: Renderer2,
    public translationService: SharedTranslationsService,
    private zone: NgZone,
    private readonly userPreferencesService: CoreUserPreferencesService,
    private instrumentationService: InstrumentationService,
    private changeDetectorRef: ChangeDetectorRef,
    media: MediaMatcher,
    private adapter: DateAdapter<any>,
    private localSettings: TranslateSettingService
  ) {
    super(elRef, renderer);
    setTimeout(() => {
      this.translationService.translations.pipe(withLatestFrom(this.localSettings.selectedLanguage), take(1)).subscribe((data) => {
        const translations: string[] = data[0];
        const language: string = data[1];
        this.searchPlaceHolderText = translations['calendar.filterEvents'];
        this.adapter.setLocale(language);
        this.switchConfig = {
          offText: translations['calendar.off'],
          onText: translations['calendar.on'],
          onColor: 'primary',
          offColor: 'primary'
        };
      });
    }, 1000);
    this.mobileQuery = media.matchMedia('(max-width: ' + MediaSizes.small.toString() + 'px)');
    this._mobileQueryListener = () => {
      // This re-renders the page once per media change
      this.zone.run(() => { });
    };
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  private _mobileQueryListener: () => void;

  ngOnInit() {
    this.updateViewDateBasedOnExcludeDays(this.viewDate);
    this.dateChanged.emit(this.viewDate);
    this.tileViewDate = this.viewDate;
    this.userPreferencesService
      .get<string>(CoreUserPreferenceKeys.calendarDefaultView, 'month')
      .pipe(take(1))
      .subscribe((calendarDefaultView: string) => {
        if (this.initialView && !this.initialViewSet) {
          this.view = this.initialView;
        } else {
          this.view = calendarDefaultView;
        }
        this.viewChanged.emit(this.view);
      });

    if (!this.allowFilterToggle) {
      this.showFilter = true;
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    const calendarEventChange = changes['events'];

    if (calendarEventChange && !calendarEventChange.firstChange) {
      this.closeOpenDay();
      this.filterEventsAndEventTypesByView(this.viewDate, this.view, this.eventTypes, this.events, true, this.eventTypesList);
    }

    if (changes['viewDate']) {
      this.updateViewDateBasedOnExcludeDays(changes['viewDate'].currentValue);
    }
  }

  ngOnDestroy() {
    this.mobileQuery.removeListener(this._mobileQueryListener);
    this.changeDetectorRef.detach();
  }

  setViewDateNoEvents(newDate: Date) {
    this.tileViewDate = newDate;
    if (isTruthy(this.calendar)) {
      this.calendar['_activeDate'] = this.tileViewDate;
    }
    this.viewDate = newDate;
  }

  // Deals with expanding/contracting day information
  dayClicked({ date, events }: { date: Date; events: CalendarEvent[] }): void {
    if ((isSameDay(this.viewDate, date) && this.isActiveDayIsOpen) || events.length === 0 || events.length <= 4) {
      this.closeOpenDay();
    } else {
      this.isActiveDayIsOpen = true;
      this.viewDate = date;
      this.dayOpen = { date: this.viewDate, events };
      this.viewDateChanged(this.viewDate);
    }
  }

  tileDateChanged(newDate) {
    this.tileViewDate = newDate;
    this.calendar['_activeDate'] = this.tileViewDate;
    // Act as if a day was clicked
    this.dayClicked({ date: newDate, events: this.getEventsForDate(newDate) });
    this.viewDate = newDate;
    this.viewDateChanged(this.viewDate);
  }

  eventSelectionChanged(event?: any, filterIndex?: number, filterType?: any) {
    this.isPersistSet = false;
    if (event && filterIndex >= 0 && filterType) {
      this.filterSegmentIndex = filterIndex;
    }
    this.filterEventsAndEventTypesByView(this.viewDate, this.view, this.eventTypes, this.events, false, this.eventTypesList);
  }

  viewDateChanged(date: Date) {
    this.updateViewDateBasedOnExcludeDays(this.viewDate);
    this.filterSegmentIndex = -1;
    this.filterEventsAndEventTypesByView(this.viewDate, this.view, this.eventTypes, this.events, true, this.eventTypesList);
    this.dateChanged.emit(date);
  }

  closeOpenDay() {
    this.isActiveDayIsOpen = false;
    this.dayOpen = null;
  }

  changeView(view: string) {
    this.filterSegmentIndex = -1;
    this.initialViewSet = true;
    // ensures that the selectAll is selected on changing the view
    this.selectAll = true;
    this.instrumentationService.trackEvent(LoggingVerbs.ViewModeCalendarChange, { view }, null, TrackingEventType.CustomerCalendar);
    this.view = view;

    this.userPreferencesService.save<string>(CoreUserPreferenceKeys.calendarDefaultView, this.view);
    this.filterEventsAndEventTypesByView(this.viewDate, this.view, this.eventTypes, this.events, true, this.eventTypesList);
    this.viewChanged.emit(this.view);
  }

  onEventTypeFilter(event: any, searchIndex?: number) {
    if (searchIndex == null) {
      this.eventItems.forEach((t) => {
        const currentLabel: string = t.getLabel();
        t.disabled = currentLabel.toLowerCase().indexOf(event.target.value.toLowerCase()) < 0;
      });
    } else {
      let startingPosition = 0;
      for (let i = 0; i < searchIndex; i++) {
        startingPosition += this.filteredEventTypesList[i].length;
      }
      this.eventItems.forEach((t, index) => {
        if (index >= startingPosition && index <= startingPosition + this.filteredEventTypesList[searchIndex].length - 1) {
          const currentLabel: string = t.getLabel();
          t.disabled = currentLabel.toLowerCase().indexOf(event.target.value.toLowerCase()) < 0;
        }
      });
    }
  }

  onMobileEventTypeFilter(event: any) {
    this.eventItemsMobile.forEach((t) => {
      const currentLabel: string = t.getLabel();
      t.disabled = currentLabel.toLowerCase().indexOf(event.target.value.toLowerCase()) < 0;
    });
  }

  onActivitySelected(meta: { id: number; typeId: number }) {
    this.activitySelected.next(this.events.find((x) => x.id === meta.id && x.typeId === meta.typeId));
  }

  onEventEdit(data: any) {
    this.activityEdited.emit(data);
  }

  onEventDelete(data: any) {
    this.activityDeleted.emit(data);
  }

  onEventComplete(data: any) {
    this.activityCompleted.emit(data);
  }
  getFullEvent(id: number, typeId: number): ICalendarEvent {
    return this.events.find((x) => x.id === id && x.typeId === typeId);
  }

  selectAllChanged() {
    this.selectAll = !this.selectAll;
    this.filterEventsAndEventTypesByView(this.viewDate, this.view, this.filteredEventTypes, this.events, true);
  }

  getSelectValue(type: any, index: number) {
    if (this.filteredEventTypesList.length > 0 && this.filterPersist && this.enablePersist) {
      this.updatePersistEventTypes();
    }
    return +('9' + index + type.id);
  }

  persistChange() {
    this.persistEventTypes = this.selectedEventTypes;
    this.isPersistSet = true;
  }

  dateFilter = (date: Date): boolean => this.isDateValidForExcludeDays(date);

  toggleShowFilter() {
    this.showFilter = !this.showFilter;
  }

  private getEventsForDate(date: Date): CalendarEvent[] {
    if (isTruthy(this.calendarEvents)) {
      return this.calendarEvents.filter(
        (event) => isSameDay(date, event.start) || (event.end != null && isWithinInterval(date, { start: event.start, end: event.end }))
      );
    }
    else {
      return [];
    }
  }

  private filterEventsAndEventTypesByView(
    date: Date,
    viewType: string,
    eventTypes: ICalendarEventType[],
    events: ICalendarEvent[],
    detectAvailableEventTypes: boolean = false,
    eventTypesList: any[] = []
  ) {
    switch (viewType) {
      case 'month':
        this.filteredCalendarEvents = events;
        this.filteredEventTypes = eventTypes;
        this.filteredEventTypesList = eventTypesList;
        break;
      case 'week':
        const week = this.getCurrentWeek(date);
        this.filteredCalendarEvents = events.filter((x) => x.start <= week[1] && x.end >= week[0]);
        this.filteredEventTypes = eventTypes.filter((x) => this.filteredCalendarEvents.find((y) => y.typeId === x.id));
        this.filteredEventTypesList = eventTypesList;
        break;
      case 'day':
        this.filteredCalendarEvents = events.filter(event =>
          event.start?.getDate() === date.getDate() ||
          event.end?.getDate() === date.getDate() ||
          (event.start?.getDate() <= date.getDate() &&
            date.getDate() <= event.end?.getDate())
        );
        this.filteredEventTypes = eventTypes.filter((x) => this.filteredCalendarEvents.find((y) => y.typeId === x.id));
        this.filteredEventTypesList = eventTypesList;
        break;
    }

    if (detectAvailableEventTypes) {
      if (this.filteredCalendarEvents.length > 0) {
        if (!this.enablePersist || !this.isPersistSet) {
          this.isPersistSet = false;
          this.persistEventTypes = [];
          this.selectedEventTypes = [];
          if (this.filteredEventTypes.length > 0) {
            this.selectedEventTypes = this.filteredEventTypes.map((t) => t.id);
          } else if (this.filteredEventTypesList.length > 0) {
            for (let i = 0; i < this.filteredEventTypesList.length; i++) {
              for (let j = 0; j < this.filteredEventTypesList[i].length; j++) {
                this.selectedEventTypes.push(+('9' + i + this.filteredEventTypesList[i][j].id));
              }
            }
          } else {
            this.selectedEventTypes = [];
          }
        } else if (!this.isPersistSet) {
          this.persistEventTypes = this.selectedEventTypes;
          this.isPersistSet = true;
        }
      } else {
        if (!this.enablePersist) {
          this.selectedEventTypes = [];
        }
      }
      if (this.selectAllEnabled) {
        this.selectedEventTypes.push(this.selectAllType.id);
      }
    }
    this.mobileSelectedEventsDisplay = this.filteredEventTypes
      .filter((x) => x.id !== 0 && this.selectedEventTypes.find((y) => y === x.id))
      .map((item) => item.description)
      .join(', ');

    if (this.selectAllEnabled) {
      const selectedEventTypesWithoutAll = this.selectedEventTypes.filter((eventType) => eventType !== this.selectAllType.id);
      const filteredEventTypesWithoutAll = this.filteredEventTypes.filter((eventType) => eventType.id !== this.selectAllType.id);

      if (selectedEventTypesWithoutAll.length <= filteredEventTypesWithoutAll.length) {
        // Determine if all filtered event types are currently selected.
        this.selectAll = !filteredEventTypesWithoutAll.some(
          (filteredEventType) => !selectedEventTypesWithoutAll.some((selectedEventType) => selectedEventType === filteredEventType.id)
        );

        this.selectedEventTypes = this.selectAll ? [...selectedEventTypesWithoutAll, this.selectAllType.id] : selectedEventTypesWithoutAll;
      }
      if (!this.selectAll && detectAvailableEventTypes) {
        this.selectedEventTypes = [];
      }
    }

    this.populateEvents();
    this.refreshOpenDayEvents();

    this.filterChanged.emit(this.selectedEventTypes);
    this.onCalendarEventsChanged.emit(this.calendarEvents);
    this.changeDetectorRef.detectChanges();
  }

  private refreshOpenDayEvents() {
    if (this.dayOpen != null) {
      const events = this.getEventsForDate(this.viewDate);
      if (events && events.length <= 0) {
        this.closeOpenDay();
      } else {
        this.dayOpen = { date: this.viewDate, events };
      }
    }
  }

  private populateEvents() {
    if (this.filteredEventTypesList.length > 0) {
      this.calendarEvents = [];
      this.latestFilteredCalendarEvents = [];
      const columnName = this.filterSegmentIndex >= 0 ? this.filteredEventTypesList[this.filterSegmentIndex][0].filterType : '';
      let updatedCalendarEvents = [];
      if (columnName) {
        for (let i = 0; i < this.filteredCalendarEvents.length; i++) {
          if (typeof this.filteredCalendarEvents[i][columnName] === 'boolean') {
            if (this.filteredCalendarEvents[i][columnName] === this.selectedEventTypes.length > 0) {
              updatedCalendarEvents.push(this.filteredCalendarEvents[i]);
            }
          } else {
            if (this.selectedEventTypes.includes(+('9' + this.filterSegmentIndex + this.filteredCalendarEvents[i][columnName]))) {
              updatedCalendarEvents.push(this.filteredCalendarEvents[i]);
            }
          }
        }
      } else {
        if (this.filteredEventTypesList.length > 0 && this.filterPersist && this.enablePersist) {
          this.updateEventsBasedOnFilter();
        }
        updatedCalendarEvents = this.filteredCalendarEvents;
      }
      updatedCalendarEvents.forEach((event) => {
        const newEvent: CalendarEvent = {
          start: event.start,
          end: event.end,
          title: event.title,
          color: { primary: event.color, secondary: event.color },
          actions: null,
          resizable: {
            beforeStart: event.isEditable,
            afterEnd: event.isEditable
          },
          draggable: event.isEditable,
          allDay: event.allDay != null ? event.allDay : false,
          meta: {
            id: event.id,
            typeId: event.typeId,
            showOptions: false,
            isEditable: event.isEditable,
            isViewable: event.isViewable,
            isDeletable: event.isDeletable,
            isCompletable: event.isCompletable,
            selectOnTileClick: event.selectOnTileClick
          },
          cssClass: event.cssClass
        };
        this.calendarEvents.push(newEvent);
        this.latestFilteredCalendarEvents.push(event);
      });
      if (this.filterSegmentIndex >= 0) {
        this.updateSelectedEventTypes();
      }
    } else {
      this.calendarEvents = [];
      this.filteredCalendarEvents
        .filter((t) => this.selectedEventTypes.includes(t.typeId))
        .forEach((event) => {
          const newEvent: CalendarEvent = {
            start: event.start,
            end: event.end,
            title: event.title,
            color: { primary: event.color, secondary: event.color },
            actions: null,
            resizable: {
              beforeStart: event.isEditable,
              afterEnd: event.isEditable
            },
            draggable: event.isEditable,
            allDay: event.allDay != null ? event.allDay : false,
            meta: {
              id: event.id,
              typeId: event.typeId,
              showOptions: false,
              isEditable: event.isEditable,
              isViewable: event.isViewable,
              isDeletable: event.isDeletable,
              isCompletable: event.isCompletable,
              selectOnTileClick: event.selectOnTileClick,
              isShowButton: event.isShowButton,
              buttonText: event.buttonText,
              buttonTooltip: event.buttonTooltip
            },
            cssClass: event.cssClass
          };
          this.calendarEvents.push(newEvent);
        });
    }
  }

  private getCurrentWeek(date: Date): [Date, Date] {
    const first = date.getDate() - date.getDay();
    const last = first + 6;
    const firstDay = AmcsDate.createFor(date.getFullYear(), date.getMonth(), first);
    let lastDay = AmcsDate.createFor(date.getFullYear(), date.getMonth(), last);
    lastDay = new Date(AmcsDate.createFrom(lastDay).setHours(23, 59, 59));
    return [firstDay, lastDay];
  }

  private updateSelectedEventTypes() {
    if (
      this.filteredCalendarEvents.length > 0 &&
      this.filterSegmentIndex >= 0 &&
      typeof this.filteredCalendarEvents[0][this.filteredEventTypesList[this.filterSegmentIndex][0].filterType] === 'boolean'
    ) {
      if (this.latestFilteredCalendarEvents.length > 0) {
        this.selectedEventTypes = [];
        for (let i = 0; i < this.filteredEventTypesList.length; i++) {
          for (let j = 0; j < this.filteredEventTypesList[i].length; j++) {
            if (
              this.latestFilteredCalendarEvents.some(
                (x) => +x[this.filteredEventTypesList[i][j].filterType] === this.filteredEventTypesList[i][j].id
              )
            ) {
              this.selectedEventTypes.push(+('9' + i + this.filteredEventTypesList[i][j].id));
            }
          }
        }
      }
    } else {
      this.selectedEventTypes = [];
      for (let i = 0; i < this.filteredEventTypesList.length; i++) {
        for (let j = 0; j < this.filteredEventTypesList[i].length; j++) {
          if (
            this.latestFilteredCalendarEvents.some(
              (x) => +x[this.filteredEventTypesList[i][0].filterType] === this.filteredEventTypesList[i][j].id
            )
          ) {
            this.selectedEventTypes.push(+('9' + i + this.filteredEventTypesList[i][j].id));
          }
        }
      }
    }
    if (this.filteredEventTypesList.length > 0 && this.filterPersist && this.enablePersist) {
      this.persistEventTypes = this.selectedEventTypes;
      this.isPersistSet = true;
    }
  }

  private updatePersistEventTypes() {
    for (let i = 0; i < this.filteredEventTypesList.length; i++) {
      for (let j = 0; j < this.filteredEventTypesList[i].length; j++) {
        const val = +('9' + i + this.filteredEventTypesList[i][j].id);
        if (this.persistEventTypes.includes(val)) {
          if (!this.selectedEventTypes.includes(val)) {
            this.selectedEventTypes.push(val);
          }
        } else {
          if (this.selectedEventTypes.includes(val)) {
            const index = this.selectedEventTypes.indexOf(val);
            if (index !== -1) {
              this.selectedEventTypes.splice(index, 1);
            }
          }
        }
      }
    }
  }

  private updateEventsBasedOnFilter() {
    let updatedEvents = [];
    let firstLoop = true;
    const columnList = [],
      resultArray = [];
    let tempArray = [];
    for (let i = 0; i < this.persistEventTypes.length; i++) {
      const filterType = this.filteredEventTypesList[this.persistEventTypes[i].toString()[1]][0];
      if (filterType && !columnList.includes(filterType.filterType)) {
        columnList.push(filterType.filterType);
      }

      const val = +this.persistEventTypes[i].toString().substring(2);
      const filterIndex = +this.persistEventTypes[i].toString()[1];
      if (
        (i !== this.persistEventTypes.length - 1 &&
          this.persistEventTypes[i + 1] &&
          filterIndex !== +this.persistEventTypes[i + 1].toString()[1]) ||
        (i === this.persistEventTypes.length - 1 &&
          this.persistEventTypes[i - 1] &&
          filterIndex === +this.persistEventTypes[i - 1].toString()[1]) ||
        (i === this.persistEventTypes.length - 1 &&
          this.persistEventTypes[i - 1] &&
          filterIndex !== +this.persistEventTypes[i - 1].toString()[1])
      ) {
        tempArray.push(val);
        resultArray.push(tempArray);
        tempArray = [];
      } else if (
        i !== this.persistEventTypes.length - 1 &&
        this.persistEventTypes[i + 1] &&
        filterIndex === +this.persistEventTypes[i + 1].toString()[1]
      ) {
        tempArray.push(val);
      }
    }

    for (let a = 0; a < columnList.length; a++) {
      if (columnList[a]) {
        const loopArray = firstLoop ? this.filteredCalendarEvents : updatedEvents;
        updatedEvents = loopArray.filter((x) => resultArray[a].includes(x[columnList[a]]));
        firstLoop = false;
      }
    }

    this.filteredCalendarEvents = updatedEvents;
  }

  private isDateValidForExcludeDays(date: Date): boolean {
    return !this.excludeDays?.some((x) => x === date.getDay());
  }

  private updateViewDateBasedOnExcludeDays(date: Date) {
    if (isTruthy(date)) {
      // If date is not valid, set to next valid date
      const updatedViewDate = date;
      let index = 1;
      while (!this.isDateValidForExcludeDays(updatedViewDate) && index <= 7) {
        updatedViewDate.setDate(date.getDate() + index);
        index++;
      }

      if (updatedViewDate !== date) {
        this.viewDate = updatedViewDate;
      }
    }
  }
}
