import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { CalendarEvent, WeekViewAllDayEvent } from 'calendar-utils';

@Component({
  selector: 'app-amcs-calendar-week-event',
  templateUrl: './amcs-calendar-week-event.component.html',
  styleUrls: ['./amcs-calendar-week-event.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AmcsCalendarWeekEventComponent extends AutomationLocatorDirective {
  @Input('weekEvent') weekEvent: WeekViewAllDayEvent;
  @Input('tooltipPlacement') tooltipPlacement = 'bottom';

  @Output('activitySelected') activitySelected: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityEdited') activityEdited: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityDeleted') activityDeleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityCompleted') activityCompleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();

  showOptions = false;

  showstatus(event: CalendarEvent) {
    this.showOptions = true;
  }

  hideStatus(event: CalendarEvent) {
    this.showOptions = false;
  }
}
