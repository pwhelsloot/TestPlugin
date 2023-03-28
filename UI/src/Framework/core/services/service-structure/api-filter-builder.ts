import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { IFilter } from './../../models/api/filters/iFilter';
import { ApiFilter, ApiFilterValue } from './api-filter';

/**
 * Used to create Filters for API services
 *
 * @export
 * @class ApiFilters
 */
export class ApiFilters {
  private filters = new Array<IFilter>();

  build(): IFilter[] {
    return this.filters;
  }

  add(filter: IFilter): ApiFilters {
    this.pushFilter(filter);
    return this;
  }

  equal(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.equal(name, value));
    return this;
  }

  notEqual(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.notEqual(name, value));
    return this;
  }

  greaterThan(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.greaterThan(name, value));
    return this;
  }

  greaterThanOrEqual(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.greaterThanOrEqual(name, value));
    return this;
  }

  lessThan(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.lessThan(name, value));
    return this;
  }

  lessThanOrEqual(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.lessThanOrEqual(name, value));
    return this;
  }

  startsWith(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.startsWith(name, value));
    return this;
  }

  endsWith(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.endsWith(name, value));
    return this;
  }

  contains(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.contains(name, value));
    return this;
  }

  in(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.in(name, value));
    return this;
  }

  between(name: string, value: ApiFilterValue): ApiFilters {
    this.pushFilter(ApiFilter.between(name, value));
    return this;
  }

  private pushFilter(filter: IFilter) {
    if (filter && isTruthy(filter.value)) {
      this.filters.push(filter);
    } else if (filter && (filter.filterOperation === FilterOperation.Equal ||
      filter.filterOperation === FilterOperation.NotEqual)) {
      this.filters.push(filter);
    }
  }
}
