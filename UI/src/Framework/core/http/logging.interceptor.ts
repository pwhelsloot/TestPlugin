
import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';

@Injectable()
export class LoggingInterceptor implements HttpInterceptor {

    intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        return next.handle(req).pipe(
            tap(
                event => {
                    // if (environment.appInsights.consoleLoggingOn) {
                    //     // there really is no point in logging to console here as have network tab but developers can turn back on if they wish
                    //     // would need to add instrumentationService to constructor params
                    //     // this.instrumentationService.logEvent(LoggingVerbs.RequestHttp,
                    //     // {
                    //     //     url: req.url,
                    //     //     method: req.method,
                    //     //     urlWithParams: req.urlWithParams,
                    //     //     body: req.body
                    //     // });
                    // }
                }
            )
        );
    }
}
