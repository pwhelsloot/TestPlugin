import { HttpClient, HttpRequest, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { LanguageInterceptor } from '@core-module/http/language.interceptor';
import { AppLocaleService } from '@translate/app-locale.service';
import { MockProvider } from 'ng-mocks';

describe('LanuageInterceptor', () => {
  let locale: string;
  let browserLocale: string;
  let localeService: AppLocaleService;
  let client: HttpClient;
  let httpMock: HttpTestingController;
  let service: LanguageInterceptor;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [{ provide: HTTP_INTERCEPTORS, useClass: LanguageInterceptor, multi: true }, MockProvider(AppLocaleService)],
    });
    const interceptors = TestBed.inject(HTTP_INTERCEPTORS);
    service = interceptors.find((interceptor) => interceptor instanceof LanguageInterceptor) as LanguageInterceptor;
    service.getBrowserLocale = () => {
      return browserLocale;
    };
    localeService = TestBed.inject(AppLocaleService);
    localeService.getLocale = () => {
      return locale;
    };
    client = TestBed.inject(HttpClient);
    httpMock = TestBed.inject(HttpTestingController);
  });

  describe('#intercept', () => {
    it('adds Accept-Language header to all requests', () => {
      // act
      client.get('/test').subscribe(() => {});

      // assert
      const request: HttpRequest<any> = httpMock.expectOne('/test').request;
      expect(request.headers.get('Accept-Language')).toBeDefined();
    });

    it('Accept-Language header includes locale, browser locale and *', () => {
      // arrange
      locale = 'en';
      browserLocale = 'en-gb';
      spyOn(service, 'buildAcceptLanguage').and.callThrough();
      spyOn(service, 'getBrowserLocale').and.callThrough();

      // act
      client.get('/test').subscribe(() => {});

      // assert
      const request: HttpRequest<any> = httpMock.expectOne('/test').request;
      expect(service.buildAcceptLanguage).toHaveBeenCalledOnceWith(locale);
      expect(service.getBrowserLocale).toHaveBeenCalledOnceWith();
      expect(request.headers.get('Accept-Language')).toEqual(`${locale}, ${browserLocale};q=0.9, *;q=0.5`);
    });

    it('Accept-Language header removes browser locale if same as locale', () => {
      // arrange
      locale = 'en-gb';
      browserLocale = 'en-gb';
      spyOn(service, 'buildAcceptLanguage').and.callThrough();
      spyOn(service, 'getBrowserLocale').and.callThrough();

      // act
      client.get('/test').subscribe(() => {});

      // assert
      const request: HttpRequest<any> = httpMock.expectOne('/test').request;
      expect(service.buildAcceptLanguage).toHaveBeenCalledOnceWith(locale);
      expect(service.getBrowserLocale).toHaveBeenCalledOnceWith();
      expect(request.headers.get('Accept-Language')).toEqual(`${locale}, *;q=0.5`);
    });
  });
});
