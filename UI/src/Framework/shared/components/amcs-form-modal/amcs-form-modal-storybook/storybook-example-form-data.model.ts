import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class SBExampleFormModal extends ApiBaseModel {

    @amcsJsonMember('Id')
    id: number;

    @amcsJsonMember('TestId')
    testId: number;

    @amcsJsonMember('TestIdTwo')
    testIdTwo: number;

    @amcsJsonMember('TestName')
    testName: string;
}
