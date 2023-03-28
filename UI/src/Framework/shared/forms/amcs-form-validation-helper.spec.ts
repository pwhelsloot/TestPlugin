import { FormControl, Validators } from '@angular/forms';
import { AmcsFormValidationHelper } from './amcs-form-validation-helper';

describe('AmcsFormValidationHelper', () => {
  let service: AmcsFormValidationHelper;

  beforeEach(() => {
    service = new AmcsFormValidationHelper();
  });

  it('can create new validation helper', () => {
    expect(service).toBeTruthy();
  });

  describe('formControlHasError', () => {
    it('returns false when control is undefined', () => {
      const control = undefined;

      const result = AmcsFormValidationHelper.formControlHasError(control);

      expect(result).toBeFalse();
    });

    it('returns false when invalid control is provided but not touched by the user', () => {
      // no value and required makes an invalid control
      const control = new FormControl(undefined, Validators.required);
      control.updateValueAndValidity();

      const result = AmcsFormValidationHelper.formControlHasError(control);

      expect(result).toBeFalse();
    });

    it('returns false when valid control is provided but not touched by the user', () => {
      // no value and required makes an invalid control
      const control = new FormControl('SomeText', Validators.required);
      control.updateValueAndValidity();

      const result = AmcsFormValidationHelper.formControlHasError(control);

      expect(result).toBeFalse();
    });

    it('returns true when invalid control is provided and has been touched by the user', () => {
      // no value and required makes an invalid control
      const control = new FormControl(undefined, Validators.required);
      control.markAsTouched();
      control.updateValueAndValidity();

      const result = AmcsFormValidationHelper.formControlHasError(control);

      expect(result).toBeTrue();
    });
  });
});
