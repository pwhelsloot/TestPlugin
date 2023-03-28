import { ElementRef, Renderer2, SimpleChange, SimpleChanges } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { ControlContainer } from '@angular/forms';
import { IGroupedLookupItem } from '@core-module/models/lookups/grouped-lookup-item.interface';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { MockProvider } from 'ng-mocks';
import { BehaviorSubject, Subject } from 'rxjs';
import { AmcsSelectHelper } from '../helpers/amcs-select-helper';
import { AmcsSelectComponent } from './amcs-select.component';

describe('AmcsSelectComponent', () => {
  const translations = [];
  const destroy: Subject<void> = new Subject();
  const previewChanges: Subject<any> = new Subject();
  const selectElement: ElementRef = new ElementRef({ nativeElement: 'T', value: undefined });

  let defaultOptions: ILookupItem[] | IGroupedLookupItem[] = [
    { id: 0, description: 'testDescription' },
    { id: 1, description: 'testDescription2' }
  ];
  let component: AmcsSelectComponent;
  let fixture: ComponentFixture<AmcsSelectComponent>;
  let appTranslationsService: SharedTranslationsService;
  let defaultValue: any;
  let change: SimpleChanges;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsSelectComponent],
      providers: [
        MockProvider(ElementRef),
        MockProvider(Renderer2),
        MockProvider(SharedTranslationsService),
        MockProvider(ControlContainer)
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(AmcsSelectComponent);
    appTranslationsService = TestBed.inject(SharedTranslationsService);

    translations['searchselect.select'] = 'test select';
    appTranslationsService.translations = new BehaviorSubject<string[]>(translations);

    component = fixture.componentInstance;
    fixture.detectChanges();

    component.previewChanges = previewChanges;
    component.selectElement = selectElement;
    change = {};
    defaultValue = '0';
  });

  afterEach(() => {
    destroy.next();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
  describe('writeValueFromView', () => {
    it('should set it to null ', () => {
      //arrange
      defaultValue = undefined;
      spyOn(component.previewChanges, 'next');

      //act
      component.writeValueFromView(defaultValue);

      // assert
      expect(component.previewChanges.next).toHaveBeenCalledOnceWith(null);
    });

    it('should set it to null ', () => {
      //arrange
      defaultValue = 'null';
      spyOn(component.previewChanges, 'next');

      //act
      component.writeValueFromView(defaultValue);

      // assert
      expect(component.previewChanges.next).toHaveBeenCalledOnceWith(null);
    });

    it('should only call writeValue and pass parameter ', () => {
      //arrange
      spyOn(component, 'writeValue');
      spyOn(component.previewChanges, 'next');
      component.previewChanges = undefined;

      //act
      component.writeValueFromView(defaultValue);

      // assert
      expect(component.writeValue).toHaveBeenCalledOnceWith(defaultValue);
    });

    it('should only call previewChanges.next and pass parameter ', () => {
      //arrange
      spyOn(component, 'writeValue');
      spyOn(component.previewChanges, 'next');
      component.useNumericValues = false;

      //act
      component.writeValueFromView(defaultValue);

      // assert
      expect(component.previewChanges.next).toHaveBeenCalledOnceWith(defaultValue);
      expect(component.writeValue).not.toHaveBeenCalled();
    });

    it('should only call previewChanges.next and pass numeric parameter ', () => {
      //arrange
      spyOn(component, 'writeValue');
      spyOn(component.previewChanges, 'next');
      component.useNumericValues = true;

      //act
      component.writeValueFromView(defaultValue);

      // assert
      expect(component.previewChanges.next).toHaveBeenCalledOnceWith(Number(defaultValue));
      expect(component.writeValue).not.toHaveBeenCalled();
    });

    it('should set selectElement.nativeElement.value to @input value ', () => {
      //arrange
      component.value = 'test';

      //act
      component.writeValueFromView(null);

      // assert
      expect(component.selectElement.nativeElement.value).toEqual(component.value);
    });
  });

  describe('ngOnChanges', () => {
    it('should call doSort and checkSelectionValid ', () => {
      //arrange
      component.keepOriginalOrder = false;
      component.options = defaultOptions;
      change = { options: new SimpleChange(0, 1, true) };
      spyOn(AmcsSelectHelper, 'doSort');
      spyOn(component, 'checkSelectionValid');

      //act
      component.ngOnChanges(change);

      // assert
      expect(component.checkSelectionValid).toHaveBeenCalled();
      expect(AmcsSelectHelper.doSort).toHaveBeenCalledOnceWith(defaultOptions);
    });

    it('should not call doSort and checkSelectionValid ', () => {
      //arrange
      component.keepOriginalOrder = true;
      component.options = defaultOptions;
      change = { options: new SimpleChange(0, 1, true) };
      spyOn(AmcsSelectHelper, 'doSort');
      spyOn(component, 'checkSelectionValid');

      //act
      component.ngOnChanges(change);

      // assert
      expect(component.checkSelectionValid).not.toHaveBeenCalled();
      expect(AmcsSelectHelper.doSort).not.toHaveBeenCalled();
    });

    it('should not call doSort and checkSelectionValid ', () => {
      //arrange
      component.options = defaultOptions;
      change = {};
      spyOn(AmcsSelectHelper, 'doSort');
      spyOn(component, 'checkSelectionValid');

      //act
      component.ngOnChanges(change);

      // assert
      expect(component.checkSelectionValid).not.toHaveBeenCalled();
      expect(AmcsSelectHelper.doSort).not.toHaveBeenCalled();
    });
  });

  describe('ngAfterViewInit', () => {
    it('should call renderer.addClass once for every custom class', () => {
      //arrange
      const customClass = 'a b c';
      component.customClass = customClass;
      spyOn(component['renderer'], 'addClass');

      //act
      component.ngAfterViewInit();

      // assert
      expect(component['renderer'].addClass).toHaveBeenCalledTimes(customClass.split(' ').length);
    });

    it('should do nothing', () => {
      //arrange
      spyOn(component['renderer'], 'addClass');

      //act
      component.ngAfterViewInit();

      // assert
      expect(component['renderer'].addClass).not.toHaveBeenCalled();
    });
  });

  describe('ngOnDestroy', () => {
    it('should unsubscribe', () => {
      //arrange
      spyOn(component['translationSubscription'], 'unsubscribe');
      spyOn(AmcsFormControlBaseComponent.prototype, 'ngOnDestroy');

      //act
      component.ngOnDestroy();

      // assert
      expect(component['translationSubscription'].unsubscribe).toHaveBeenCalledOnceWith();
      expect(AmcsFormControlBaseComponent.prototype.ngOnDestroy).toHaveBeenCalledOnceWith();
    });
  });

  describe('writeValue', () => {
    beforeEach(() => {
      component.value = null;
      component.edited = false;
    });
    it('should change edited to false and call onChangeCallback with null', () => {
      //arrange
      component.edited = true;
      defaultValue = null;
      spyOn(component, 'onChangeCallback');

      //act
      component.writeValue(defaultValue);

      // assert
      expect(component.value).toEqual(defaultValue);
      expect(component.edited).toBeFalse();
      expect(component.onChangeCallback).toHaveBeenCalledWith(null);
    });

    it('should change edited to true and call onChangeCallback with parameter value', () => {
      //arrange
      component.useNumericValues = false;
      spyOn(component, 'onChangeCallback');

      //act
      component.writeValue(defaultValue);

      // assert
      expect(component.value).toEqual(defaultValue);
      expect(component.edited).toBeTrue();
      expect(component.onChangeCallback).toHaveBeenCalledWith(defaultValue);
    });

    it('should change edited to true and call onChangeCallback with numerical value of parameter', () => {
      //arrange
      spyOn(component, 'onChangeCallback');

      //act
      component.writeValue(defaultValue);

      // assert
      expect(component.value).toEqual(defaultValue);
      expect(component.edited).toBeTrue();
      expect(component.onChangeCallback).toHaveBeenCalledWith(Number(defaultValue));
    });
  });

  describe('checkSelectionValid', () => {
    beforeEach(() => {
      component.options = defaultOptions;
    });

    it('should do nothing ', fakeAsync(async () => {
      //arrange
      component.value = null;
      spyOn(component, 'writeValue');
      spyOn(component.options, 'some').and.callThrough();

      //act
      component.checkSelectionValid();
      tick(0);

      // assert
      expect(component.writeValue).not.toHaveBeenCalled();
      expect(component.options.some).not.toHaveBeenCalled();
    }));

    it('should do find some value and call some on options) ', fakeAsync(async () => {
      //arrange
      component.value = '0';
      component.useNumericValues = true;
      component.options = defaultOptions;
      component.bindValue = 'id';
      spyOn(component, 'writeValue');
      spyOn(component.options, 'some').and.callThrough();

      //act
      component.checkSelectionValid();
      tick(0);

      // assert
      expect(component.writeValue).not.toHaveBeenCalled();
      expect(component.options.some).toHaveBeenCalledTimes(1);
    }));
  });

  describe('support methods', () => {
    const testParam = 'test';

    it('registerOnChange should set onChangeCallback to passed parameter', () => {
      //arrange

      //act
      component.registerOnChange(testParam);

      //assert
      expect(component.onChangeCallback).toEqual(testParam);
    });

    it('registerOnTouched should set onTouchedCallback to passed parameter', () => {
      //arrange

      //act
      component.registerOnTouched(testParam);

      //assert
      expect(component.onTouchedCallback).toEqual(testParam);
    });

    it('should set isDisabled to true', () => {
      //arrange
      //act
      component.setDisabledState(true);

      //assert
      expect(component.isDisabled).toBeTrue();
    });

    it('onFocus should call focus.next once with its parameter', () => {
      //arrange
      spyOn(component.focus, 'next');

      //act
      component.onFocus(null);

      //assert
      expect(component.focus.next).toHaveBeenCalledOnceWith(null);
    });
    it('onBlur should call blur.next once with its parameter', () => {
      //arrange
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
      spyOn(component.change, 'emit');

      //act
      component.onChange(testParam);

      //assert
      expect(component.change.emit).toHaveBeenCalledOnceWith(testParam);
    });
  });
});
