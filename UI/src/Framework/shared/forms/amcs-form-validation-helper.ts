import { FormControl } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsFormControlStatus } from './amcs-form-control-status';

/**
 * Helpers for validating form's and form controls
 */
export class AmcsFormValidationHelper {
  /**
   * Check this FormControl for errors
   * Taken from BaseFormGroup hasErrors()
   * @param control FormControl to check for error state
   * @returns True if FormControl has errors
   */
  static formControlHasError(control: FormControl) {
    if (control === undefined) {
      return false;
    }

    let hasErrors = control.invalid && control.touched;
    if (
      control.status === AmcsFormControlStatus.DISABLED &&
      !control.value &&
      control.touched &&
      isTruthy(control.errors) &&
      control.errors.required
    ) {
      hasErrors = !control.value;
    }
    return hasErrors;
  }
}
