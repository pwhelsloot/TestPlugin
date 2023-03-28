import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from './api/api-base.model';

/**
 * @deprecated Move to IMMUI
 */
@amcsJsonObject()
export class CompanyOutlet extends ApiBaseModel {

    @alias('companyOutletId')
    @amcsJsonMember('CompanyOutletId')
    id: number;

    @alias('companyId')
    @amcsJsonMember('CompanyId')
    companyId: number;

    @alias('description')
    @amcsJsonMember('Description')
    description: string;

    @alias('abbreviatedDescription')
    @amcsJsonMember('AbbreviatedDescription')
    abbreviatedDescription: string;
}
