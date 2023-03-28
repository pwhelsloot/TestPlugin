import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../../core/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class FieldDefinitionDataModel extends ApiBaseModel {

    @amcsJsonMember('key')
    key: string;

    @amcsJsonMember('isMandatory')
    isMandatory: boolean;

    @amcsJsonMember('isEditable')
    isEditable: boolean;

    @amcsJsonMember('isVisible')
    isVisible: boolean;
}
