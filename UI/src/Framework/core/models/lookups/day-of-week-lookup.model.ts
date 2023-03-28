import { alias } from '@coreconfig/api-dto-alias.function';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';

/**
 * @deprecated Move to PlatformUI + IMMUI
 */
@amcsJsonObject()
export class DayOfWeekLookup extends ApiBaseModel implements ILookupItem {
    @amcsJsonMember('DayOfWeekId')
    @alias('DayOfWeekId')
    id: number;

    @amcsJsonMember('IsWorkingDay')
    @alias('IsWorkingDay')
    isWorkingDay: boolean;

    @amcsJsonMember('DayOfWeek')
    @alias('DayOfWeek')
    description: string;
}
