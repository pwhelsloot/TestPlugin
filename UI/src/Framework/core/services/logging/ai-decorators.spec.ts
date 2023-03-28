/* eslint-disable max-classes-per-file */
/* tslint:disable:max-classes-per-file */
import { AfterViewInit, Component, NO_ERRORS_SCHEMA, OnDestroy, OnInit } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiLoggingService } from '@core-module/services/logging/ai-logging.service';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

export class AiDecoratorTestComponentConstants {
  static viewName = 'test';
}
// Test component to attach decorator
@Component({
  selector: 'app-decorator-test',
  template: '',
})
@aiComponent(AiDecoratorTestComponentConstants.viewName)
export class AiDecoratorTestComponent implements OnInit, AfterViewInit, OnDestroy {
  viewReady: AiViewReady;
  initInnerMethodCalled = false;
  afterViewInitInnerMethodCalled = false;
  destroyInnerMethodCalled = false;

  ngOnInit(): void {
    this.initInnerMethodCalled = true;
  }
  ngAfterViewInit(): void {
    this.afterViewInitInnerMethodCalled = true;
  }
  ngOnDestroy(): void {
    this.destroyInnerMethodCalled = true;
  }
}
describe('AIDecorators', () => {
  let component: AiDecoratorTestComponent;
  let fixture: ComponentFixture<AiDecoratorTestComponent>;
  let viewReady: AiViewReady;
  AiLoggingService.aiLoggingServiceReference = {} as AiLoggingService;

  beforeEach(async () => {
    viewReady = new AiViewReady();
    AiLoggingService.aiLoggingServiceReference.viewInit = (viewName: string): void => {};
    AiLoggingService.aiLoggingServiceReference.viewFirstRender = (viewName: string): void => {};
    AiLoggingService.aiLoggingServiceReference.viewReady = (viewName: string): void => {};
    AiLoggingService.aiLoggingServiceReference.viewDestroyed = (viewName: string) => void {};

    TestBed.configureTestingModule({
      declarations: [AiDecoratorTestComponent],
      imports: [],
      schemas: [NO_ERRORS_SCHEMA],
    });
    TestBed.compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(AiDecoratorTestComponent);
    component = fixture.componentInstance;
  });

  it('ngOnInit calls ai logging viewInit', () => {
    // Arrange
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewInit').and.callThrough();
    expect(component.initInnerMethodCalled).toBeFalse();

    // Act
    component.viewReady = viewReady;
    component.ngOnInit();

    // Assert
    expect(AiLoggingService.aiLoggingServiceReference.viewInit).toHaveBeenCalledOnceWith(AiDecoratorTestComponentConstants.viewName);
    expect(component.initInnerMethodCalled).toBeTrue();
  });

  it('ngOnInit to throw error if viewReady not set', () => {
    // Arrange
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewInit').and.callThrough();

    // Act/Assert
    expect(function() {
      component.ngOnInit();
    }).toThrow(
      new Error(
        `viewReady subject missing for ${AiDecoratorTestComponentConstants.viewName}. This must be created/assigned in component class definition or constructor, i.e BEFORE ngOnInit.`
      )
    );
  });

  it('ngAfterViewInit calls ai logging viewInit ', () => {
    // Arrange
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewInit').and.callThrough();
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewFirstRender').and.callThrough();
    expect(component.initInnerMethodCalled).toBeFalse();
    expect(component.afterViewInitInnerMethodCalled).toBeFalse();
    component.viewReady = viewReady;
    component.ngOnInit();

    // Act
    component.ngAfterViewInit();

    // Assert
    expect(AiLoggingService.aiLoggingServiceReference.viewInit).toHaveBeenCalledOnceWith(AiDecoratorTestComponentConstants.viewName);
    expect(AiLoggingService.aiLoggingServiceReference.viewFirstRender).toHaveBeenCalledOnceWith(AiDecoratorTestComponentConstants.viewName);
    expect(component.initInnerMethodCalled).toBeTrue();
    expect(component.afterViewInitInnerMethodCalled).toBeTrue();
  });

  it('ngOnDestroy calls ai logging viewInit and completes viewReady', () => {
    // Arrange
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewInit').and.callThrough();
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewDestroyed').and.callThrough();
    expect(component.initInnerMethodCalled).toBeFalse();
    expect(component.destroyInnerMethodCalled).toBeFalse();
    component.viewReady = viewReady;
    component.ngOnInit();

    // Act
    component.ngOnDestroy();

    // Assert
    expect(AiLoggingService.aiLoggingServiceReference.viewInit).toHaveBeenCalledOnceWith(AiDecoratorTestComponentConstants.viewName);
    expect(AiLoggingService.aiLoggingServiceReference.viewDestroyed).toHaveBeenCalledOnceWith(AiDecoratorTestComponentConstants.viewName);
    expect(component.initInnerMethodCalled).toBeTrue();
    expect(component.destroyInnerMethodCalled).toBeTrue();

    // Ensures viewReady has been completed
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewReady').and.callThrough();

    component.viewReady.next();

    expect(AiLoggingService.aiLoggingServiceReference.viewReady).toHaveBeenCalledTimes(0);
  });

  it('pushing undefined or true onto viewReady calls ai logging viewInit ', () => {
    // Arrange
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewReady').and.callThrough();
    component.viewReady = viewReady;
    component.ngOnInit();

    // Act
    component.viewReady.next();

    // Assert
    expect(AiLoggingService.aiLoggingServiceReference.viewReady).toHaveBeenCalledOnceWith(AiDecoratorTestComponentConstants.viewName);

    // Act
    component.ngOnInit();
    component.viewReady.next(true);

    // Assert
    expect(AiLoggingService.aiLoggingServiceReference.viewReady).toHaveBeenCalledTimes(2);
    expect(AiLoggingService.aiLoggingServiceReference.viewReady).toHaveBeenCalledWith(AiDecoratorTestComponentConstants.viewName);
  });

  it('pushing undefined or true onto viewReady calls ai logging viewInit ', () => {
    // Arrange
    spyOn(AiLoggingService.aiLoggingServiceReference, 'viewReady').and.callThrough();
    component.viewReady = viewReady;
    component.ngOnInit();

    // Act
    component.viewReady.next(false);

    // Assert
    expect(AiLoggingService.aiLoggingServiceReference.viewReady).toHaveBeenCalledTimes(0);
  });
});
