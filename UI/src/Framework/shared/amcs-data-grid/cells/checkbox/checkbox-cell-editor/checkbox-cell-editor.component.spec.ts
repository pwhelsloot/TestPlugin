/* tslint:disable:no-unused-variable */

import { CheckboxCellEditorComponent } from './checkbox-cell-editor.component';
import { MockBuilder, MockRenderFactory, ngMocks } from 'ng-mocks';
import { AmcsDataGridModule } from '@shared-module/amcs-data-grid/amcs-data-grid.module';
import { ICellEditorParams } from 'ag-grid-enterprise';
import { fakeAsync, tick } from '@angular/core/testing';

describe('CheckboxCellEditorComponent', () => {
  const factory = MockRenderFactory(CheckboxCellEditorComponent, []);
  ngMocks.faster();

  beforeEach(() => MockBuilder(CheckboxCellEditorComponent, AmcsDataGridModule));
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

    const params = {
      value: false,
      colDef: {
        field: 'myField',
      },
      data: {
        myField: false,
      },
    } as ICellEditorParams;

    fixture.agInit(params);
    return { fixture, params };
  }

  it('should create', () => {
    const { fixture, params } = createDefaultFixture();

    expect(fixture).toBeTruthy();
    expect(fixture.checkbox).toBeTruthy();
  });

  it('should focus on element', fakeAsync(() => {
    const { fixture, params } = createDefaultFixture();
    spyOn(fixture.checkbox.nativeElement, 'focus');

    tick();

    expect(fixture.checkbox.nativeElement.focus).toHaveBeenCalledOnceWith();
  }));

  it('agInit sets params and value', () => {
    const { fixture, params } = createDefaultFixture();

    expect(fixture.cellValue).toBe(params.value);
    expect(fixture['params']).toBe(params);
  });

  it('onchange updates value', () => {
    const { fixture, params } = createDefaultFixture();

    expect(fixture.cellValue).toBeFalse();
    expect(fixture['params']).toBe(params);

    fixture.onChange(true);

    expect(fixture.cellValue).toBeTrue();
  });

  it('get value returns value', () => {
    const { fixture, params } = createDefaultFixture();
    expect(fixture.cellValue).toBeFalse();

    expect(fixture.getValue()).toBeFalse();

    fixture.onChange(true);

    expect(fixture.getValue()).toBeTrue();
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
