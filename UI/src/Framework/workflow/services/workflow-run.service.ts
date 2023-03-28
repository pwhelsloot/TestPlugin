import { Injectable } from '@angular/core';
import { ApiBaseService } from '@core-module/services/api-base.service';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { WorkflowResponse } from '@workflow/models/workflow-response.model';
import { WorkflowRunRequest } from '@workflow/models/workflow-run-request.model';

@Injectable()
export class WorkflowRunService extends ApiBaseService<WorkflowRunRequest, WorkflowResponse> {

  constructor(
    enhancedErpApiService: EnhancedErpApiService) {
    super(enhancedErpApiService, WorkflowRunRequest,WorkflowResponse);
  }

}
