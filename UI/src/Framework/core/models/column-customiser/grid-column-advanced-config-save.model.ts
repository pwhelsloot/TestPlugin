import { amcsJsonObject, ApiBaseModel, amcsJsonMember } from '../api/api-base.model';
import { GridColumnAdvancedTypeEnum } from './grid-column-advanced-type.enum';

@amcsJsonObject()
export class GridColumnAdvancedConfigSave extends ApiBaseModel {

    @amcsJsonMember('id')
    id: number;

    @amcsJsonMember('advancedType')
    advancedType: GridColumnAdvancedTypeEnum;

    @amcsJsonMember('key')
    key: string;

    @amcsJsonMember('position')
    position: number;
}
