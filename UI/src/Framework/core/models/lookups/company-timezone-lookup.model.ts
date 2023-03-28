import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class CompanyTimeZoneLookup extends ApiBaseModel implements ILookupItem {
    id: number;

    @amcsJsonMember('Description')
    description: string;

    @amcsJsonMember('IANATimeZoneId')
    ianaTimeZoneId: string;
}
