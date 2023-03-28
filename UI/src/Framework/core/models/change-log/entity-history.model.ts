import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, amcsJsonZonedDateMember, ApiBaseModel } from '@core-module/models/api/api-base.model';
import { EntityHistoryChange } from './entity-history-change.model';
import { EntityHistoryTypeEnum } from './entity-history-type.enum';

@amcsJsonObject()
export class EntityHistory extends ApiBaseModel {

    @amcsJsonMember('EntityHistoryId')
    entityHistoryId: number;

    @amcsJsonMember('Table')
    table: string;

    @amcsJsonMember('TableId')
    tableId: number;

    @amcsJsonZonedDateMember('Date')
    date: Date;

    @amcsJsonMember('SysUserId')
    sysUserId: number;

    @amcsJsonMember('SysUser')
    sysUser: string;

    @amcsJsonMember('Changes')
    changes: string;

    @amcsJsonMember('ParentTable')
    parentTable: string;

    @amcsJsonMember('ParentTableId')
    parentTableId: number;

    @amcsJsonMember('CorrelationId')
    correlationId: string;

    @amcsJsonMember('EntityHistoryTypeId')
    entityHistoryTypeId: EntityHistoryTypeEnum;

    @amcsJsonArrayMember('TypedChanges', EntityHistoryChange)
    typedChanges: EntityHistoryChange[];

    formattedChangeDescription: string;
}
