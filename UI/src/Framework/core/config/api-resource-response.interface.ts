import { ApiResourceExtrasResponse } from './api-resource-extras-response.interface';
export interface ApiResourceResponse {
    resource: any | any[] | number;
    extra: ApiResourceExtrasResponse;
    errors: string;
}
