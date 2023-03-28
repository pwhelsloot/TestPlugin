import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonObject, ApiBaseModel, amcsJsonMember } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class PropertyMetadata extends ApiBaseModel {
    @alias('Name')
    @amcsJsonMember('Name')
    name: string;

    @alias('IsEditable')
    @amcsJsonMember('IsEditable')
    isEditable: boolean;

    @alias('IsMandatory')
    @amcsJsonMember('IsMandatory')
    IsMandatory: boolean;

    @alias('IsDisplay')
    @amcsJsonMember('IsDisplay')
    isDisplay: boolean;

    @alias('Position')
    @amcsJsonMember('Position')
    position: number;
}
