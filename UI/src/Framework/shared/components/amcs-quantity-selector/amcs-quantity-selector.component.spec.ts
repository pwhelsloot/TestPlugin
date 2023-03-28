import { ElementRef, Renderer2 } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ControlContainer } from '@angular/forms';
import { By } from '@angular/platform-browser';
import { MockProvider } from 'ng-mocks';
import { AmcsQuantitySelectorComponent } from './amcs-quantity-selector.component';

describe('AmcsQuantitySelectorComponent', () => {
  let component: AmcsQuantitySelectorComponent;
  let fixture: ComponentFixture<AmcsQuantitySelectorComponent>;
  let minValue = -5;
  let defaultValue = 0;
  let maxValue = 5;
  let passedValue: number;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsQuantitySelectorComponent],
      providers: [MockProvider(ElementRef), MockProvider(Renderer2), MockProvider(ControlContainer)],
    }).compileComponents();

    fixture = TestBed.createComponent(AmcsQuantitySelectorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();

    component.minValue = minValue;
    component.value = defaultValue;
    component.maxValue = maxValue;
  });

  beforeEach(() => {});

  it('should create', () => {
    //arrange

    //act

    //assert
    expect(component).toBeTruthy();
  });

  describe('html buttons test', () => {
    it('clicking minus button should call decreaseQty, but not increaseQty', () => {
      //arrange
      spyOn(component, 'increaseQty');
      spyOn(component, 'decreaseQty');
      const button = fixture.debugElement.query(By.css('.btn.btn-default.minus'));

      //act
      button.triggerEventHandler('click', null);

      //assert
      expect(component.decreaseQty).toHaveBeenCalled();
      expect(component.increaseQty).not.toHaveBeenCalled();
    });

    it('clicking plus button should call increaseQty, but not decreaseQty', () => {
      //arrange
      spyOn(component, 'increaseQty');
      spyOn(component, 'decreaseQty');
      const button = fixture.debugElement.query(By.css('.btn.btn-default.plus'));

      //act
      button.triggerEventHandler('click', null);

      //assert
      expect(component.decreaseQty).not.toHaveBeenCalled();
      expect(component.increaseQty).toHaveBeenCalled();
    });
  });

  it('should value to passed value and call validateValue and onChangeCallback once with passedValue', () => {
    //arrange
    passedValue = 1;
    spyOn(component, 'validateValue').and.callThrough();
    spyOn(component, 'onChangeCallback');

    //act
    component.writeValue(passedValue);

    //assert
    expect(component.validateValue).toHaveBeenCalledOnceWith(passedValue);
    expect(component.onChangeCallback).toHaveBeenCalledOnceWith(passedValue);
    expect(component.value).toEqual(passedValue);
  });

  describe('validateValue', () => {
    it('should set edited to true and return a clamped numerical value if passed a number value', () => {
      //arrange
      passedValue = 1;
      spyOn(component, 'clampValue').and.callThrough();
      //act
      const result = component.validateValue(passedValue);

      //assert
      expect(component.edited).toBeTrue();
      expect(component.clampValue).toHaveBeenCalledOnceWith(passedValue);
      expect(result).toEqual(passedValue);
    });

    it('should set edited to false and return 0 if passed a NaN, null or undefined value', () => {
      //arrange
      //act
      let result = component.validateValue(NaN);

      //assert
      expect(component.edited).toBeFalse();
      expect(result).toEqual(0);

      //act
      result = component.validateValue(null);

      //assert
      expect(component.edited).toBeFalse();
      expect(result).toEqual(0);

      //act
      result = component.validateValue(undefined);

      //assert
      expect(component.edited).toBeFalse();
      expect(result).toEqual(0);
    });
  });

  describe('clampValue', () => {
    it('should return passed value and set maxLength to null if minValue and maxValue are not set', () => {
      //arrange
      component.maxValue = undefined;
      component.minValue = undefined;
      passedValue = 10;

      //act
      const result = component.clampValue(passedValue);

      //assert
      expect(result).toEqual(passedValue);
      expect(component.maxLength).toEqual(null);
    });

    it('should return passed value and set maxLength to null if it\'s below set maxValue, and minValue is not set ', () => {
      //arrange
      component.minValue = undefined;
      passedValue = -10;

      //act
      const result = component.clampValue(passedValue);

      //assert
      expect(result).toEqual(passedValue);
      expect(component.maxLength).toEqual(null);
    });

    it('should return passed value and set maxLength to null if it\'s above set minValue, and maxValue is not set ', () => {
      //arrange
      component.maxValue = undefined;
      passedValue = 30;

      //act
      const result = component.clampValue(passedValue);

      //assert
      expect(result).toEqual(passedValue);
      expect(component.maxLength).toEqual(null);
    });

    it('should return passed value and set maxLength to null if it\'s between set minValue and maxValue ', () => {
      //arrange
      passedValue = 2;

      //act
      const result = component.clampValue(passedValue);

      //assert
      expect(result).toEqual(passedValue);
      expect(component.maxLength).toEqual(null);
    });

    it('should return maxValue and set maxLength to number of digits in maxValue if passed value is above set maxValue ', () => {
      //arrange
      passedValue = 10;

      //act
      const result = component.clampValue(passedValue);

      //assert
      expect(result).toEqual(component.maxValue);
      expect(component.maxLength).toEqual(Math.abs(maxValue).toString.length);
    });

    it('should return minValue set maxLength to number of digits in minValue if passed value is below set minValue ', () => {
      //arrange
      component.minValue = 5;
      passedValue = 1;

      //act
      const result = component.clampValue(passedValue);

      //assert
      expect(result).toEqual(component.minValue);
      expect(component.maxLength).toEqual(Math.abs(minValue).toString.length);
    });
  });

  describe('numericalInputChanged', () => {
    it('should change value if numerical input is different', () => {
      //arrange
      passedValue = 4;
      spyOn(component, 'writeValue').and.callThrough();

      //act
      component.numericalInputChanged(passedValue);

      //assert
      expect(component.writeValue).toHaveBeenCalledOnceWith(passedValue);
      expect(component.value).toEqual(passedValue);
    });

    it('should not change value if numerical input is same', () => {
      //arrange
      passedValue = 0;
      spyOn(component, 'writeValue').and.callThrough();

      //act
      component.numericalInputChanged(passedValue);

      //assert
      expect(component.writeValue).not.toHaveBeenCalled();
      expect(component.value).toEqual(defaultValue);
    });
  });

  it('should set registerOnChange to passed parameter', () => {
    //arrange
    const testParam = 'test';

    //act
    component.registerOnChange(testParam);

    //assert
    expect(component.onChangeCallback).toEqual(testParam);
  });

  it('should set registerOnTouched to passed parameter', () => {
    //arrange
    const testParam = 'test';

    //act
    component.registerOnTouched(testParam);

    //assert
    expect(component.onTouchedCallback).toEqual(testParam);
  });

  describe('setDisabledState', () => {
    it('should change isDisabled from false to true', () => {
      //arrange
      component.isDisabled = false;

      //act
      component.setDisabledState(true);

      //assert
      expect(component.isDisabled).toBeTrue();
    });

    it('should change isDisabled from true to false', () => {
      //arrange
      component.isDisabled = true;

      //act
      component.setDisabledState(false);

      //assert
      expect(component.isDisabled).toBeFalse();
    });
  });

  it('onFocus should call focus.emit once with its parameter', () => {
    //arrange
    spyOn(component.focus, 'next');

    //act
    component.onFocus(null);

    //assert
    expect(component.focus.next).toHaveBeenCalledOnceWith(null);
  });

  it('onBlur should call change.emit once with its parameter', () => {
    //arrange
    const testParam = 'test';
    spyOn(component.blur, 'next');
    spyOn(component, 'onTouchedCallback');

    //act
    component.onBlur(testParam);

    //assert
    expect(component.blur.next).toHaveBeenCalledOnceWith(testParam);
    expect(component.onTouchedCallback).toHaveBeenCalledOnceWith();
  });

  it('onChange should call change.emit once with its parameter', () => {
    //arrange
    const testParam = 'test';
    spyOn(component.change, 'emit');

    //act
    component.onChange(testParam);

    //assert
    expect(component.change.emit).toHaveBeenCalledOnceWith(testParam);
  });

  describe('decreaseQty', () => {
    it('decreaseQty without minValue should call writeValue once', () => {
      //arrange
      component.minValue = undefined;
      spyOn(component, 'writeValue');

      //act
      component.decreaseQty(null);

      //assert
      expect(component.writeValue).toHaveBeenCalledOnceWith(-1);
    });

    it('decreaseQty above minValue should call writeValue once', () => {
      //arrange
      spyOn(component, 'writeValue');

      //act
      component.decreaseQty(null);

      //assert
      expect(component.writeValue).toHaveBeenCalledOnceWith(-1);
    });

    it('decreaseQty below minValue should not call writeValue', () => {
      //arrange
      component.minValue = 0;
      spyOn(component, 'writeValue');

      //act
      component.decreaseQty(null);

      //assert
      expect(component.writeValue).not.toHaveBeenCalled();
    });
  });

  describe('increaseQty', () => {
    it('increaseQty without maxValue should call writeValue once', () => {
      //arrange
      component.maxValue = undefined;
      spyOn(component, 'writeValue');

      //act
      component.increaseQty(null);

      //assert
      expect(component.writeValue).toHaveBeenCalledOnceWith(1);
    });

    it('increaseQty below maxValue should call writeValue once', () => {
      //arrange
      spyOn(component, 'writeValue');

      //act
      component.increaseQty(null);

      //assert
      expect(component.writeValue).toHaveBeenCalledOnceWith(1);
    });

    it('increaseQty above maxValue should not call writeValue', () => {
      //arrange
      component.maxValue = 0;
      spyOn(component, 'writeValue');

      //act
      component.increaseQty(null);

      //assert
      expect(component.writeValue).not.toHaveBeenCalled();
    });
  });
});
