import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { ICalendarEvent } from '@shared-module/models/icalendar-event.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { useArgs } from '@storybook/client-api';
import { AmcsCalendarComponent } from './amcs-calendar.component';

let eventsLoading = false;
const calendarEventTypes = [
  {
    id: 1,
    description: 'Appointment',
    color: 'red'
  },
  {
    id: 2,
    description: 'Training',
    color: 'green'
  },
  {
    id: 3,
    description: 'Reminder',
    color: 'yellow'
  }
];
const generateEvent = (eventTitle: string, typeId: number, startOffset = 0, endOffset = 0): ICalendarEvent => {
  const startDay = AmcsDate.addDays(AmcsDate.create(), startOffset);
  const endDay = AmcsDate.addDays(AmcsDate.create(), endOffset);
  const type = calendarEventTypes.find((eventType) => eventType.id === typeId);

  return {
    id: 1,
    start: startDay,
    end: endDay,
    title: eventTitle,
    typeId: type.id,
    type: type.description,
    color: type.color,
    allDay: true,
    isEditable: true,
    isViewable: true,
    isDeletable: true,
    isCompletable: true,
    eventDate: startDay,
    getDescription: () => {}
  };
};

const calendarEvents: ICalendarEvent[] = [
  generateEvent('Doctors Appointment', 1),
  generateEvent('Football Training', 2, -1),
  generateEvent('Reminder', 3),
  generateEvent('Reminder',3),
  generateEvent('Gym Workout',2, -1, 4)
];
export default {
  title: StorybookGroupTitles.Data + 'Calendar',
  component: AmcsCalendarComponent,

  args: {
    events: [],
    initialView: 'month'
  },
  parameters: {
    docs: {
      description: {
        component: 'I am AMCS-Calendar component. I show events on a monthly, weekly or daily view, which can be filtered by type.'
      }
    }
  },
  decorators: [generateModuleMetaDataForStorybook(AmcsCalendarComponent, [SharedTranslationsService])]
} as Meta<AmcsCalendarComponent>;

const Template: Story<AmcsCalendarComponent> = (args) => {
  const [, updateArgs] = useArgs();
  //There are problems with calendar event detection, this is a workaround
  const onViewChanged = () => {
    if (!eventsLoading) {
      eventsLoading = true;
      setTimeout(() => {
        updateArgs({ events: [] });
      }, 0);
      setTimeout(() => {
        updateArgs({ events: calendarEvents });
      }, 0);
      setTimeout(() => {
        eventsLoading = false;
      }, 0);
    }
  };
  return {
    component: AmcsCalendarComponent,
    props: {
      compName: 'CalendarDatesDesignsComponent',
      events: args.events,
      eventTypes: calendarEventTypes,
      initialView: args.initialView,
      viewChanged: onViewChanged
    }
  };
};
export const Primary = Template.bind({});
Primary.args = { ...Primary.args };
