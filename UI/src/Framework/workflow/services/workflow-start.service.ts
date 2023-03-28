import { Injectable } from '@angular/core';
import { ApiBaseService } from '@core-module/services/api-base.service';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { WorkflowResponse } from '@workflow/models/workflow-response.model';
import { WorkflowStartRequest } from '@workflow/models/workflow-start-request.model';

@Injectable()
export class WorkflowStartService extends ApiBaseService<WorkflowStartRequest, WorkflowResponse> {

  constructor(
    enhancedErpApiService: EnhancedErpApiService) {
    super(enhancedErpApiService, WorkflowStartRequest,WorkflowResponse);
  }

}
