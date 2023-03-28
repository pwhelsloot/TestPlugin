import { ApiResourceSelfResponse } from './api-resource-self-response.interface';
export interface ApiResourceLinksResponse {
    self: ApiResourceSelfResponse;
    operations: any[];
    associations: any[];
    allowed: ApiResourceSelfResponse[];
    attached: ApiResourceSelfResponse[];
}
