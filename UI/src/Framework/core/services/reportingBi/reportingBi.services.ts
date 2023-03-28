import { Injectable } from '@angular/core';
import { ReportHtml } from '@core-module/models/external-dependencies/reports/report-html.model';
import { Report } from '@core-module/models/external-dependencies/reports/report.model';
import { ReportingBiSearch } from '@coremodels/reportingBi-search.model';
import { BaseService } from '@coreservices/base.service';
import { ReportingBiDataService } from '@coreservices/reportingBi/data/reportingBi.data.Service';
import { environment } from '@environments/environment';
import { BehaviorSubject, Observable, ReplaySubject, Subject } from 'rxjs';
import { switchMap, takeUntil } from 'rxjs/operators';
import { ReportSessionRequest } from '../../models/reportingBi/report-session-request.model';
import { ReportSession } from '../../models/reportingBi/report-session.model';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/245825
 */
@Injectable({ providedIn: 'root' })
export class ReportingBiService extends BaseService {

  reportingBi$: Observable<any>;
  reportSession$: Observable<ReportSession>;
  reportDetail$: Observable<Report>;
  reportHtmlDetail$: Observable<ReportHtml>;
  reportExecutable$: Observable<ReportSession>;
  reportExecutableSearch$: Observable<Blob>;

  constructor(private reportingBiDataService: ReportingBiDataService) {
    super();
    this.checkExagoScriptUrl();
    this.setUpReportSessionStream();
    this.setUpReportDetailStream();
    this.setUpReportHtmlDetailStream();
    this.setUpReportBiStream();
    this.setUpReportExecutableStream();
    this.setUpReportExecutableSearchStream();
  }

  private exagoScriptUrlIsValid = false;
  private exagoScriptUrlErrorReported = false;

  private reportSession: Subject<ReportSession> = new Subject<ReportSession>();
  private reportSessionRequest: Subject<ReportSessionRequest> = new Subject<ReportSessionRequest>();

  private reportingBi: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  private reportDetail: ReplaySubject<Report> = new ReplaySubject<Report>(1);
  private reportDetailRequest: ReplaySubject<any> = new ReplaySubject<any>(1);

  private reportHtmlDetail: ReplaySubject<ReportHtml> = new ReplaySubject<ReportHtml>(1);
  private reportHtmlDetailRequest: ReplaySubject<any> = new ReplaySubject<any>(1);

  private reportExecutable: ReplaySubject<ReportSession> = new ReplaySubject<ReportSession>(1);
  private reportExecutableRequest: ReplaySubject<ReportingBiSearch> = new ReplaySubject<ReportingBiSearch>(1);

  private reportExecutableSearch: ReplaySubject<Blob> = new ReplaySubject<Blob>(1);
  private reportExecutableSearchRequest: ReplaySubject<ReportingBiSearch> = new ReplaySubject<ReportingBiSearch>(1);

  setReportBi(exago: any) {
    if (this.exagoScriptUrlIsValid) {
      this.reportingBi.next(exago);
    } else {
      this.reportExagoScriptUrlError();
    }
  }

  requestReportSession(request: ReportSessionRequest) {
    if (this.exagoScriptUrlIsValid) {
      this.reportSessionRequest.next(request);
    } else {
      this.reportExagoScriptUrlError();
    }
  }

  requestReportDetail() {
    if (this.exagoScriptUrlIsValid) {
      this.reportDetailRequest.next(null);
    } else {
      this.reportExagoScriptUrlError();
    }
  }

  requestReportHtmlDetail() {
    if (this.exagoScriptUrlIsValid) {
      this.reportHtmlDetailRequest.next(null);
    } else {
      this.reportExagoScriptUrlError();
    }
  }

  requestReportExecutable(reportingBiSearch: ReportingBiSearch) {
    if (this.exagoScriptUrlIsValid) {
      this.reportExecutableRequest.next(reportingBiSearch);
    } else {
      this.reportExagoScriptUrlError();
    }
  }

  requestReportExecutableSearch(reportingBiSearch: ReportingBiSearch) {
    this.reportExecutableSearchRequest.next(reportingBiSearch);
  }

  private setUpReportSessionStream() {
    this.reportSession$ = this.reportSession.asObservable();
    this.reportSessionRequest
      .pipe(
        switchMap(request => {
          return this.reportingBiDataService.getReportSession(request);
        })).subscribe((result: ReportSession) => {
          this.reportSession.next(result);
        });
  }

  private setUpReportExecutableStream() {
    this.reportExecutable$ = this.reportExecutable.asObservable();
    this.reportExecutableRequest
      .pipe(
        switchMap(data => {
          return this.reportingBiDataService.getReportExecutable(data.reportPath, data.reportFilter);
        })).subscribe((result: ReportSession) => {
          this.reportExecutable.next(result);
        });
  }

  private setUpReportExecutableSearchStream() {
    this.reportExecutableSearch$ = this.reportExecutableSearch.asObservable();
    this.reportExecutableSearchRequest
      .pipe(
        switchMap(data => {
          return this.reportingBiDataService.getReportPDF(data);
        })).subscribe((result: any) => {
          this.reportExecutableSearch.next(result);
        });
  }

  private setUpReportDetailStream() {
    this.reportDetail$ = this.reportDetail.asObservable();
    this.reportDetailRequest
      .pipe(
        takeUntil(this.unsubscribe),
        switchMap(data => {
          return this.reportingBiDataService.getReportDetail();
        })).subscribe((result: Report) => {
          this.reportDetail.next(result);
        });
  }

  private setUpReportHtmlDetailStream() {
    this.reportHtmlDetail$ = this.reportHtmlDetail.asObservable();
    this.reportHtmlDetailRequest
      .pipe(
        takeUntil(this.unsubscribe),
        switchMap(data => {
          const reportName = 'Global/Communication Html';
          const exportType = 'html-clean';
          const reportFilter = 'Communication.CustomerId eq 602';
          return this.reportingBiDataService.getReportHtmlDetail(reportName, exportType, reportFilter);
        })).subscribe((result: ReportHtml) => {
          this.reportHtmlDetail.next(result);
        });
  }

  private setUpReportBiStream() {
    this.reportingBi$ = this.reportingBi.asObservable();
  }

  private checkExagoScriptUrl() {
    try {
      // this should throw if not a valid URL
      const url = new URL(environment.exagoApiScriptUrl);
      this.exagoScriptUrlIsValid = (url !== null);
    } catch (_) {
    }
  }

  private reportExagoScriptUrlError() {
    if (!this.exagoScriptUrlErrorReported) {
      console.log('Invalid Exago script URL');
      this.exagoScriptUrlErrorReported = true;
    }
  }
}
