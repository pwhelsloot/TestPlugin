import { IApiRequestGetOptions } from '@core-module/models/api/api-request-get-options.interface';
import { ApiOptionsEnum } from '@coremodels/api/api-options.enum';
import { IncludeLinksEnum } from '@coremodels/api/include-links.enum';
import { APIRequestFormatterService } from '@coreservices/api-request-formatter.service';
import { FilterOperation } from './filters/filter-operation.enum';
import { IFilter } from './filters/iFilter';

export class ApiRequest implements IApiRequestGetOptions {
  urlResourcePath: (string | number)[] = [];
  searchTerms: string[] = [];
  filters: IFilter[] = [];

  max: number;
  key: string;
  includeCount: boolean;
  includeDeleted: boolean;
  page: number;
  returnUrl: string;
  postData: string;
  linkOptions: IncludeLinksEnum[] = [];
  apiOptions: ApiOptionsEnum = ApiOptionsEnum.core;
  suppressErrorModal: boolean;

  constructor(options?: IApiRequestGetOptions) {
    this.searchTerms = options?.searchTerms ? options.searchTerms : [];
    this.max = options?.max;
    this.includeCount = options?.includeCount;
    this.includeDeleted = options?.includeDeleted;
    this.page = options?.page;
    this.suppressErrorModal = options?.suppressErrorModal !== undefined ? options?.suppressErrorModal : false;
  }

  getFilters(): string {
    if (this.filters.length === 0) {
      return '';
    }

    const filterTexts: string[] = [];

    for (const filter of this.filters) {
      switch (filter.filterOperation) {
        case FilterOperation.Between:
          filterTexts.push(
            `${filter.name} from '${APIRequestFormatterService.formatDateToUTC(
              filter.value[0]
            )}' to '${APIRequestFormatterService.formatDateToUTC(filter.value[1])}'`
          );
          break;
        case FilterOperation.In:
          filterTexts.push(`${filter.name} ${this.getFilterOperationAsString(filter.filterOperation)} (${filter.value})`);
          break;
        default:
          if (filter.value && typeof filter.value === 'string') {
            // RDM - Sanitising filter strings. The string may contain apostrophes, this escapes them by..
            // 1. `'${filter.value}'` - Adds start/end apostrophes to the string.
            // 2. new RegExp('\'', 'g'), '\'\'') - Doubles every apostrophe/quotes in the string.
            // 3. new RegExp('^\'|\'$', 'g'), '') - Removes first/last apostrophe.
            //
            // The end result of this is essentially the string is wrapped in single apostrophes with every apostrophe within it doubled
            filter.value = `'${filter.value}'`.replace(new RegExp('\'', 'g'), '\'\'').replace(new RegExp('^\'|\'$', 'g'), '');
          } else if (filter.value && filter.value instanceof Date) {
            filter.value = `'${APIRequestFormatterService.formatDateToUTC(filter.value)}'`;
          }
          filterTexts.push(`${filter.name} ${this.getFilterOperationAsString(filter.filterOperation)} ${filter.value}`);
          break;
      }
    }
    return filterTexts.join(' and ');
  }

  getLinkOptionsAsString(): string {
    if (this.linkOptions != null && this.linkOptions.length > 0) {
      const linkOptionStrings: string[] = [];
      this.linkOptions.forEach((element) => {
        switch (element) {
          case IncludeLinksEnum.allowed:
            linkOptionStrings.push('allowed');
            break;
          case IncludeLinksEnum.associations:
            linkOptionStrings.push('associations');
            break;
          case IncludeLinksEnum.attached:
            linkOptionStrings.push('attached');
            break;
          case IncludeLinksEnum.operations:
            linkOptionStrings.push('operations');
            break;
          case IncludeLinksEnum.self:
            linkOptionStrings.push('self');
            break;
        }
      });
      return linkOptionStrings.join(', ');
    } else {
      return null;
    }
  }

  getFilterOperationAsString(filterOperation: FilterOperation): string {
    switch (+filterOperation) {
      case FilterOperation.Equal:
        return 'eq';
      case FilterOperation.GreaterThan:
        return 'gt';
      case FilterOperation.GreaterThanOrEqual:
        return 'gte';
      case FilterOperation.LessThan:
        return 'lt';
      case FilterOperation.LessThanOrEqual:
        return 'lte';
      case FilterOperation.NotEqual:
        return 'ne';
      case FilterOperation.StartsWith:
        return 'startswith';
      case FilterOperation.EndsWith:
        return 'endsWith';
      case FilterOperation.In:
        return 'in';
      case FilterOperation.Contains:
        return 'contains';
      default:
        return 'eq';
    }
  }

  getUrlResourcePath(): string {
    let params = '';
    for (const param of this.urlResourcePath) {
      params = params + param + '/';
    }

    if (this.searchTerms.length > 0) {
      params = params + '?search=';
    }

    for (const search of this.searchTerms) {
      params = params + search;
    }

    return params;
  }
}
