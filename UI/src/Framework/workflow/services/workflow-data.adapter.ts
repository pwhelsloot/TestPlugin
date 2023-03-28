import { Injectable } from '@angular/core';
import { ApiBaseModel } from '@core-module/models/api/api-base.model';
import { BaseService } from '@core-module/services/base.service';

@Injectable()
export class WorkflowDataAdapter extends BaseService {

  protected parseData<T extends ApiBaseModel>(viewData: string, type: new () => T): T {
    return new type().parse(viewData, type);
  }

  protected postData<T extends ApiBaseModel>(model: T, type: (new () => T)): string {
    return new type().post<T>(model, type);
  }

}
