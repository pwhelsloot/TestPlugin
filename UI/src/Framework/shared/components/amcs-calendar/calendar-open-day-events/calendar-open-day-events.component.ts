import { Component, ElementRef, EventEmitter, Input, OnChanges, Output, Renderer2, SimpleChanges } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { CalendarEvent, MonthViewDay } from 'calendar-utils';
import { AmcsCalendarMultiDayService } from '../amcs-calendar-multi-day.service';

@Component({
  selector: 'app-calendar-open-day-events',
  templateUrl: './calendar-open-day-events.component.html',
  styleUrls: ['./calendar-open-day-events.component.scss']
})
export class CalendarOpenDayEventsComponent extends AutomationLocatorDirective implements OnChanges {
  @Input('day') day: Partial<MonthViewDay>;
  @Input('locale') locale: string;
  @Input('isOpen') isOpen: boolean;

  @Output('activitySelected') activitySelected: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityEdited') activityEdited: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityDeleted') activityDeleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityCompleted') activityCompleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();

  events: CalendarEvent[] = [];
  constructor(protected elRef: ElementRef, protected renderer: Renderer2, private calendarMultidayService: AmcsCalendarMultiDayService) {
    super(elRef, renderer);
  }

  ngOnChanges(simpleChanges: SimpleChanges) {
    if (
      simpleChanges['day'] &&
      isTruthy(simpleChanges['day'].currentValue) &&
      simpleChanges['day'].currentValue !== simpleChanges['day'].previousValue
    ) {
      this.parseDayEvents(simpleChanges['day'].currentValue);
    }
  }

  showstatus(event: CalendarEvent) {
    event.meta.showOptions = true;
    this.multidayHoverHandler(event, true);
  }

  hideStatus(event: CalendarEvent) {
    event.meta.showOptions = false;
    this.multidayHoverHandler(event, false);
  }

  private multidayHoverHandler(event, hoverState) {
    if (event.meta.isMultiDay) {
      this.day.events
        .filter((dayEvent) => dayEvent.meta.id === event.meta.id)
        .forEach((dayEvent) => (dayEvent.meta.isMultiDayHovered = hoverState));
      this.calendarMultidayService.onMultidayHoverTrigger(hoverState);
    }
  }

  private parseDayEvents(day: Partial<MonthViewDay>) {
    this.events = [];

    // This seems dumb, but we have to make a clone of the calendar object we pass in, otherwise we'll show item options
    // for both the calendar cell item AND the expanded list item on mouse hover
    const copy: Partial<MonthViewDay> = JSON.parse(JSON.stringify(day));
    this.events = copy.events;
  }
}
