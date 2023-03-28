/* tslint:disable:no-unused-variable */
import { DatePipe } from '@angular/common';
import { fakeAsync, tick } from '@angular/core/testing';
import { AmcsDataGridModule } from '@shared-module/amcs-data-grid/amcs-data-grid.module';
import { ICellEditorParams } from 'ag-grid-community';
import { MockRenderFactory, ngMocks, MockBuilder } from 'ng-mocks';

import { DateCellEditorComponent } from './date-cell-editor.component';

describe('DateCellEditorComponent', () => {
  const factory = MockRenderFactory(DateCellEditorComponent, [], {
    providers: [DatePipe],
  });
  ngMocks.faster();
  let datePipe: DatePipe;
  beforeEach(() => MockBuilder(DateCellEditorComponent, AmcsDataGridModule));
  beforeEach(() => factory.configureTestBed());
  beforeEach(() => {
    datePipe = ngMocks.findInstance(DatePipe);
  });

  function createFixture(...params: Parameters<typeof factory>) {
    const fixture = factory(...params);
    fixture.autoDetectChanges();
    return fixture;
  }

  function getFixtureInstance(...params: Parameters<typeof factory>) {
    return createFixture(...params).point.componentInstance;
  }

  function createDefaultFixture() {
    const fixture = getFixtureInstance();
    const date = new Date();
    const params = {
      value: date,
      colDef: {
        field: 'myField',
      },
      data: {
        myField: date,
      },
      stopEditing: () => {},
    } as ICellEditorParams;

    fixture.agInit(params);
    fixture.isFirstChange = false;
    return { fixture, params };
  }

  it('should create', () => {
    const { fixture, params } = createDefaultFixture();

    expect(fixture).toBeTruthy();
    expect(fixture.input.element).toBeTruthy();
  });

  it('should focus on element', fakeAsync(() => {
    const { fixture, params } = createDefaultFixture();
    spyOn(fixture.input.element.nativeElement, 'focus');

    tick();

    expect(fixture.input.element.nativeElement.focus).toHaveBeenCalledOnceWith();
  }));

  it('agInit sets params and value', () => {
    const { fixture, params } = createDefaultFixture();

    expect(fixture.cellValue).toEqual(params.value);
    expect(fixture['params']).toEqual(params);
  });

  it('onchange updates value', () => {
    const { fixture, params } = createDefaultFixture();

    expect(fixture.cellValue).toEqual(params.value);
    expect(fixture['params']).toEqual(params);
    const newDate = new Date();

    fixture.onChange(newDate);

    expect(fixture.cellValue).toEqual(newDate);
  });

  it('get value returns value', () => {
    const { fixture, params } = createDefaultFixture();
    expect(fixture.cellValue).toEqual(params.value);

    expect(fixture.getValue()).toEqual(params.value);

    const newValue = new Date();

    fixture.onChange(newValue);

    expect(fixture.getValue()).toEqual(newValue);
  });

  it('isCancelBeforeStart returns false', () => {
    const { fixture, params } = createDefaultFixture();
    expect(fixture.isCancelBeforeStart()).toBeFalse();
  });

  it('isCancelAfterEnd returns false', () => {
    const { fixture, params } = createDefaultFixture();
    expect(fixture.isCancelAfterEnd()).toBeFalse();
  });
});
