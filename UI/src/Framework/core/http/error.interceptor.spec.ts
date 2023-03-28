import { HttpClient, HttpErrorResponse, HTTP_INTERCEPTORS } from '@angular/common/http';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { Router } from '@angular/router';
import * as AuthActions from '@auth-module/store/auth.actions';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { PreviousRouteService } from '@coreservices/previous-route.service';
import { Action, Store } from '@ngrx/store';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { AmcsModalConfig } from '../../shared/components/amcs-modal/amcs-modal-config.model';
import { ErrorInterceptor } from './error.interceptor';

describe('Service: ErrorInterceptor', () => {
  let client: HttpClient;
  let httpMock: HttpTestingController;
  let instrumentationService: InstrumentationService;
  let store: Store;
  let router: Router;
  let previousRouteService: PreviousRouteService;

  beforeEach(() => {
    const routerMock = () => ({ navigate: array => ({}) });
    const instrumentationServiceMock = () => ({
      trackException: (error, errorHttp, object) => ({})
    });
    const previousRouteServiceMock = () => ({ removeCurrentUrl: () => ({}) });
    const amcsModalServiceMock = () => ({
      createModal: (config: AmcsModalConfig) => ({
        afterClosed: () => ({ pipe: () => ({ subscribe: sub => sub({}) }) })
      })
    });
    const storeMock = () => ({ dispatch: (action: Action) => ({}) });

    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [
        { provide: Router, useFactory: routerMock },
        {
          provide: InstrumentationService,
          useFactory: instrumentationServiceMock
        },
        { provide: PreviousRouteService, useFactory: previousRouteServiceMock },
        { provide: Store, useFactory: storeMock },
        { provide: AmcsModalService, useFactory: amcsModalServiceMock },
        { provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true }
      ]
    });

    client = TestBed.inject(HttpClient);
    store = TestBed.inject(Store);
    httpMock = TestBed.inject(HttpTestingController);
    instrumentationService = TestBed.inject(InstrumentationService);
    router = TestBed.inject(Router);
    previousRouteService = TestBed.inject(PreviousRouteService);
  });

  describe('#intercept', () => {
    it('tracks exception on error', () => {
      const body = { property: 'track exception' };
      spyOn(instrumentationService, 'trackException').and.callThrough();

      client.get('/test').subscribe(() => { }, (response: HttpErrorResponse) => {
        expect(response.status).toEqual(500);
      });
      const request = httpMock.expectOne('/test');
      request.flush(body, { status: 500, statusText: 'poof' });

      expect(instrumentationService.trackException).toHaveBeenCalled();
      expect(instrumentationService.trackException).toHaveBeenCalledTimes(1);
    });

    it('call signout if authStatus fails and complete', () => {
      const body = { property: 'signout authstatus' };
      spyOn(store, 'dispatch').and.callThrough();

      client.get('/authStatus').subscribe(fail);
      const request = httpMock.expectOne('/authStatus');
      request.flush(body, { status: 500, statusText: 'poof' });

      expect(store.dispatch).toHaveBeenCalledOnceWith(new AuthActions.TrySignout());
    });

    it('call signout on 401 and complete', () => {
      const body = { property: 'signout 404' };
      spyOn(store, 'dispatch').and.callThrough();

      client.get('/someUrl').subscribe(fail);
      const request = httpMock.expectOne('/someUrl');
      request.flush(body, { status: 401, statusText: 'poof' });

      expect(store.dispatch).toHaveBeenCalledOnceWith(new AuthActions.TrySignout(window.parent.location.href));
    });

    it('do not throw 404 on Exago call', () => {
      const body = { property: 'exago prop' };

      const sub = client.get(`${window.coreServiceRoot}/exago/`).subscribe();
      const request = httpMock.expectOne(`${window.coreServiceRoot}/exago/`);
      request.flush(body, { status: 404, statusText: 'poof' });

      expect(sub.closed).toBeTrue();
    });

    it('do not throw 404 on Core API apps call', () => {
      const body = { property: 'core api prop' };

      const sub = client.get(`${window.coreServiceRoot}/services/api/apps/`).subscribe();
      const request = httpMock.expectOne(`${window.coreServiceRoot}/services/api/apps/`);
      request.flush(body, { status: 404, statusText: 'poof' });

      expect(sub.closed).toBeTrue();
    });

    it('do not throw 500 on bireports post', () => {
      const body = { property: 'bireports prop' };

      const sub = client.post(`${window.coreServiceRoot}/bireports/`, {}).subscribe();
      const request = httpMock.expectOne(`${window.coreServiceRoot}/bireports/`);
      request.flush(body, { status: 500, statusText: 'poof' });

      expect(sub.closed).toBeTrue();
    });

    it('redirect to not found on 404 and complete', () => {
      const body = { property: 'something' };
      spyOn(router, 'navigate').and.callThrough();
      spyOn(previousRouteService, 'removeCurrentUrl').and.callThrough();

      const sub = client.get(`${window.coreServiceRoot}/someApi/`).subscribe();
      const request = httpMock.expectOne(`${window.coreServiceRoot}/someApi/`);
      request.flush(body, { status: 404, statusText: 'poof' });

      expect(router.navigate).toHaveBeenCalledOnceWith([CoreAppRoutes.notFound]);
      expect(previousRouteService.removeCurrentUrl).toHaveBeenCalled();
      expect(previousRouteService.removeCurrentUrl).toHaveBeenCalledTimes(1);
      expect(sub.closed).toBeTrue();
    });

    it('throw on any non handled non 200 request', () => {
      const body = { property: 'unhandled' };
      const statusText = 'poof';
      client.get(`${window.coreServiceRoot}/unhandledApi/`).subscribe(() => {

      }, (error: HttpErrorResponse) => {
        expect(error).toBeDefined();
        expect(error.statusText).toEqual(statusText);
        expect(error.status).toEqual(500);
      });
      const request = httpMock.expectOne(`${window.coreServiceRoot}/unhandledApi/`);
      request.flush(body, { status: 500, statusText });
    });
  });
});
