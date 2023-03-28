import { TemplateRef, Type } from '@angular/core';
import { AmcsModalChildComponent } from '@shared-module/components/amcs-modal/amcs-modal-child-component.interface';
import { AmcsModalButton } from './amcs-modal-button.model';

export class AmcsModalConfig {
  // Either a component reference
  template: Type<AmcsModalChildComponent> | TemplateRef<any> | string;

  // Title of the dialog
  title?: string;

  // Button Text of the dialog
  buttonText?: string;

  // Is this a confirmation dialog or an alert?
  type?: 'confirmation' | 'alert' | 'save' | null;

  /** if true modal appears via animation  */
  animated?: boolean;

  /** custom class for modal body */
  class?: string;

  /** if true background click doesn't close modal */
  ignoreBackdropClick?: boolean;

  // Optional base portlet styling to use
  baseColor?: string;

  // If passing a AmcsModalChildComponent, this is how you'd send extra data to the component
  extraData?: any;

  // The width of the container
  largeSize?= false;

  hideButtons?: boolean;

  redirectUrlOnClosing?: string;

  /**
   * Width of the modal in px
   *
   * @type {string}
   * @memberof AmcsModalConfig
   */
  width?: string;

  icon?: string;

  autoFocus?= true;

  isMobile?= false;

  isError?= false;

  /** Additional buttons on title bar */
  buttons?: AmcsModalButton[];

  hideCloseButton?= false;
}
