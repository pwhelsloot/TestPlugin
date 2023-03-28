import { Component, HostBinding, Input } from '@angular/core';

/**
 * Use this wrap up one or multiple controls in a form
 */
@Component({
  selector: 'app-amcs-form-group',
  templateUrl: './amcs-form-group.component.html',
  styleUrls: ['./amcs-form-group.component.scss'],
  // Disabling no-host-metadata rule here, we want this component to always have this class applied.
  // tslint:disable: no-host-metadata-property
  // eslint-disable-next-line @angular-eslint/no-host-metadata-property
  host: { class: 'form-group' },
})
export class AmcsFormGroupComponent {
  /**
   * Enable to activate the error state
   */
  @HostBinding('class.has-error')
  @Input()
  hasError = false;

  /**
   * Enable to activate the success state
   */
  @HostBinding('class.has-success')
  @Input()
  hasSuccess = false;

  /**
   * Enable to make this form group inline
   */
  @HostBinding('class.form-md-line-input')
  @Input()
  inline = false;

  /**
   * Enable the actions class
   */
  @HostBinding('class.actions')
  @Input()
  hasActions = false;
}
