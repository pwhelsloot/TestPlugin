import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class BarcodeSearchResult extends ApiBaseModel {

    @amcsJsonMember('WeighingInId')
    id: number;

    @amcsJsonMember('Barcode')
    barcode: string;

    @amcsJsonMember('RegistrationNo')
    vehicleRegistration: string;

    @amcsJsonMember('TicketNo')
    ticketNo: string;

    @amcsJsonMember('MaterialName')
    materialName: string;

    @amcsJsonMember('Weight')
    weight: number;

    @amcsJsonMember('WeighingId')
    weighingId: number;

    @amcsJsonMember('JobPriceBreakdownId')
    jobPriceBreakdownId?: number;

    @amcsJsonMember('Customer')
    customer: string;
}
