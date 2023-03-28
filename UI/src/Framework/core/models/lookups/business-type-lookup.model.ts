import { CustomerTemplateBusinessTypeOption } from '@core-module/models/external-dependencies/customer-template-business-type-option.model';
import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

@amcsJsonObject()
export class BusinessTypeLookup extends ApiBaseModel {

    @alias('BusinessTypeId')
    @amcsJsonMember('BusinessTypeId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    businessTypeOptions: CustomerTemplateBusinessTypeOption[];

    constructor() {
        super();
        this.businessTypeOptions = [];
    }
}
