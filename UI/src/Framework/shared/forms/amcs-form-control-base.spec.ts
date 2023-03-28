import { Component, ElementRef, NO_ERRORS_SCHEMA, Renderer2 } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ControlContainer, FormControl, Validators } from '@angular/forms';
import { AmcsFormControlBaseComponent } from './amcs-form-control-base';

@Component({
  template: '',
})
export class DummyFormControlComponent extends AmcsFormControlBaseComponent {}

describe('AmcsFormControlBaseComponent', () => {
  let component: DummyFormControlComponent;
  let fixture: ComponentFixture<DummyFormControlComponent>;
  beforeEach(() => {
    const elementRefStub = () => ({});
    const renderer2Stub = () => ({});
    const controlContainerStub = () => ({});
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      declarations: [DummyFormControlComponent],
      providers: [
        { provide: ElementRef, useFactory: elementRefStub },
        { provide: Renderer2, useFactory: renderer2Stub },
        { provide: ControlContainer, useFactory: controlContainerStub },
      ],
    });
    fixture = TestBed.createComponent(DummyFormControlComponent);
    component = fixture.componentInstance;
  });

  it('can load instance', () => {
    expect(component).toBeTruthy();
  });

  it(`isDefaultLabel has default value`, () => {
    expect(component.isDefaultLabel).toEqual(false);
  });

  it(`useDefaultErrorState has default value`, () => {
    expect(component.useDefaultErrorState).toEqual(true);
  });

  describe('isRequired', () => {
    it('set isRequired with False returns false', () => {
      component.isRequired = false;

      expect(component.isRequired).toBeFalse();
    });

    it('set isRequired with true returns true', () => {
      component.isRequired = true;

      expect(component.isRequired).toBeTrue();
    });

    it('set isRequired with false as string  returns false', () => {
      component.isRequired = 'false';

      expect(component.isRequired).toBeFalse();
    });

    it('set isRequired with true as string  returns true', () => {
      component.isRequired = 'true';

      expect(component.isRequired).toBeTrue();
    });

    it('set isRequired with undefined as string  returns false', () => {
      component.isRequired = undefined;

      expect(component.isRequired).toBeFalse();
    });
  });

  describe('label', () => {
    it('sets label value', () => {
      const value = 'MyLabelvalue';
      component.label = value;

      expect(component.label).toBe(value);
    });

    it('sets label value', () => {
      spyOn(component, 'initLabel').and.callThrough();
      component.label = 'DoesNotMatter';

      expect(component.initLabel).toHaveBeenCalled();
    });
  });

  describe('ngAfterContentInit', () => {
    it('makes expected calls', () => {
      spyOn(component, 'initLabel').and.callThrough();

      component.ngAfterContentInit();

      expect(component.initLabel).toHaveBeenCalled();
    });
  });

  describe('initLabel', () => {
    it('makes expected calls', () => {
      spyOn(component, 'setUsingDefaultLabel').and.callThrough();
      spyOn(component, 'setIsRequired').and.callThrough();

      component.initLabel();

      expect(component.setUsingDefaultLabel).toHaveBeenCalled();
      expect(component.setIsRequired).toHaveBeenCalled();
    });
  });

  describe('setUsingDefaultLabel', () => {
    it('has not set label, default label will be false', () => {
      component.setUsingDefaultLabel();
      expect(component.isDefaultLabel).toBeFalse();
    });
    it('has set label, default label will be true', () => {
      component.label = 'DoesNotMatter';

      component.setUsingDefaultLabel();
      expect(component.isDefaultLabel).toBeTrue();
    });
  });

  describe('setIsRequired', () => {
    it('DefaultLabel is set, required has not been set manually, formcontrol exists, call hasRequiredValidator', () => {
      spyOn(component, 'hasRequiredValidator').and.callThrough();
      component.isDefaultLabel = true;
      const formControl = new FormControl();
      spyOn(component, 'getControlFromControlContainer').and.returnValue(formControl);
      component.setFormControl();

      component.setIsRequired();
      expect(component.hasRequiredValidator).toHaveBeenCalled();
    });

      it('DefaultLabel is set, required not has been set manually, form control does not exist, does not call hasRequiredValidator', () => {
      spyOn(component, 'hasRequiredValidator').and.callThrough();
      component.isDefaultLabel = true;

      component.setIsRequired();
      expect(component.hasRequiredValidator).not.toHaveBeenCalled();
    });

    it('DefaultLabel is set, required has been set manually, form control does not exist, does not call hasRequiredValidator', () => {
      spyOn(component, 'hasRequiredValidator').and.callThrough();
      component.isDefaultLabel = true;
      component.isRequired = true;

      component.setIsRequired();
      expect(component.hasRequiredValidator).not.toHaveBeenCalled();
    });

    it('DefaultLabel is set, required has been set manually, form control exists, does not call hasRequiredValidator', () => {
      spyOn(component, 'hasRequiredValidator').and.callThrough();
      component.isDefaultLabel = true;
      component.isRequired = true;
      const formControl = new FormControl();
      spyOn(component, 'getControlFromControlContainer').and.returnValue(formControl);
      component.setFormControl();

      component.setIsRequired();
      expect(component.hasRequiredValidator).not.toHaveBeenCalled();
    });

    it('isDefaultLabel is not set, required has not been set manually, form control does not exist, does not call hasRequiredValidator', () => {
      spyOn(component, 'hasRequiredValidator').and.callThrough();
      component.isDefaultLabel = false;

      component.setIsRequired();
      expect(component.hasRequiredValidator).not.toHaveBeenCalled();
    });

    it('isDefaultLabel is not set, required has not been set manually, form control exists, does not call hasRequiredValidator', () => {
      spyOn(component, 'hasRequiredValidator').and.callThrough();
      component.isDefaultLabel = false;
      const formControl = new FormControl();
      spyOn(component, 'getControlFromControlContainer').and.returnValue(formControl);
      component.setFormControl();

      component.setIsRequired();
      expect(component.hasRequiredValidator).not.toHaveBeenCalled();
    });
  });

  describe('hasRequiredValidator', () => {
    it('no control is provided, will return false', () => {
      const formControl = undefined;
      spyOn(component, 'getControlFromControlContainer').and.returnValue(formControl);
      component.setFormControl();

      const result = component.hasRequiredValidator();

      expect(result).toBeFalse();
    });

    it('control is provided, no validators, will return false', () => {
      const formControl = new FormControl();
      spyOn(component, 'getControlFromControlContainer').and.returnValue(formControl);
      component.setFormControl();

      const result = component.hasRequiredValidator();

      expect(result).toBeFalse();
    });

    it('control is provided with no value, has required validator, will return true', () => {
      const formControlValue = undefined;
      const formControl = new FormControl(formControlValue, Validators.required);
      component.formControl = formControl;

      const result = component.hasRequiredValidator();

      expect(result).toBeTrue();
    });

    it('control is provided with a value, has required validator, will return true', () => {
      const formControlValue = 'MyValue';
      const formControl = new FormControl(formControlValue, Validators.required);
      component.formControl = formControl;

      const result = component.hasRequiredValidator();

      expect(result).toBeTrue();
    });

    it('control is provided with a value, has a different validator, will return false', () => {
      const formControlValue = 'MyValue';
      const formControl = new FormControl(formControlValue, Validators.minLength(0));
      spyOn(component, 'getControlFromControlContainer').and.returnValue(formControl);
      component.setFormControl();

      const result = component.hasRequiredValidator();

      expect(result).toBeFalse();
    });
  });

  describe('getControl', () => {
    it('no formcontrol, no controlcontainer, returns undefined', () => {
      component.formControl = undefined;
      component.formControlName = undefined;
      spyOn(component, 'getControlFromControlContainer').and.returnValue(undefined);

      component.setFormControl();

      expect(component.formControl).toBeUndefined();
    });

    it('has formcontrol, no controlcontainer, returns formControl', () => {
      const formControl = new FormControl('MyValue');
      component.formControl = formControl;
      component.formControlName = undefined;

      component.setFormControl();
      const control = component.formControl;

      expect(control).toBe(formControl);
    });

    it('no formcontrol, has controlcontainer and formControlName, returns control', () => {
      const formControl = new FormControl('MyValue');

      spyOn(component, 'getControlFromControlContainer').and.returnValue(formControl);
      component.formControl = undefined;
      component.formControlName = 'MyControl';

      component.setFormControl();
      const control = component.formControl;

      expect(control).toBe(formControl);
    });

    it('no formcontrol, has controlcontainer and no formControlName, returns undefined', () => {
      const formControl = undefined;

      component.formControl = undefined;
      component.formControlName = undefined;

      component.setFormControl();
      const control = component.formControl;

      expect(control).toBe(formControl);
    });

    it('has formcontrol, has controlcontainer and no formControlName, returns the manually provided FormControl', () => {
      const manualProvidedFormControl = new FormControl('MyValue');
      const providedFormControl = new FormControl('MyValue');

      spyOn(component, 'getControlFromControlContainer').and.returnValue(providedFormControl);
      component.formControl = manualProvidedFormControl;
      component.formControlName = 'MyControl';

      component.setFormControl();
      const control = component.formControl;

      expect(control).toBe(manualProvidedFormControl);
    });
  });
});
