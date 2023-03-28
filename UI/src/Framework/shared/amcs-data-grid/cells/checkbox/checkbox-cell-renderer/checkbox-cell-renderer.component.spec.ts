/* tslint:disable:no-unused-variable */
import { CheckboxCellRendererComponent } from './checkbox-cell-renderer.component';
import { ICellRendererParams } from 'ag-grid-enterprise';
import { AmcsDataGridModule } from '@shared-module/amcs-data-grid/amcs-data-grid.module';
import { MockRenderFactory, ngMocks, MockBuilder } from 'ng-mocks';

describe('CheckboxCellRendererComponent', () => {
  const factory = MockRenderFactory(CheckboxCellRendererComponent, []);
  ngMocks.faster();

  beforeEach(() => MockBuilder(CheckboxCellRendererComponent, AmcsDataGridModule));
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
    const value = false;
    const params = {
      value,
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

    expect(fixture).toBeTruthy();
  });

  it('inits with value', () => {
    const { fixture, params } = createDefaultFixture();
    fixture.agInit(params);

    expect(fixture.cellValue).toBe(params.value);
  });

  it('refresh sets value from params', () => {
    const newValue = true;
    testRefreshValue(newValue);
  });

  describe('coercion', () => {
    it('accepts true as string as lowercase', () => {
      const newValue = 'true';
      const { fixture, refreshed } = testRefreshValue(newValue);

      expect(fixture.cellValue).toBeTrue();
      expect(refreshed).toBeTrue();
    });

    it('accepts true as string as uppercase', () => {
      const newValue = 'TRUE';
      const { fixture, refreshed } = testRefreshValue(newValue);

      expect(fixture.cellValue).toBeTrue();
      expect(refreshed).toBeTrue();
    });

    it('accepts true as boolean', () => {
      const newValue = true;
      const { fixture, refreshed } = testRefreshValue(newValue);

      expect(fixture.cellValue).toBeTrue();
      expect(refreshed).toBeTrue();
    });

    it('accepts false as string as lowercase', () => {
      const newValue = 'false';
      const { fixture, refreshed } = testRefreshValue(newValue);

      expect(fixture.cellValue).toBeFalse();
      expect(refreshed).toBeTrue();
    });

    it('accepts false as string as uppercase', () => {
      const newValue = 'FALSE';
      const { fixture, refreshed } = testRefreshValue(newValue);

      expect(fixture.cellValue).toBeFalse();
      expect(refreshed).toBeTrue();
    });

    it('accepts false as boolean', () => {
      const newValue = false;
      const { fixture, refreshed } = testRefreshValue(newValue);

      expect(fixture.cellValue).toBeFalse();
      expect(refreshed).toBeTrue();
    });
  });
});
