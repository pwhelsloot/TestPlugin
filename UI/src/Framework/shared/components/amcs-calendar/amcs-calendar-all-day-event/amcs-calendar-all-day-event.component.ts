import { Component, EventEmitter, Input, Output, ViewEncapsulation } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { CalendarEvent } from 'calendar-utils';

@Component({
  selector: 'app-amcs-calendar-all-day-event',
  templateUrl: './amcs-calendar-all-day-event.component.html',
  styleUrls: ['./amcs-calendar-all-day-event.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AmcsCalendarAllDayEventComponent extends AutomationLocatorDirective {

  @Input('event') event: CalendarEvent;
  @Input('tooltipPlacement') tooltipPlacement = 'bottom';

  @Output('activitySelected') activitySelected: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityEdited') activityEdited: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityDeleted') activityDeleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();
  @Output('activityCompleted') activityCompleted: EventEmitter<CalendarEvent> = new EventEmitter<CalendarEvent>();

  showstatus(event: CalendarEvent) {
    event.meta.showOptions = true;
  }

  hideStatus(event: CalendarEvent) {
    event.meta.showOptions = false;
  }
}
