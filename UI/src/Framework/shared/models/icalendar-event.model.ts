export interface ICalendarEvent {
  id: number;
  start: Date;
  end?: Date;
  title: string;
  typeId: number;
  type: string;
  color: string;
  allDay?: boolean;
  isEditable: boolean;
  isViewable: boolean;
  isDeletable: boolean;
  isCompletable: boolean;
  eventDate: Date;
  cssClass?: string;
  selectOnTileClick?: boolean;
  getDescription();
  isMultiDay?: boolean;
  isMultiDayHovered?: boolean;
  tooltipText?: string;
  isShowButton?: boolean;
  buttonText?: string;
  buttonTooltip?: string;
}
