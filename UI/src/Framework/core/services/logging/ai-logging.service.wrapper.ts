import { Injectable } from '@angular/core';
import { AiLoggingService } from '@core-module/services/logging/ai-logging.service';

@Injectable({ providedIn: 'root'})
export class AiLoggingServiceWrapper {
    constructor(readonly aiLoggingService: AiLoggingService) {
        AiLoggingService.aiLoggingServiceReference = aiLoggingService;
    }
}
