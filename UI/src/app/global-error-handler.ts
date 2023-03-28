import { ErrorHandler, Injectable } from '@angular/core';
import { ErrorNotificationService } from '@coreservices/error-notification.service';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';

@Injectable()
export class GlobalErrorHandler extends ErrorHandler {
  constructor(private errorNotificationService: ErrorNotificationService, private instrumentationService: InstrumentationService) {
    super();
  }

  handleError(error) {
    this.instrumentationService.trackException(error, LoggingVerbs.ErrorGlobal, {
      error: error.message ? error.message : error.toString(),
      originalError: error.originalError ? error.originalError : '',
      stack: error.stack ? error.stack : '',
    });

    this.errorNotificationService.notifyError(error.message ? error.message : error.toString());

    // keep the default behaviour (which is to log to the console)
    // AJM TO DO - THIS SHOULD BE TAKEN AWAY?
    super.handleError(error);
  }
}
