import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class MaterialProducerTypeLookup extends ApiBaseModel {

    @amcsJsonMember('MaterialProducerTypeId')
    id: number;

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonMember('GUID')
    guid: string;

}
