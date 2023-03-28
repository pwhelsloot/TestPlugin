import { ActionColumnButton } from '../../amcs-grid-action-column/amcs-grid-action-button';

export class MenuItemClickedEvent<T = any> {
  row: T;
  action: ActionColumnButton;
}
