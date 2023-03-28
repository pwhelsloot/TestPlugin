import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class PickupIntervalLookup extends ApiBaseModel implements ILookupItem {
    @alias('PickupIntervalId')
    @amcsJsonMember('PickupIntervalId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('IsWeekly')
    @amcsJsonMember('IsWeekly')
    isWeekly: boolean;

    @alias('IsMonthly')
    @amcsJsonMember('IsMonthly')
    isMonthly: boolean;
}
