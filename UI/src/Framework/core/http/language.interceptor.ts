import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AppLocaleService } from '@translate/app-locale.service';
import { Observable } from 'rxjs';

@Injectable()
export class LanguageInterceptor implements HttpInterceptor {
  constructor(private readonly localeService: AppLocaleService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    let clonedRequest = null;
    clonedRequest = req.clone({
      headers: req.headers.append('Accept-Language', this.buildAcceptLanguage(this.localeService.getLocale())),
    });
    return next.handle(clonedRequest);
  }

  buildAcceptLanguage(locale: string): string {
    let browserLocale = this.getBrowserLocale();
    if (browserLocale !== locale) {
      browserLocale = ` ${browserLocale};q=0.9, `;
    } else {
      browserLocale = ' ';
    }
    return `${locale},${browserLocale}*;q=0.5`;
  }

  getBrowserLocale(): string {
    return navigator.language;
  }
}
