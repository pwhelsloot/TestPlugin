import { FilterOperation } from '../../models/api/filters/filter-operation.enum';
import { IFilter } from '../../models/api/filters/iFilter';

export class ApiFilter {
  static equal(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.Equal, name, value);
  }

  static notEqual(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.NotEqual, name, value);
  }

  static greaterThan(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.GreaterThan, name, value);
  }

  static greaterThanOrEqual(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.GreaterThanOrEqual, name, value);
  }

  static lessThan(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.LessThan, name, value);
  }

  static lessThanOrEqual(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.LessThanOrEqual, name, value);
  }

  static startsWith(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.StartsWith, name, value);
  }

  static endsWith(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.EndsWith, name, value);
  }

  static contains(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.Contains, name, value);
  }

  static in(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.In, name, value);
  }

  static between(name: string, value: ApiFilterValue): IFilter {
    return this.create(FilterOperation.Between, name, value);
  }

  static create(filterOperation: FilterOperation, name: string, value: ApiFilterValue) {
    return { filterOperation, name, value } as IFilter;
  }
}

export type ApiFilterValue = string | number | Date | [] | boolean | [Date, Date];
