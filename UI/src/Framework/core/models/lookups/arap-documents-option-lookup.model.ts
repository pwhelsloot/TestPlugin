import { alias } from '../../config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ARAPDocumentsOptionLookup extends ApiBaseModel {
    @alias('ARAPDocumentsOptionId')
    @amcsJsonMember('ARAPDocumentsOptionId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('GUID')
    @amcsJsonMember('GUID')
    guid: string;
}

export enum ARAPDocumentsOptionEnum {
    SignBased = 1,
    ServiceBased = 2
}
