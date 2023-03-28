import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy,
  OnInit,
  Output,
  Renderer2,
  SimpleChanges,
  ViewEncapsulation
} from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { CalendarEvent, MonthViewDay } from 'calendar-utils';
import { isSameDay, isSameMonth } from 'date-fns';
import { Subscription } from 'rxjs';
import { AmcsCalendarMultiDayService } from '../amcs-calendar-multi-day.service';

@Component({
  selector: 'app-amcs-calendar-desktop-cell',
  templateUrl: './amcs-calendar-desktop-cell.component.html',
  styleUrls: ['./amcs-calendar-desktop-cell.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AmcsCalendarDesktopCellComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {
  @Input('day') day: MonthViewDay;
  @Input('viewDate') viewDate: Date;
  @Input('locale') locale: string;
  @Input('isActiveDayIsOpen') isActiveDayIsOpen = false;
  @Input('tooltipPlacement') tooltipPlacement = 'bottom';
  @Input('widthToggle') widthToggle: boolean;

  @Output('seeMoreClicked') seeMoreClicked: EventEmitter<MonthViewDay> = new EventEmitter<MonthViewDay>();
  @Output('activitySelected') activitySelected: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityEdited') activityEdited: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityDeleted') activityDeleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityCompleted') activityCompleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();

  hideCell = false;
  hiddenEventsTotal = 0;
  displayEvents: CalendarEvent[] = [];
  hasMoreEvents = false;
  showOptions = false;
  multidayHoverHighlight: boolean;
  multiDayHoverTrigger: boolean;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, private calendarMultidayService: AmcsCalendarMultiDayService) {
    super(elRef, renderer);
  }

  private multidayEventIndexes: number[];
  private multiDayHoverTriggerSubscription: Subscription;
  ngOnInit() {
    this.populateDayEvents();
    this.multiDayHoverTriggerSubscription = this.calendarMultidayService.isMultidayHovered.subscribe(() => {
      this.multidayHighlightHandler();
    });
    // If outside of month show blank cell
    if (!isSameMonth(this.day.date, this.viewDate)) {
      this.hideCell = true;
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    const dayChanges = changes['day'];
    if (isTruthy(dayChanges) && dayChanges.currentValue && dayChanges.currentValue.events && dayChanges.currentValue.events.length >= 0) {
      this.hasMoreEvents = false;
      this.populateDayEvents();
      this.setIsMultiDay();
    }
  }

  ngOnDestroy(): void {
    this.multiDayHoverTriggerSubscription?.unsubscribe();
  }

  seeMore() {
    this.seeMoreClicked.emit(this.day);
  }

  showstatus(event: CalendarEvent) {
    this.showOptions = true;
    this.multidayHoverHandler(event, true);
  }

  hideStatus(event: CalendarEvent) {
    this.showOptions = false;
    this.multidayHoverHandler(event, false);
  }

  private setIsMultiDay() {
    this.multidayEventIndexes = [];
    this.day.events.forEach((event, index) => {
      if (!isSameDay(event.start, event.end)) {
        event.meta.isMultiDay = true;
        event.meta.tooltipText = `${event.title} ${event.start.toDateString()} - ${event.end.toDateString()}`;
        this.multidayEventIndexes.push(index);
      } else {
        event.meta.isMultiDay = false;
        event.meta.tooltipText = event.title;
      }
    });
  }

  private multidayHighlightHandler() {
    this.multidayHoverHighlight = false;
    this.multidayEventIndexes.forEach((index) => {
      if (this.day.events[index].meta.isMultiDayHovered) {
        this.multidayHoverHighlight = true;
      }
    });
  }

  private multidayHoverHandler(event: CalendarEvent, hoverState: boolean) {
    if (event.meta.isMultiDay) {
      this.multidayEventIndexes.forEach((index) => {
        if (this.day.events[index].meta.id === event.meta.id) {
          this.day.events[index].meta.isMultiDayHovered = hoverState;
        }
      });
      this.calendarMultidayService.onMultidayHoverTrigger(hoverState);
    }
  }

  private populateDayEvents() {
    if (this.day.events != null) {
      this.displayEvents = this.day.events.slice();
      // If > 4 events only display 3 and a see more button
      if (this.displayEvents.length > 4) {
        this.hasMoreEvents = true;
        this.displayEvents = this.displayEvents.slice(0, 3);
        this.hiddenEventsTotal = this.day.events.length - 3;
      }
    }
  }
}
