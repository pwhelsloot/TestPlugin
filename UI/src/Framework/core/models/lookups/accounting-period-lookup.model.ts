import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI + IMMUI
 */
@amcsJsonObject()
export class AccountingPeriodLookup extends ApiBaseModel implements ILookupItem {

    @alias('AccountingPeriodId')
    @amcsJsonMember('AccountingPeriodId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('StartDate', true)
    @amcsJsonMember('StartDate', true)
    startDate: Date;

    @alias('EndDate', true)
    @amcsJsonMember('EndDate', true)
    endDate: Date;

    @alias('IsClosed')
    @amcsJsonMember('IsClosed')
    isClosed: boolean;
}
