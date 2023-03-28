import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class WeighingTicketSearchResult extends ApiBaseModel {

  @amcsJsonMember('WeighingId')
  id: number;

  @amcsJsonMember('CompanyOutletId')
  companyOutletId: number;

  @amcsJsonMember('Confirmed')
  confirmed: boolean;

  @amcsJsonMember('WeighingDate', true)
  weighingDate: Date;

  @amcsJsonMember('TicketNo')
  ticketNo: string;

  @amcsJsonMember('CustomerName')
  customerName: string;

  @amcsJsonMember('CompanyOutlet')
  companyOutlet: string;

  @amcsJsonMember('Material')
  material: string;

  @amcsJsonMember('RegistrationNo')
  registrationNo: string;

  @amcsJsonMember('NetWeight')
  netWeight: number;

  @amcsJsonMember('Quantity')
  quantity: number;

  @amcsJsonMember('AuditRequired')
  auditRequired: boolean;

  @amcsJsonMember('Status')
  status: number;

  @amcsJsonMember('WeighingTypeId')
  weighingTypeId: number;

  @amcsJsonMember('CollectedAndReceivedBales')
  collectedAndReceivedBales: string;
}
