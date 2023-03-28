import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { ISelectableItem } from '@core-module/models/iselectable-item.model';

/**
 * @deprecated Move to PlatformUI + ScaleUI
 */
@amcsJsonObject()
export class BusinessTypeOptionLookup extends ApiBaseModel implements ISelectableItem {

    @amcsJsonMember('BusinessTypeOptionId')
    id: number;

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonMember('BusinessTypeId')
    BusinessTypeId: number;

    isSelected: boolean;
}
