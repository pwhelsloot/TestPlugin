import { ApiFilter } from '@core-module/services/service-structure/api-filter';
import { FilterOperation } from '../../models/api/filters/filter-operation.enum';
import { ApiFilters } from './api-filter-builder';

describe('ApiFilterBuilder', () => {
  let builder: ApiFilters;
  let expectedProperty: string;
  let expectedValue: string;
  beforeEach(() => {
    builder = new ApiFilters();
  });

  it('outputs on build', () => {
    const output = builder.build();
    expect(output).toBeDefined();
    expect(output).toEqual([]);
  });

  it('starts empty', () => {
    const output = builder.build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(0);
  });

  it('can add filters', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.add(ApiFilter.equal(expectedProperty, expectedValue)).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.Equal);
  });

  it('can add equal filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.equal(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.Equal);
  });

  it('can add notEqual filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.notEqual(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.NotEqual);
  });

  it('can add endsWith filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.endsWith(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.EndsWith);
  });

  it('can add greaterThan filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.greaterThan(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.GreaterThan);
  });

  it('can add greaterThanOrEqual filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.greaterThanOrEqual(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.GreaterThanOrEqual);
  });

  it('can add in filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.in(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.In);
  });

  it('can add lessThan filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.lessThan(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.LessThan);
  });

  it('can add lessThanOrEqual filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.lessThanOrEqual(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.LessThanOrEqual);
  });

  it('can add startsWith filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.startsWith(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.StartsWith);
  });

  it('can add contains filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.contains(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.Contains);
  });

  it('can add between filter', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';
    const output = builder.between(expectedProperty, expectedValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(1);

    const filter = output[0];
    expect(filter).toBeDefined();
    expect(filter.name).toBe(expectedProperty);
    expect(filter.value).toBe(expectedValue);
    expect(filter.filterOperation).toBe(FilterOperation.Between);
  });

  it('can add multiple filters', () => {
    expectedProperty = 'SomeProperty';
    expectedValue = 'SomeValue';

    const betweenProperty = 'Date';
    const betweenValue = '12-2-2021';
    const output = builder.contains(expectedProperty, expectedValue).between(betweenProperty, betweenValue).build();
    expect(output).toBeDefined();
    expect(output).toHaveSize(2);

    const containsFilter = output[0];
    expect(containsFilter).toBeDefined();
    expect(containsFilter.name).toBe(expectedProperty);
    expect(containsFilter.value).toBe(expectedValue);
    expect(containsFilter.filterOperation).toBe(FilterOperation.Contains);

    const betweenFilter = output[1];
    expect(betweenFilter).toBeDefined();
    expect(betweenFilter.name).toBe(betweenProperty);
    expect(betweenFilter.value).toBe(betweenValue);
    expect(betweenFilter.filterOperation).toBe(FilterOperation.Between);
  });
});
