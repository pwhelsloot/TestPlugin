import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI + IMMUI
 */
@amcsJsonObject()
export class CompanyLookup extends ApiBaseModel {

    @alias('CompanyId')
    @amcsJsonMember('CompanyId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('LegislationTypeId')
    @amcsJsonMember('LegislationTypeId')
    legislationTypeId: number;
}
