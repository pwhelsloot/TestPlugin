import { ApiRequestTracking } from '@core-module/models/api/api-request-tracking.interface';

export interface AiViewApiRequest extends ApiRequestTracking {
  viewName: string;
}
