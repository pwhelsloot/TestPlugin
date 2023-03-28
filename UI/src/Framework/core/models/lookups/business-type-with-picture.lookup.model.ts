import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { BusinessTypeOptionLookup } from './business-type-option.lookup.model';

/**
 * @deprecated Move to PlatformUI + ScaleUI
 */
@amcsJsonObject()
export class BusinessTypeWithPictureLookup extends ApiBaseModel {

    @amcsJsonMember('BusinessTypeId')
    id: number;

    @amcsJsonMember('BusinessTypeEnumId')
    businessTypeEnumId: number;

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonMember('ImageContentBase64')
    imageContentBase64: string;

    businessTypeOptions: BusinessTypeOptionLookup[] = [];
}
