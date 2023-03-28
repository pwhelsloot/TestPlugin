export class AmcsDatepickerConfig {
    /** min date selectable */
    minDate?: Date;
    /** max date selectable */
    maxDate?: Date;
    /** custom class for container */
    containerClass?: string;
    /** date input format accepted */
    dateInputFormat?: string;
    /** locale of datepicker */
    locale?: string;

    daysDisabled?: number[];

    selectFromOtherMonth?: boolean;
}
