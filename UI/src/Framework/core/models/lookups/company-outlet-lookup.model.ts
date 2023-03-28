import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsApiUrl, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI + ScaleUI + IMMUI
 */
@amcsJsonObject()
@amcsApiUrl('customer/SysUserCompanyOutletLookups')
export class CompanyOutletLookup extends ApiBaseModel implements ILookupItem {

    @alias('CompanyOutletId')
    @amcsJsonMember('CompanyOutletId')
    id: number;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    @alias('CompanyId')
    @amcsJsonMember('CompanyId')
    companyId: number;

    @alias('Company')
    @amcsJsonMember('Company')
    company: string;

    @alias('LegislationTypeId')
    @amcsJsonMember('LegislationTypeId')
    legislationTypeId: number;

    @alias('ScheduleCapacitySettingsId')
    @amcsJsonMember('ScheduleCapacitySettingsId')
    scheduleCapacitySettingsId: number;

    @alias('UnitOfMeasurementId')
    @amcsJsonMember('UnitOfMeasurementId')
    unitOfMeasurementId: number;

    @alias('IsAssigned')
    @amcsJsonMember('IsAssigned')
    isAssigned: boolean;

    @alias('IsSelected')
    @amcsJsonMember('IsSelected')
    isSelected: boolean;

    @alias('DepartmentId')
    @amcsJsonMember('DepartmentId')
    departmentId: number;
}
