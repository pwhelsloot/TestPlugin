import { AgGridAngular, ICellRendererAngularComp } from 'ag-grid-angular';
import { CellValueChangedEvent, ColumnApi, GridApi, GridReadyEvent, TextCellEditor } from 'ag-grid-community';
import { MockBuilder, MockRenderFactory, ngMocks } from 'ng-mocks';
import { AmcsDataGridComponent } from './amcs-data-grid.component';
import { AmcsDataGridModule } from './amcs-data-grid.module';
import { AmcsDataGridOptionsBuilder } from './amcs-data-grid.options';
import { AmcsDataGridColDefinitions, AmcsColDef } from './amcs-column-definitions';
import { CheckboxCellRendererComponent } from './cells/checkbox/checkbox-cell-renderer/checkbox-cell-renderer.component';
import { DateCellRendererComponent } from './cells/date/date-cell-renderer/date-cell-renderer.component';
import { NumericCellRendererComponent } from './cells/numeric/numeric-cell-renderer/numeric-cell-renderer.component';
import { ChangedRowAmountHelper } from './helpers/changed-row-amount-helper';

describe('AmcsDataGridComponent', () => {
  const factory = MockRenderFactory(AmcsDataGridComponent, ['rowData', 'columnDefs', 'options', 'gridReadyEvent', 'rowChangesAmount']);
  ngMocks.faster();
  const defaultOptions = AmcsDataGridOptionsBuilder.default;

  beforeEach(() => MockBuilder(AmcsDataGridComponent, AmcsDataGridModule));
  beforeEach(() => factory.configureTestBed());

  function createFixture(...params: Parameters<typeof factory>) {
    const fixture = factory(...params);
    fixture.autoDetectChanges();
    return fixture;
  }

  function createDefaultInstance(...params: Parameters<typeof factory>) {
    return createFixture(...params);
  }

  function getAgGridInstance(): AgGridAngular {
    return ngMocks.find(AgGridAngular).componentInstance;
  }

  it('should create', () => {
    const fixture = createFixture();
    expect(fixture.componentInstance).toBeTruthy();
  });

  describe('inputs', () => {
    it('defaultColDef is set', () => {
      const fixture = createDefaultInstance({
        options: AmcsDataGridOptionsBuilder.default,
      });

      expect(fixture.point.componentInstance.defaultColDef).toEqual(defaultOptions.defaultColDef);

      expect(getAgGridInstance().defaultColDef).toEqual(defaultOptions.defaultColDef);
    });

    it('options', () => {
      const options = AmcsDataGridOptionsBuilder.default;
      const fixture = createDefaultInstance({
        options,
      });
      expect(ngMocks.input(fixture.point, 'options')).toEqual(options);
    });

    it('rowData', () => {
      const rowData = [{ make: 'Porsche', model: 'Macan', insured: 'someday', price: 72000 }];
      const fixture = createDefaultInstance({
        rowData,
      });
      expect(ngMocks.input(fixture.point, 'rowData')).toEqual(rowData);

      expect(getAgGridInstance().rowData).toEqual(rowData);
    });

    it('columnDefs', () => {
      const columnDefs = [{ field: 'price' } as AmcsColDef];
      const fixture = createDefaultInstance({
        columnDefs,
      });
      expect(ngMocks.input(fixture.point, 'columnDefs')).toEqual(columnDefs);

      expect(getAgGridInstance().columnDefs).toEqual(columnDefs);
    });
  });

  describe('outputs', () => {
    it('gridReadEvent', () => {
      const gridReadyEventSpy = jasmine.createSpy('gridReadyEvent');
      const fixture = createDefaultInstance({
        gridReadyEvent: gridReadyEventSpy,
      });

      const readyEvent = { type: 'sometype' } as GridReadyEvent;

      ngMocks.output(fixture.point, 'gridReadyEvent').emit(readyEvent);

      expect(gridReadyEventSpy).toHaveBeenCalledWith(readyEvent);
      gridReadyEventSpy.calls.reset();

      getAgGridInstance().gridReady.emit(readyEvent);

      expect(gridReadyEventSpy).toHaveBeenCalledWith(readyEvent);
    });
  });

  describe('api', () => {
    it('has api available after grid is ready', () => {
      const gridReadyEventSpy = jasmine.createSpy('gridReadyEvent');
      const fixture = createDefaultInstance({
        gridReadyEvent: gridReadyEventSpy,
      });

      const readyEvent = {
        type: 'sometype',
        api: new GridApi(),
        columnApi: new ColumnApi(),
      } as GridReadyEvent;

      ngMocks.output(fixture.point, 'gridReadyEvent').emit(readyEvent);

      expect(gridReadyEventSpy).toHaveBeenCalledWith(readyEvent);
      gridReadyEventSpy.calls.reset();

      getAgGridInstance().gridReady.emit(readyEvent);

      expect(gridReadyEventSpy).toHaveBeenCalledWith(readyEvent);
    });

    it('can download data as CSV with api', () => {
      const gridReadyEventSpy = jasmine.createSpy('gridReadyEvent');
      createDefaultInstance({
        gridReadyEvent: gridReadyEventSpy,
      });

      const readyEvent = {
        type: 'sometype',
        api: new GridApi(),
        columnApi: new ColumnApi(),
      } as GridReadyEvent;

      getAgGridInstance().gridReady.emit(readyEvent);

      expect(gridReadyEventSpy).toHaveBeenCalledWith(readyEvent);

      expect(readyEvent.api.exportDataAsCsv).toBeTruthy();
    });
  });

  describe('controls', () => {
    it('has date cell available', () => {
      const columnDefs = [AmcsDataGridColDefinitions.createDateColDef({ field: 'released' })];
      rendersWithGivenColumnDefinitions(createDefaultInstance, columnDefs, getAgGridInstance);
    });

    it('has checkbox cell available', () => {
      const columnDefs = [AmcsDataGridColDefinitions.createCheckboxColDef({ field: 'insured' })];
      rendersWithGivenColumnDefinitions(createDefaultInstance, columnDefs, getAgGridInstance);
    });

    it('has numerical cell available', () => {
      const columnDefs = [AmcsDataGridColDefinitions.createNumericColDef({ field: 'price' })];
      rendersWithGivenColumnDefinitions(createDefaultInstance, columnDefs, getAgGridInstance);
    });

    it('has input cells available', () => {
      const columnDefs = [
        AmcsDataGridColDefinitions.createLargeTextColDef({ field: 'make' }),
        AmcsDataGridColDefinitions.createTextColDef({ field: 'model' }),
      ];
      rendersWithGivenColumnDefinitions(createDefaultInstance, columnDefs, getAgGridInstance);
    });
  });

  describe('onCellValueChanged', () => {
    it('returns when errorfield changed', () => {
      const component = new AmcsDataGridComponent();

      const params = {
        colDef: {
          field: 'error',
        },
      } as CellValueChangedEvent;

      component.options = {
        errorField: 'error',
      };
      const result = component.onCellValueChanged(params);

      expect(result).toBeUndefined();
    });

    it('calls emitRowChangesAmount when not pasting', () => {
      const component = new AmcsDataGridComponent();
      const newValue = undefined;
      const oldValue = undefined;
      const nodeId = '100';
      const field = 'SomeDateField';
      spyOn(component.rowChangesAmount, 'emit').and.callFake(() => {});
      component.isPasting = false;

      callValueChanged(field, component, nodeId, oldValue, newValue, NumericCellRendererComponent);

      expect(component.rowChangesAmount.emit).toHaveBeenCalledTimes(1);
    });

    it('calls emitRowChangesAmount when not pasting', () => {
      const component = new AmcsDataGridComponent();
      const newValue = undefined;
      const oldValue = undefined;
      const nodeId = '100';
      const field = 'SomeDateField';
      spyOn(component.rowChangesAmount, 'emit').and.callFake(() => {});
      component.isPasting = true;

      callValueChanged(field, component, nodeId, oldValue, newValue, NumericCellRendererComponent);

      expect(component.rowChangesAmount.emit).not.toHaveBeenCalled();
    });

    describe('DateCellRendererComponent', () => {
      it('does not call storeEdit when no value changed', () => {
        const component = new AmcsDataGridComponent();
        const newValue = undefined;
        const oldValue = undefined;
        const nodeId = '100';
        const field = 'SomeDateField';

        callValueChanged(field, component, nodeId, oldValue, newValue, DateCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).not.toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('does not call storeEdit with same dates', () => {
        const component = new AmcsDataGridComponent();
        const newValue = new Date();
        const oldValue = new Date();
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, DateCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).not.toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('#newValue: calls storeEdit with different dates', () => {
        const component = new AmcsDataGridComponent();
        const newValue = new Date();
        const oldValue = new Date();
        oldValue.setDate(new Date().getDate() + 1);
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, DateCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('#oldValue: calls storeEdit with different dates', () => {
        const component = new AmcsDataGridComponent();
        const newValue = new Date();
        newValue.setDate(new Date().getDate() + 1);
        const oldValue = new Date();
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, DateCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).toHaveBeenCalledOnceWith(nodeId, field);
      });
    });

    describe('NumericCellRendererComponent', () => {
      it('does not call storeEdit when no value changed', () => {
        const component = new AmcsDataGridComponent();
        const newValue = undefined;
        const oldValue = undefined;
        const nodeId = '100';
        const field = 'SomeDateField';

        callValueChanged(field, component, nodeId, oldValue, newValue, NumericCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).not.toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('does not call storeEdit with same numbers', () => {
        const component = new AmcsDataGridComponent();
        const newValue = 10;
        const oldValue = 10;
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, NumericCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).not.toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('#newValue: calls storeEdit with different numbers', () => {
        const component = new AmcsDataGridComponent();
        const newValue = 10;
        const oldValue = 11;
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, NumericCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('#oldValue: calls storeEdit with different numbers', () => {
        const component = new AmcsDataGridComponent();
        const newValue = 11;
        const oldValue = 10;
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, NumericCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).toHaveBeenCalledOnceWith(nodeId, field);
      });
    });

    describe('CheckboxCellRendererComponent', () => {
      it('does not call storeEdit when undefined', () => {
        const component = new AmcsDataGridComponent();
        const newValue = undefined;
        const oldValue = undefined;
        const nodeId = '100';
        const field = 'SomeDateField';

        callValueChanged(field, component, nodeId, oldValue, newValue, CheckboxCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).not.toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('does not call storeEdit when both false', () => {
        const component = new AmcsDataGridComponent();
        const newValue = false;
        const oldValue = false;
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, CheckboxCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).not.toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('does not call storeEdit when both true', () => {
        const component = new AmcsDataGridComponent();
        const newValue = true;
        const oldValue = true;
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, CheckboxCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).not.toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('#newValue: calls storeEdit', () => {
        const component = new AmcsDataGridComponent();
        const newValue = false;
        const oldValue = true;
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, CheckboxCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).toHaveBeenCalledOnceWith(nodeId, field);
      });

      it('#oldValue: calls storeEdit', () => {
        const component = new AmcsDataGridComponent();
        const newValue = true;
        const oldValue = false;
        const nodeId = '100';
        const field = '';

        callValueChanged(field, component, nodeId, oldValue, newValue, CheckboxCellRendererComponent);

        expect(component.changedRowAmountHelper.storeEdit).toHaveBeenCalledOnceWith(nodeId, field);
      });
    });
  });
});

function callValueChanged(
  field: string,
  component: AmcsDataGridComponent,
  nodeId: string,
  oldValue: any,
  newValue: any,
  cellRenderer: any
) {
  component.changedRowAmountHelper = new ChangedRowAmountHelper();

  spyOn(component.changedRowAmountHelper, 'storeEdit').and.callFake(() => {});

  const params = {
    node: {
      id: nodeId,
    },
    oldValue,
    newValue,
    colDef: {
      field,
    },
    column: {
      getColDef: () => {
        return { cellRenderer };
      },
    },
  } as CellValueChangedEvent;

  component.options = {
    errorField: 'error',
  };
  const result = component.onCellValueChanged(params);

  expect(result).toBeUndefined();
}

function rendersWithGivenColumnDefinitions(createDefaultInstance, columnDefs: AmcsColDef[], getAgGridInstance: () => AgGridAngular) {
  const releaseDate = new Date();
  const rowData = [
    {
      make: 'Porsche',
      model: 'Macan',
      insured: false,
      price: 72000,
      released: releaseDate,
    },
    {
      make: 'Seat',
      model: 'Ateca',
      insured: true,
      price: 1000000,
      released: releaseDate.setMonth(1),
    },
  ];
  createDefaultInstance({
    columnDefs,
    rowData,
  });
  const gridInstance = getAgGridInstance();
  expect(gridInstance.columnDefs).toEqual(columnDefs);
  expect(gridInstance.rowData).toEqual(rowData);
}
