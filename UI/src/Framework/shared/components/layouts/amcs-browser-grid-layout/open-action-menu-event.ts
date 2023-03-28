import { AmcsDropdownOpenChange } from '../../amcs-dropdown/amcs-dropdown-open-change.model';

export class OpenActionMenuEvent<T = any> {
  /**
   * Original drop down open change event
   *
   * @type {AmcsDropdownOpenChange}
   * @memberof OpenActionEvent
   */
  event: AmcsDropdownOpenChange;

  /**
   * data related to row
   *
   * @memberof OpenActionEvent
   */
  row: T;
}
