import { HttpClientModule } from '@angular/common/http';
import { ElementRef, NO_ERRORS_SCHEMA, Renderer2 } from '@angular/core';
import { ComponentFixture, fakeAsync, TestBed, tick } from '@angular/core/testing';
import { HttpClient } from '@microsoft/signalr';
import { NgSelectComponent, NgSelectModule } from '@ng-select/ng-select';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { AmcsFormControlBaseComponent } from '@shared-module/forms/amcs-form-control-base';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { createTranslateLoader } from '@shared-module/shared.module';
import { MockProvider } from 'ng-mocks';
import { BehaviorSubject, Subject } from 'rxjs';
import { AmcsSelectTypeaheadComponent } from './amcs-select-typeahead.component';

describe('AmcsSelectTypeaheadComponent', () => {
  const translations = [];
  const destroy: Subject<void> = new Subject();
  const mockItemSubject = new Subject<string>();

  let component: AmcsSelectTypeaheadComponent;
  let fixture: ComponentFixture<AmcsSelectTypeaheadComponent>;
  let appTranslationsService: SharedTranslationsService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsSelectTypeaheadComponent, NgSelectComponent],
      imports: [
        NgSelectModule,
        HttpClientModule,
        TranslateModule.forRoot({
          loader: { provide: TranslateLoader, useFactory: createTranslateLoader, deps: [HttpClient] },
          isolate: true
        })
      ],
      providers: [HttpClient, MockProvider(ElementRef), MockProvider(Renderer2), MockProvider(SharedTranslationsService)],
      schemas: [NO_ERRORS_SCHEMA]
    }).compileComponents();

    fixture = TestBed.createComponent(AmcsSelectTypeaheadComponent);
    appTranslationsService = TestBed.inject(SharedTranslationsService);

    translations['selectTypeahead.typeToSearch'] = 'typeToSearch-test';
    translations['selectTypeahead.noResultsFound'] = 'noResultsFound-test';
    translations['selectTypeahead.loadingText'] = 'loadingText-test';
    appTranslationsService.translations = new BehaviorSubject<string[]>(translations);

    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  afterEach(() => {
    destroy.next();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('should create', () => {
    //arrange
    //act
    //assert
    expect(component).toBeTruthy();
  });

  describe('ngOnInit', () => {
    it('should set translations', () => {
      //arrange
      //act
      component.ngOnInit();

      //assert
      expect(component.typeToSearchText).toEqual(translations['selectTypeahead.typeToSearch']);
      expect(component.notFoundText).toEqual(translations['selectTypeahead.noResultsFound']);
      expect(component.loadingText).toEqual(translations['selectTypeahead.loadingText']);
    });
  });

  describe('ngAfterViewInit', () => {
    it('should set selector.typeahead, call selector.setDisabledState with false, but not selector.focus', fakeAsync(async () => {
      //arrange
      component.inputSubject = mockItemSubject;
      spyOn(component.selector, 'setDisabledState');
      spyOn(component.selector, 'focus');

      //act
      component.ngAfterViewInit();
      tick();

      //assert
      expect(component.selector.typeahead).toEqual(mockItemSubject);
      expect(component.selector.setDisabledState).toHaveBeenCalledOnceWith(false);
      expect(component.selector.focus).not.toHaveBeenCalled();
    }));

    it('should set selector.typeahead, call selector.setDisabledState with true and selector.focus once', fakeAsync(async () => {
      //arrange
      component.inputSubject = mockItemSubject;
      component.autoFocus = true;
      component.isDisabled = true;
      spyOn(component.selector, 'setDisabledState');
      spyOn(component.selector, 'focus');

      //act
      component.ngAfterViewInit();
      tick();

      //assert
      expect(component.selector.typeahead).toEqual(mockItemSubject);
      expect(component.selector.setDisabledState).toHaveBeenCalledOnceWith(true);
      expect(component.selector.focus).toHaveBeenCalledOnceWith();
    }));
  });

  describe('ngOnDestroy', () => {
    it('should unsubscribe and call super OnDestroy', () => {
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

  describe('onTabPressed', () => {
    it('should do nothing', () => {
      //arrange
      spyOn(component.selector, 'toggleItem');
      spyOn(component.selector, 'selectTag');

      //act
      component.onTabPressed();

      // assert
      expect(component.selector.toggleItem).not.toHaveBeenCalled();
      expect(component.selector.selectTag).not.toHaveBeenCalled();
    });

    it('should do nothing', () => {
      //arrange
      component.selectOnTab = true;
      component.selector.isOpen = false;
      spyOn(component.selector, 'toggleItem');
      spyOn(component.selector, 'selectTag');

      //act
      component.onTabPressed();

      // assert
      expect(component.selector.toggleItem).not.toHaveBeenCalled();
      expect(component.selector.selectTag).not.toHaveBeenCalled();
    });

    it('should do call selector.selectTag once', () => {
      //arrange
      component.selectOnTab = true;
      component.selector.isOpen = true;
      component.selector.addTag = true;

      spyOn(component.selector, 'toggleItem');
      spyOn(component.selector, 'selectTag');

      //act
      component.onTabPressed();

      // assert
      expect(component.selector.toggleItem).not.toHaveBeenCalled();
      expect(component.selector.selectTag).toHaveBeenCalledOnceWith();
    });

    it('should call selector.toggleItem once', () => {
      //arrange
      component.selectOnTab = true;
      component.selector.isOpen = true;
      component.selector.itemsList.setItems([{ name: 'test' }]);
      component.selector.itemsList.markNextItem();

      spyOn(component.selector, 'toggleItem');
      spyOn(component.selector, 'selectTag');

      //act
      component.onTabPressed();

      // assert
      expect(component.selector.toggleItem).toHaveBeenCalledOnceWith(component.selector.itemsList.markedItem);
      expect(component.selector.selectTag).not.toHaveBeenCalled();
    });
  });

  describe('onBlur', () => {
    it('should call blur.emit once with its parameter', () => {
      //arrange
      const testParam = 'test';
      spyOn(component.blur, 'emit');
      spyOn(component, 'onTouchedCallback');

      //act
      component.onBlur(testParam);

      //assert
      expect(component.blur.emit).toHaveBeenCalledOnceWith(testParam);
      expect(component.onTouchedCallback).toHaveBeenCalledOnceWith();
    });
  });

  describe('writeValue', () => {
    it('should call onChangeCallback once with passed in value, and change edited to true', () => {
      //arrange
      const mockValue = 'test';
      component.edited = false;
      component.clearOnSelect = false;
      spyOn(component, 'onChangeCallback');
      spyOn(component.selector, 'handleClearClick');

      //act
      component.writeValue(mockValue);

      //assert
      expect(component.edited).toBeTrue();
      expect(component.onChangeCallback).toHaveBeenCalledOnceWith(mockValue);
      expect(component.selector.handleClearClick).not.toHaveBeenCalled();
    });

    it('should call onChangeCallback once with passed in value, handleClearClick once, and change edited to true', () => {
      //arrange
      const mockValue = 'test';
      component.edited = false;
      component.clearOnSelect = true;
      spyOn(component, 'onChangeCallback');
      spyOn(component.selector, 'handleClearClick');

      //act
      component.writeValue(mockValue);

      //assert
      expect(component.edited).toBeTrue();
      expect(component.onChangeCallback).toHaveBeenCalledOnceWith(mockValue);
      expect(component.selector.handleClearClick).toHaveBeenCalledOnceWith();
    });

    it('should call onChangeCallback once with null, and change edited to false', () => {
      //arrange
      const mockValue = [];
      component.edited = true;
      component.clearOnSelect = false;
      spyOn(component, 'onChangeCallback');
      spyOn(component.selector, 'handleClearClick');

      //act
      component.writeValue(mockValue);

      //assert
      expect(component.edited).toBeFalse();
      expect(component.onChangeCallback).toHaveBeenCalledOnceWith(null);
      expect(component.selector.handleClearClick).not.toHaveBeenCalled();
    });
  });

  describe('support methods', () => {
    const mockParam = 'test';

    it('onChangeCallback should call change.emit once with passed parameter', () => {
      //arrange
      spyOn(component.change, 'emit');

      //act
      component.onChangeCallback(mockParam);

      //assert
      expect(component.change.emit).toHaveBeenCalledOnceWith(mockParam);
    });

    it('registerOnChange should set onChangeCallback to passed parameter', () => {
      //arrange

      //act
      component.registerOnChange(mockParam);

      //assert
      expect(component.onChangeCallback).toEqual(mockParam);
    });

    it('registerOnTouched should set onTouchedCallback to passed parameter', () => {
      //arrange

      //act
      component.registerOnTouched(mockParam);

      //assert
      expect(component.onTouchedCallback).toEqual(mockParam);
    });

    it('setDisabledState should set isDisabled to true', () => {
      //arrange
      //act
      component.setDisabledState(true);

      //assert
      expect(component.isDisabled).toBeTrue();
    });
  });
});
