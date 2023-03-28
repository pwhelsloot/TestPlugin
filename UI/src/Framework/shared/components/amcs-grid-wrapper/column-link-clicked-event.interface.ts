import { ITableItem } from '@shared-module/models/itable-item.model';

/**
 * used by amcs-grid event when a column of link type is clicked
 *
 * @export
 * @class IColumnLinkClickedEvent
 */
export interface IColumnLinkClickedEvent {
  key: string;
  item: ITableItem;
}
