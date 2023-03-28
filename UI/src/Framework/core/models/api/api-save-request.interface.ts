import { ApiRequest } from '@core-module/models/api/api-request.model';

export interface IApiSaveRequest {
    req: ApiRequest;
    urlResourcePath: string;
}
