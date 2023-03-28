import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class SalesOrderSearchResult extends ApiBaseModel {

    @amcsJsonMember('SalesOrderId')
    id: number;

    @amcsJsonMember('SONumber')
    soNumber: string;

    @amcsJsonMember('StartDate', true)
    startDate: Date;

    @amcsJsonMember('SalesOrderStatusId')
    salesOrderStatusId: number;

    salesOrderStatus: string;

    @amcsJsonMember('Customer')
    customer: string;

    @amcsJsonMember('CustomerSite')
    customerSite: string;

    @amcsJsonMember('Materials')
    materials: string;

    @amcsJsonMember('RequiredUnits')
    requiredUnits: number;

    @amcsJsonMember('ExpectedRevenue')
    expectedRevenue: number;
}
