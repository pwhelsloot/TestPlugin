import { ElementRef, Renderer2, SimpleChange, SimpleChanges } from '@angular/core';
import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { GlossaryService } from '@core-module/services/glossary/glossary.service';
import { GlossaryPipe } from '@shared-module/pipes/glossary.pipe';
import { MockProvider } from 'ng-mocks';

import { AmcsInputComponent } from './amcs-input.component';

describe('AmcsInputComponent', () => {
  let fixture: ComponentFixture<AmcsInputComponent>;
  let component: AmcsInputComponent;
  let mockGlossaryPipe: GlossaryPipe;
  let change: SimpleChanges;
  let preGlossaryTransformValue: string;
  let defaultValue: any = 'test';
  let event = new KeyboardEvent('keypress', {
    key: 'a'
  });

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [AmcsInputComponent],
        providers: [MockProvider(GlossaryService), MockProvider(GlossaryPipe), MockProvider(Renderer2)]
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsInputComponent);
    mockGlossaryPipe = TestBed.inject(GlossaryPipe);
    component = fixture.componentInstance;
    fixture.detectChanges();

    component.value = defaultValue;
    change = {};
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  describe('ngOnChanges', () => {
    it('should do nothing', () => {
      //arrange
      //@ts-ignore
      spyOn(component, 'transformValueUsingGlossary');
      //act
      component.ngOnChanges(change);
      //assert
      expect(component.value).toEqual(defaultValue);
      expect(component['transformValueUsingGlossary']).not.toHaveBeenCalled();
    });

    it('should call transformValueUsingGlossary and set value using glossaryPipe.transform', () => {
      //arrange
      component.isDisabled = true;
      change = { isReadOnly: new SimpleChange(0, 1, true), isDisabled: new SimpleChange(0, 1, true) };
      //act
      component.ngOnChanges(change);
      //assert
      expect(component.value).toEqual(mockGlossaryPipe.transform(defaultValue));
    });

    it('should call transformValueUsingGlossary and set value to preGlossaryTransformValue', () => {
      //arrange
      change = { isReadOnly: new SimpleChange(0, 1, true), isDisabled: new SimpleChange(0, 1, true) };
      component.isDisabled = false;
      component.isReadOnly = false;
      component.value = defaultValue;
      //act
      component.ngOnChanges(change);
      //assert
      expect(component.value).toEqual(preGlossaryTransformValue);
    });
  });

  describe('ngAfterViewInit', () => {
    const mockCustomClass = 'a b c';

    it('should call renderer.addClass once for every custom class with defaultInput', () => {
      //arrange
      component.customClass = mockCustomClass;
      spyOn(component['renderer'], 'addClass');

      //act
      component.ngAfterViewInit();

      // assert
      expect(component['renderer'].addClass).toHaveBeenCalledTimes(mockCustomClass.split(' ').length);
      mockCustomClass.split(' ').forEach((mockSubclass) => {
        expect(component['renderer'].addClass).toHaveBeenCalledWith(component.defaultInput.nativeElement, mockSubclass);
      });
    });

    it('should call renderer.addClass once for every custom class with checkboxInput', () => {
      //arrange
      component.type = 'checkbox';
      component.customClass = mockCustomClass;
      component.checkboxInput = new ElementRef(null); //not being set by default
      spyOn(component['renderer'], 'addClass');

      //act
      component.ngAfterViewInit();

      // assert
      expect(component['renderer'].addClass).toHaveBeenCalledTimes(mockCustomClass.split(' ').length);
      mockCustomClass.split(' ').forEach((mockSubclass) => {
        expect(component['renderer'].addClass).toHaveBeenCalledWith(component.checkboxInput.nativeElement, mockSubclass);
      });
    });

    it('should call renderer.addClass once for every custom class wrapper with wrapperElement', () => {
      //arrange
      component.customWrapperClass = mockCustomClass;
      spyOn(component['renderer'], 'addClass');

      //act
      component.ngAfterViewInit();

      // assert
      expect(component['renderer'].addClass).toHaveBeenCalledTimes(mockCustomClass.split(' ').length);
      mockCustomClass.split(' ').forEach((mockSubclass) => {
        expect(component['renderer'].addClass).toHaveBeenCalledWith(component.wrapperElement.nativeElement, mockSubclass);
      });
    });

    it('should do nothing by default', () => {
      //arrange
      spyOn(component['renderer'], 'addClass');
      //act
      component.ngAfterViewInit();
      //assert
      expect(component['renderer'].addClass).not.toHaveBeenCalled();
    });
  });

  describe('writeValue', () => {
    beforeEach(() => {
      component.value = null;
      component.edited = false;
    });
    it('should reset value, preGlossaryTransformValue and edited and call onChangeCallback with null', () => {
      //arrange
      component.edited = true;
      spyOn(component, 'onChangeCallback');
      //@ts-ignore
      spyOn(component, 'writeNullValue').and.callThrough();

      //act
      component.writeValue(null);

      // assert
      expect(component['writeNullValue']).toHaveBeenCalledOnceWith();
      expect(component.value).toBeNull();
      expect(component.edited).toBeFalse();
      expect(component['preGlossaryTransformValue']).toBeNull();
      expect(component.onChangeCallback).toHaveBeenCalledWith(null);
    });
  });

  it('should set value to numeric value and call onChangeCallback with value and onTouchedCallback once', () => {
    //arrange
    defaultValue = '1';
    component.type = 'number';
    spyOn(component, 'onChangeCallback');
    spyOn(component, 'onTouchedCallback');

    //act
    component.writeValue(defaultValue);
    // assert
    expect(component.value).toEqual(Number(defaultValue));
    expect(component.edited).toBeTrue();
    expect(component.onChangeCallback).toHaveBeenCalledOnceWith(Number(defaultValue));
    expect(component.onTouchedCallback).toHaveBeenCalledOnceWith();
  });

  it('should not change value, but should change edited to true, set preGlossaryTransformValue to defaul call onChangeCallback with default value and onTouchedCallback once', () => {
    //arrange
    component.type = 'text';
    spyOn(component, 'onChangeCallback');
    spyOn(component, 'onTouchedCallback');
    //@ts-ignore
    spyOn(component, 'transformValueUsingGlossary');

    //act
    component.writeValue(defaultValue);
    // assert
    expect(component.value).toEqual(defaultValue);
    expect(component['preGlossaryTransformValue']).toEqual(defaultValue);
    expect(component.edited).toBeTrue();
    expect(component['transformValueUsingGlossary']).toHaveBeenCalledOnceWith();
    expect(component.onChangeCallback).toHaveBeenCalledOnceWith(defaultValue);
    expect(component.onTouchedCallback).toHaveBeenCalledOnceWith();
  });

  describe('setDisabledState', () => {
    it('should set isDisabled to false and call transformValueUsingGlossary', () => {
      //arrange
      //@ts-ignore
      spyOn(component, 'transformValueUsingGlossary');
      //act
      component.setDisabledState(false);
      //assert
      expect(component.isDisabled).toEqual(false);
      expect(component['transformValueUsingGlossary']).toHaveBeenCalledOnceWith();
    });

    it('should set isDisabled to true and call transformValueUsingGlossary', () => {
      //arrange
      //@ts-ignore
      spyOn(component, 'transformValueUsingGlossary');
      //act
      component.setDisabledState(true);
      //assert
      expect(component.isDisabled).toEqual(true);
      expect(component['transformValueUsingGlossary']).toHaveBeenCalledOnceWith();
    });
  });

  describe('support methods', () => {
    const testParam = 'test';

    it('should set registerOnChange to passed parameter', () => {
      //arrange

      //act
      component.registerOnChange(testParam);

      //assert
      expect(component.onChangeCallback).toEqual(testParam);
    });

    it('should set registerOnTouched to passed parameter', () => {
      //arrange

      //act
      component.registerOnTouched(testParam);

      //assert
      expect(component.onTouchedCallback).toEqual(testParam);
    });

    it('onChange should call change.emit once with passed parameter', () => {
      //arrange
      spyOn(component.change, 'next');

      //act
      component.onChange(testParam);

      //assert
      expect(component.change.next).toHaveBeenCalledOnceWith(testParam);
    });
  });

  describe('onFocus', () => {
    const mockFocusEvent = { target: { select: () => {} } };

    it('should call focus.emit once with passed parameter and call target.select once', () => {
      //arrange
      component.selectAllOnFocus = true;
      spyOn(component.focus, 'next');
      spyOn(mockFocusEvent.target, 'select');

      //act
      component.onFocus(mockFocusEvent);

      //assert
      expect(component.focus.next).toHaveBeenCalledOnceWith(mockFocusEvent);
      expect(mockFocusEvent.target.select).toHaveBeenCalledOnceWith();
    });

    it('should just call focus.emit once with passed parameter', () => {
      //arrange
      component.selectAllOnFocus = false;
      spyOn(component.focus, 'next');
      spyOn(mockFocusEvent.target, 'select');

      //act
      component.onFocus(mockFocusEvent);

      //assert
      expect(component.focus.next).toHaveBeenCalledOnceWith(mockFocusEvent);
      expect(mockFocusEvent.target.select).not.toHaveBeenCalled();
    });
  });

  describe('onBlur', () => {
    const precision = 1;
    const defaultNumberValue = 1.5678;
    const factor = Math.pow(10, precision);
    const roundedValue = Math.round(Number(defaultNumberValue) * factor) / factor;

    it('should call writeValue with roundedValue, call blur.next and onTouchedCallback', () => {
      //arrange
      component.type = 'number';
      component.precision = precision;
      component.value = defaultNumberValue;
      spyOn(component.blur, 'next');
      spyOn(component, 'onTouchedCallback');
      spyOn(component, 'writeValue');
      //act
      component.onBlur(event);

      //assert
      expect(component.writeValue).toHaveBeenCalledOnceWith(roundedValue);
      expect(component.blur.next).toHaveBeenCalledOnceWith(event);
      expect(component.onTouchedCallback).toHaveBeenCalledOnceWith();
    });

    it('should just call blur.next and onTouchedCallback if the value equals roundedValue', () => {
      //arrange
      component.type = 'number';
      component.precision = precision;
      component.value = roundedValue;
      spyOn(component.blur, 'next');
      spyOn(component, 'onTouchedCallback');
      spyOn(component, 'writeValue');
      //act
      component.onBlur(event);

      //assert
      expect(component.writeValue).not.toHaveBeenCalled();
      expect(component.blur.next).toHaveBeenCalledOnceWith(event);
      expect(component.onTouchedCallback).toHaveBeenCalledOnceWith();
    });

    it('should just call blur.next and onTouchedCallback', () => {
      //arrange
      spyOn(component.blur, 'next');
      spyOn(component, 'onTouchedCallback');
      spyOn(component, 'writeValue');
      //act
      component.onBlur(event);

      //assert
      expect(component.writeValue).not.toHaveBeenCalled();
      expect(component.blur.next).toHaveBeenCalledOnceWith(event);
      expect(component.onTouchedCallback).toHaveBeenCalledOnceWith();
    });
  });

  describe('keyPressEvent', () => {
    it('should call preventDefault after pressing . with precision 0', () => {
      //arrange
      component.precision = 0;
      event = new KeyboardEvent('keypress', {
        key: '.'
      });
      spyOn(event, 'preventDefault');
      //act
      component.keyPressEvent(event);
      //assert
      expect(event.preventDefault).toHaveBeenCalledOnceWith();
    });

    it('should call preventDefault after pressing insert', () => {
      //arrange
      component.min = 1;
      event = new KeyboardEvent('keypress', {
        keyCode: 45
      });
      spyOn(event, 'preventDefault');
      //act
      component.keyPressEvent(event);
      //assert
      expect(event.preventDefault).toHaveBeenCalledOnceWith();
    });

    it('should do nothing', () => {
      //arrange
      spyOn(event, 'preventDefault');
      //act
      component.keyPressEvent(event);
      //assert
      expect(event.preventDefault).not.toHaveBeenCalledOnceWith();
    });
  });
});
