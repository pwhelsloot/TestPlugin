import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class WasteDeclarationSearchResult extends ApiBaseModel {

    @amcsJsonMember('CustomerId')
    customerId: number;

    @amcsJsonMember('WasteDeclarationTransactionId')
    wasteDeclarationTransactionId: number;

    @amcsJsonMember('ManifestMaterialProfileId')
    manifestMaterialProfileId: number;

    @amcsJsonMember('CustomerName')
    customerName: string;

    @amcsJsonMember('JobTicketNo')
    jobTicketNo: string;

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonMember('DeclarationNumber')
    declarationNumber: string;
}
