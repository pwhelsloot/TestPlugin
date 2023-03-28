/**
 * Used by the grid-wrapper to generate the default actionColumnTemplate
 *
 * @export
 * @class ActionColumnButton
 */
export class ActionColumnButton {
  id?: string | number;
  action?: string;
  link?: string;
  description?: string;
  isDisabled = false;
  icon?: string;
}
