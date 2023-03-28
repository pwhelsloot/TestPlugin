import { Injectable } from '@angular/core';
import { DinnerPlanEditorData } from '@app/template/models/recipe-stepper/dinner-plan-editor-data.model';
import { ApiBaseService } from '@core-module/services/api-base.service';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';

@Injectable()
export class DinnerPlanService extends ApiBaseService<DinnerPlanEditorData> {
  constructor(enhancedErpApiService: EnhancedErpApiService) {
    super(enhancedErpApiService, DinnerPlanEditorData);
  }
}
