import { alias } from '../config/api-dto-alias.function';
import { amcsJsonArrayMember, amcsJsonMember, amcsJsonObject, amcsJsonZonedDateMember, ApiBaseModel } from './api/api-base.model';
import { HistoryEntryTypeEnum } from './history-entry-type.enum';
import { TypedHistoryChange } from './typed-history-change.model';

@amcsJsonObject()
export class HistoryEntry extends ApiBaseModel {

    @alias('EntityHistoryId')
    @amcsJsonMember('EntityHistoryId')
    entityHistoryId: number;

    @alias('Table')
    @amcsJsonMember('Table')
    table: string;

    @alias('TableId')
    @amcsJsonMember('TableId')
    tableId: number;

    @alias('Date', true)
    @amcsJsonZonedDateMember('Date')
    date: Date;

    @alias('SysUserId')
    @amcsJsonMember('SysUserId')
    sysUserId: number;

    @alias('SysUser')
    @amcsJsonMember('SysUser')
    sysUser: string;

    @alias('Changes')
    @amcsJsonMember('Changes')
    changes: string;

    @alias('EntityHistoryTypeId')
    @amcsJsonMember('EntityHistoryTypeId')
    entityHistoryTypeId: HistoryEntryTypeEnum;

    @alias('TypedChanges')
    @amcsJsonArrayMember('TypedChanges', TypedHistoryChange)
    typedChanges: TypedHistoryChange[];
}
