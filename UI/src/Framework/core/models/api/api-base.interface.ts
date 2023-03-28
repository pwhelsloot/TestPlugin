import { ApiBaseModel } from './api-base.model';
export interface IApiBaseModel {
    parse<T extends ApiBaseModel>(json: any, type: (new () => T)): T;
    parseArray<T extends ApiBaseModel>(json: any, type: (new () => T)): T[];
    post<T extends ApiBaseModel>(model: T, type: (new () => T)): string;
    postArray<T extends ApiBaseModel>(models: T[], type: (new () => T)): string;
    filterByValues(filterTerm: string, fieldValuesToSearch: (string | number)[]): boolean;
}
