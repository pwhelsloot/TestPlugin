import { ElementRef, NO_ERRORS_SCHEMA, Renderer2 } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ControlContainer } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { ModalGridSelectorServiceAdapter } from '@core-module/services/forms/modal-grid-selector.adapter';
import { Subject } from 'rxjs';
import { AmcsModalConfig } from '../amcs-modal/amcs-modal-config.model';
import { AmcsModalService } from '../amcs-modal/amcs-modal.service';
import { AmcsModalSelectorComponent } from './amcs-modal-selector.component';
import { ModalSelectorModeEnum } from './modal-selector-mode.enum';
import { ModalDetailsComponent } from './modal-selector/modal-details.component';

describe('component: AmcsModalSelectorComponent', () => {
  let component: AmcsModalSelectorComponent;
  let fixture: ComponentFixture<AmcsModalSelectorComponent>;
  let mockControlContainer = {} as ControlContainer;
  let mockElRef = {} as ElementRef;
  let mockRenderer2 = {} as Renderer2;
  let mockModalService = {} as AmcsModalService;
  let mockAdapter = {} as ModalGridSelectorServiceAdapter;
  let mockModalCloseSubject: Subject<boolean | ILookupItem[]>;
  let mode: ModalSelectorModeEnum = ModalSelectorModeEnum.Single;
  const value = 3;
  const autoClose = false;
  const mockFilter: IFilter = { filterOperation: FilterOperation.Contains, name: 'test', value: 1 };
  const selectedItem: ILookupItem = { id: 2, description: 'd2' };
  const modalTitle = 'Test title';
  const observer: jasmine.Spy = jasmine.createSpy('AmcsModalSelectorComponent Observer');

  beforeEach(async () => {
    mockModalCloseSubject = new Subject<boolean | ILookupItem[]>();
    mockModalService.createModal = (config: AmcsModalConfig): MatDialogRef<any, any> => {
      const mockMatDialog = {} as MatDialogRef<any, any>;
      mockMatDialog.afterClosed = () => {
        return mockModalCloseSubject;
      };
      return mockMatDialog;
    };
    TestBed.configureTestingModule({
      declarations: [AmcsModalSelectorComponent],
      imports: [],
      providers: [
        { provide: ControlContainer, useValue: mockControlContainer },
        { provide: ElementRef, useValue: mockElRef },
        { provide: Renderer2, useValue: mockRenderer2 },
        { provide: AmcsModalService, useValue: mockModalService },
        { provide: ModalGridSelectorServiceAdapter, useValue: mockAdapter },
      ],
      schemas: [NO_ERRORS_SCHEMA],
    });

    TestBed.overrideComponent(AmcsModalSelectorComponent, {
      set: {
        providers: [],
      },
    });
    TestBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsModalSelectorComponent);
    component = fixture.componentInstance;
    component.modalTitle = modalTitle;
    component.value = value;
    component.columns = [];
    component.filters = [mockFilter];
    component.autoClose = autoClose;
    component.modalSelectorMode = mode;
    fixture.detectChanges();
  });

   afterEach(() => {
      observer.calls.reset();
   });

  it('should create', () => {
    expect(component).toBeTruthy();
    expect(component.edited).toBeFalse();
  });

  it('should create ModalDetailsComponent modal when onControlClick is called', () => {
    // Arrange
    spyOn(mockModalService, 'createModal').and.callThrough();

    // Act
    component.onControlClick();

    //Assert
    expect(mockModalService.createModal).toHaveBeenCalledOnceWith({
      type: 'confirmation',
      title: modalTitle,
      template: ModalDetailsComponent,
      largeSize: true,
      hideButtons: true,
      extraData: [[value], [], mockAdapter, [mockFilter], mode === ModalSelectorModeEnum.Multi, autoClose],
    });
  });

  it('should do nothing if modal is closed with false value', () => {
      // Arrange
      spyOn(component, 'writeValue').and.callThrough();
      const subscription = component.itemsSelected.subscribe(observer);

      // Act
    component.onControlClick();
    mockModalCloseSubject.next(false);

    //Assert
    expect(component.writeValue).toHaveBeenCalledTimes(0);
    expect(observer).toHaveBeenCalledTimes(0);
    subscription.unsubscribe();
    });

    it('should call writeValue and emit itemSelected if modal is closed with selectedItem', () => {
      // Arrange
      spyOn(component, 'writeValue').and.callThrough();
      const subscription = component.itemsSelected.subscribe(observer);

      // Act
      component.onControlClick();
      mockModalCloseSubject.next([selectedItem]);

      //Assert
      expect(component.writeValue).toHaveBeenCalledOnceWith(selectedItem.id);
      expect(observer).toHaveBeenCalledOnceWith(selectedItem);
      subscription.unsubscribe();
    });

      it('reset should call writeValue(null) and emit null on itemSelected', () => {
        // Arrange
        spyOn(component, 'writeValue').and.callThrough();
        const subscription = component.itemsSelected.subscribe(observer);

        // Act
        component.reset();

        //Assert
        expect(component.writeValue).toHaveBeenCalledOnceWith(null);
        expect(observer).toHaveBeenCalledOnceWith(null);
        subscription.unsubscribe();
      });

     it('writeValue(value) should set value, edited=true and onChangeCallback(value) should be called', () => {
            // Arrange + assert
            spyOn(component, 'onChangeCallback').and.callThrough();
            expect(component.value).toEqual(value);

            const newValue = 5;

            // Act
            component.writeValue(newValue);

            //Assert
            expect(component.onChangeCallback).toHaveBeenCalledOnceWith(newValue);
            expect(component.value).toEqual(newValue);
            expect(component.edited).toBeTrue();
          });


    it('writeValue(null) should call writeNullValue()', () => {
        // Arrange
        spyOn(component, 'onChangeCallback').and.callThrough();

        // Act
        component.writeValue(null);

        //Assert
         expect(component.onChangeCallback).toHaveBeenCalledOnceWith(null);
         expect(component.description).toEqual('');
         expect(component.value).toBeNull();
        expect(component.edited).toBeFalse();
    });
});
