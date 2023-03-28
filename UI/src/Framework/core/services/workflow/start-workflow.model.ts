import { amcsJsonMember, amcsJsonObject, amcsApiUrl, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
@amcsApiUrl('workflow/instances')
export class StartWorkflow extends ApiBaseModel {

    @amcsJsonMember('WorkflowDefinitionId')
    id: string;

    @amcsJsonMember('ProviderName')
    providerName: string;

    @amcsJsonMember('UserContext')
    userContext: string;

    @amcsJsonMember('State')
    state: string;
}
