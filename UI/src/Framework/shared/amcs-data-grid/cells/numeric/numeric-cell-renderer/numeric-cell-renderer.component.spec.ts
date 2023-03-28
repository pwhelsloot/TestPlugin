/* tslint:disable:no-unused-variable */
import { NumericCellRendererComponent } from './numeric-cell-renderer.component';
import { ICellRendererParams } from 'ag-grid-enterprise';
import { AmcsDataGridModule } from '@shared-module/amcs-data-grid/amcs-data-grid.module';
import { MockRenderFactory, ngMocks, MockBuilder } from 'ng-mocks';
import { DecimalPipe } from '@angular/common';

describe('NumericCellRendererComponent', () => {
  const factory = MockRenderFactory(NumericCellRendererComponent, [], {
    providers: [DecimalPipe],
  });
  ngMocks.faster();
  beforeEach(() => MockBuilder(NumericCellRendererComponent, AmcsDataGridModule));
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
    const value = 12341234;
    const params = {
      value,
    } as ICellRendererParams;

    return { fixture, params };
  }

  it('should create', () => {
    const { fixture, params } = createDefaultFixture();

    expect(fixture).toBeTruthy();
    expect(fixture.noMargin).toBeTrue();
    expect(fixture.noPadding).toBeTrue();
    expect(fixture.isDisabled).toBeTrue();
  });

  it('inits with value', () => {
    const { fixture, params } = createDefaultFixture();
    fixture.agInit(params);

    let decimalPipe = ngMocks.findInstance(DecimalPipe);
    expect(fixture.cellValue).toBe(decimalPipe.transform(params.value));
  });

  it('refresh sets value from params', () => {
    const { fixture, params } = createDefaultFixture();
    fixture.agInit(params);

    const newNumber = 12345;
    params.value = newNumber;
    const refreshed = fixture.refresh(params);

    let decimalPipe = ngMocks.findInstance(DecimalPipe);
    expect(fixture.cellValue).toBe(decimalPipe.transform(newNumber));
    expect(refreshed).toBeTrue();
  });
});
