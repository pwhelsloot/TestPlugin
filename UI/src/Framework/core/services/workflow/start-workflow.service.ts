import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { SignalRUserContextService } from '@core-module/services/config/messaging/signalr-user-context.service';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { combineLatest } from 'rxjs';
import { take } from 'rxjs/operators';
import { CoreTranslationsService } from '../translation/core-translation.service';
import { StartWorkflow } from './start-workflow.model';

@Injectable({ providedIn: 'root' })
export class StartWorkflowService extends BaseService {

    constructor(
        private readonly businessService: ApiBusinessService,
        private readonly userContextService: SignalRUserContextService,
        private readonly coreTranslationsService: CoreTranslationsService) {
        super();
    }

    start(workflowInstanceId: string, providerName: string, state: string) {
        combineLatest([
            this.userContextService.userContext$,
            this.coreTranslationsService.translations
        ]).pipe(take(1))
        .subscribe(async ([userContext, translations]) => {
            await this.businessService.saveAsync(
                this.createStartWorkflowModel(workflowInstanceId, providerName, userContext.instance, state), translations['workflow.started'], StartWorkflow);
        });
    }

    private createStartWorkflowModel(workflowInstanceId: string, providerName: string, userContext: string, state: string): StartWorkflow {
        const model = new StartWorkflow();
        model.id = workflowInstanceId;
        model.providerName = providerName;
        model.userContext = userContext;
        model.state = state;

        return model;
    }
}
