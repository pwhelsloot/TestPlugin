import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class ServiceAgreementServiceLookup extends ApiBaseModel implements ILookupItem {

    @amcsJsonMember('ServiceId')
    @alias('ServiceId')
    id: number;

    @amcsJsonMember('Description')
    @alias('Description')
    description: string;

    @amcsJsonMember('IsCollection')
    @alias('IsCollection')
    isCollection: boolean;

    @amcsJsonMember('IsUnScheduledMaterial')
    @alias('IsUnScheduledMaterial')
    isUnScheduledMaterial: boolean;

    @amcsJsonMember('IsScheduled')
    @alias('IsScheduled')
    isScheduled: boolean;

    @amcsJsonMember('IsMaterialOut')
    @alias('IsMaterialOut')
    isMaterialOut: boolean;

    @amcsJsonMember('DynamicOptimisationEnabled')
    @alias('DynamicOptimisationEnabled')
    dynamicOptimisationEnabled: boolean;

    @amcsJsonMember('AllowBulkMovements')
    @alias('AllowBulkMovements')
    allowBulkMovements: boolean;
}
