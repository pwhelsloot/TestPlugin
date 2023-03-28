/* tslint:disable:no-unused-variable */
import { DatePipe } from '@angular/common';
import { AmcsDataGridModule } from '@shared-module/amcs-data-grid/amcs-data-grid.module';
import { ICellRendererParams } from 'ag-grid-community';
import { MockBuilder, MockRenderFactory, ngMocks } from 'ng-mocks';
import { DateCellRendererComponent, dateCellValueFormatter } from './date-cell-renderer.component';

describe('DateCellRendererComponent', () => {
  const factory = MockRenderFactory(DateCellRendererComponent, [], {
    providers: [DatePipe],
  });
  ngMocks.faster();

  beforeEach(() => MockBuilder(DateCellRendererComponent, AmcsDataGridModule));
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
    const date = new Date();
    const params = {
      value: date,
    } as ICellRendererParams;

    fixture.agInit(params);
    return { fixture, params };
  }

  function testRefreshValue(newValue: any) {
    const { fixture, params } = createDefaultFixture();
    params.value = newValue;
    return { fixture, refreshed: fixture.refresh(params) };
  }

  it('should create', () => {
    const { fixture, params } = createDefaultFixture();
    expect(fixture.noMargin).toBeTrue();
    expect(fixture.noPadding).toBeTrue();
    expect(fixture.disabled).toBeTrue();
    expect(fixture.readonly).toBeTrue();
    expect(fixture).toBeTruthy();
  });

  it('inits with value', () => {
    const { fixture, params } = createDefaultFixture();

    let datePipe = ngMocks.findInstance(DatePipe);

    expect(fixture.cellValue).toEqual(datePipe.transform(params.value));
  });

  it('refresh sets value from params', () => {
    const newDate = new Date();
    const { fixture, refreshed } = testRefreshValue(newDate);

    let datePipe = ngMocks.findInstance(DatePipe);

    expect(fixture.cellValue).toEqual(datePipe.transform(newDate));
    expect(refreshed).toBeTrue();
  });

  it('refresh accepts date as string', () => {
    const newDate = 'Mon Jul 18 2022 09:38:04 GMT+0200 (Midden-Europese zomertijd)';
    const { fixture, refreshed } = testRefreshValue(newDate);

    const expectedDate = new Date(newDate);

    let datePipe = ngMocks.findInstance(DatePipe);

    expect(fixture.cellValue).toEqual(datePipe.transform(expectedDate));
    expect(refreshed).toBeTrue();
  });

  describe('dateCellValueFormatter', () => {
    it(`returns valid date with date string`, () => {
      const date = new Date().toDateString();

      const result = dateCellValueFormatter(date) as any;

      expect(result).not.toBe(null);
    });

    it(`returns valid date with date`, () => {
      const date = new Date();

      const result = dateCellValueFormatter(date) as any;

      expect(result).not.toBe(null);
    });

    it(`returns 'null' with invalid date`, () => {
      const expected = null;

      const date = '123 nope';

      const result = dateCellValueFormatter(date);

      expect(result).toBe(expected);
    });

    it(`returns 'null' with null date`, () => {
      const expected = null;

      const date = null;

      const result = dateCellValueFormatter(date);

      expect(result).toBe(expected);
    });
  });
});
