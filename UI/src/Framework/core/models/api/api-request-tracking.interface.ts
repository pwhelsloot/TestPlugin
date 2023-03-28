import { AiLoggingRequestType } from '@core-module/services/logging/ai-logging-request-type.constants';

export interface ApiRequestTracking {
  id: number;
  url: string;
  type: AiLoggingRequestType;
}
