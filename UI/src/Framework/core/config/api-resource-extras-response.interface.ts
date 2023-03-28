import { ApiResourceLinksResponse } from './api-resource-links-response.interface';
export interface ApiResourceExtrasResponse {
    links: ApiResourceLinksResponse;
    count: number;
}
