import { Injectable } from '@angular/core';
import { ReportHtml } from '@core-module/models/external-dependencies/reports/report-html.model';
import { Report } from '@core-module/models/external-dependencies/reports/report.model';
import { ReportingBiSearch } from '@core-module/models/reportingBi-search.model';
import { ApiResourceResponse } from '@coreconfig/api-resource-response.interface';
import { ApiOptionsEnum } from '@coremodels/api/api-options.enum';
import { ApiRequest } from '@coremodels/api/api-request.model';
import { ErpApiService } from '@coreservices/erp-api.service';
import { of } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { ReportSessionRequest } from '../../../models/reportingBi/report-session-request.model';
import { ReportSessionResponse } from '../../../models/reportingBi/report-session-response.model';
import { ReportSession } from '../../../models/reportingBi/report-session.model';
import { EnhancedErpApiService } from '../../enhanced-erp-api.service';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/245825
 */
@Injectable({ providedIn: 'root' })
export class ReportingBiDataService {

  constructor(private erpApiService: ErpApiService,
    private enhancedErpApiService: EnhancedErpApiService) {
  }

  getReportDetail() {
    const getReportDetailMap =
      (response: ApiResourceResponse) => {
        const data = response;
        const report: Report = new Report();
        report.BaseUrl = data['BaseUrl'];
        report.ApiKey = data['ApiKey'];
        return report;
      };
    const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban

    apiRequest.filters = [];
    apiRequest.apiOptions = ApiOptionsEnum.core;
    apiRequest.urlResourcePath = ['bireports/createApiKey'];

    return this.erpApiService.postRequest<ApiResourceResponse>(apiRequest, getReportDetailMap);
  }

  getReportHtmlDetail(reportPath: string, exportType: string, reportFilter: string) {
    const getReportHtmlDetailMap =
      (response: string) => {
        const data = response;
        const report: ReportHtml = new ReportHtml();
        report.reportContent = data;
        return report;
      };
    const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
    const param = { ReportPath: reportPath, ExportType: exportType, ReportFilter: reportFilter };
    apiRequest.postData = JSON.stringify(param).slice(1, -1);
    apiRequest.apiOptions = ApiOptionsEnum.core;
    apiRequest.urlResourcePath = ['bireports/executeReport'];

    return this.erpApiService.postHtmlRequest(apiRequest, getReportHtmlDetailMap);
  }

  getReportPDF(reportSearch: ReportingBiSearch) {
    const mapFunction =
      (response: Blob) => {
        return response;
      };
    const errorMapFunction =
      (error: any) => {
        return null;
      };
    const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
    apiRequest.postData = JSON.stringify(reportSearch).slice(1, -1);
    apiRequest.apiOptions = ApiOptionsEnum.core;
    apiRequest.urlResourcePath = ['bireports/executeReport'];

    return this.erpApiService.postBlobRequest(apiRequest, mapFunction, errorMapFunction);
  }

  getReportExecutable(reportPath: string, reportFilter: string) {
    const getReportExecuatableMap =
      (response: ApiResourceResponse) => {
        const data = response;
        const reportSession: ReportSession = new ReportSession();
        reportSession.Url = data['Url'];
        return reportSession;
      };
    const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
    const param = { ReportPath: reportPath, ReportFilter: reportFilter };
    apiRequest.postData = JSON.stringify(param).slice(1, -1);
    apiRequest.apiOptions = ApiOptionsEnum.core;
    apiRequest.urlResourcePath = ['bireports/session'];

    return this.erpApiService.postRequest<ApiResourceResponse>(apiRequest, getReportExecuatableMap);
  }

  getReportSession(request: ReportSessionRequest) {
    const apiRequest: ApiRequest = new ApiRequest(); // eslint-disable-line amcs-ts-plugin/api-request-ban
    apiRequest.urlResourcePath = ['bireports/session'];
    return this.enhancedErpApiService.postMessage(apiRequest, request, ReportSessionRequest, ReportSessionResponse)
      .pipe(
        switchMap(response => {
          const reportSession = new ReportSession();
          reportSession.Url = response.url;
          return of(reportSession);
        })
      );
  }
}
