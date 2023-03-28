import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class LocalAuthorityLookup extends ApiBaseModel {

    @alias('LocalAuthorityId')
    @amcsJsonMember('LocalAuthorityId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;

}
