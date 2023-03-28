/* tslint:disable:no-unused-variable */
import { fakeAsync, tick } from '@angular/core/testing';
import { NumericCellEditorComponent } from './numeric-cell-editor.component';
import { AmcsDataGridModule } from '@shared-module/amcs-data-grid/amcs-data-grid.module';
import { ICellEditorParams } from 'ag-grid-community';
import { MockRenderFactory, ngMocks, MockBuilder } from 'ng-mocks';

describe('NumericCellEditorComponent', () => {
  const factory = MockRenderFactory(NumericCellEditorComponent, []);
  ngMocks.faster();

  beforeEach(() => MockBuilder(NumericCellEditorComponent, AmcsDataGridModule));
  beforeEach(() => factory.configureTestBed());

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
    const initialValue = 999;
    const params = {
      value: initialValue,
      colDef: {
        field: 'myField',
      },
      data: {
        myField: initialValue,
      },
    } as ICellEditorParams;

    fixture.agInit(params);
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

    expect(fixture.cellValue).toBe(params.value);
    expect(fixture['params']).toBe(params);
  });

  it('onchange updates value', () => {
    const { fixture, params } = createDefaultFixture();

    expect(fixture.cellValue).toBe(params.value);
    expect(fixture['params']).toBe(params);
    const newValue = 14567;

    fixture.onChange(newValue);

    expect(fixture.cellValue).toBe(newValue);
  });

  it('get value returns value', () => {
    const { fixture, params } = createDefaultFixture();
    expect(fixture.cellValue).toBe(params.value);

    expect(fixture.getValue()).toBe(params.value);

    const newValue = 1234567;

    fixture.onChange(newValue);

    expect(fixture.getValue()).toBe(newValue);
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
