import { FilterOperation } from './filter-operation.enum';
export interface IFilter {
    filterOperation: FilterOperation;
    name: string;
    value: any;
}
