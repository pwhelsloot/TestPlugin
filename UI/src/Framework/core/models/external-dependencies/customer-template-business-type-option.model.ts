
import { postAlias } from '@coreconfig/alias-to-api.function';
import { alias } from '@coreconfig/api-dto-alias.function';
import { ISelectableItem } from '@coremodels/iselectable-item.model';

export class CustomerTemplateBusinessTypeOption implements ISelectableItem {

    @postAlias('BusinessTypeBusinessTypeOptionId')
    @alias('businessTypeBusinessTypeOptionId')
    id: number;

    @postAlias('CustomerTemplateBusinessTypeOptionId')
    @alias('CustomerTemplateBusinessTypeOptionId')
    customerTemplateBusinessTypeOptionId: number;

    @postAlias('CustomerTemplateId')
    @alias('CustomerTemplateId')
    customerTemplateId: number;

    @postAlias('BusinessTypeId')
    @alias('BusinessTypeId')
    businessTypeId: number;

    @postAlias('BusinessTypeOptionId')
    @alias('BusinessTypeOptionId')
    businessTypeOptionId: number;

    @postAlias('Description')
    @alias('Description')
    description: string;

    isSelected: boolean;
}
