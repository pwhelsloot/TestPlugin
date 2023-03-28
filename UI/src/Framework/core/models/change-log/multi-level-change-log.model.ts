import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { ChangeLog } from './change-log.model';

export abstract class MultiLevelChangeLog extends ChangeLog {

  abstract getParentTable(): string;

  abstract getParentLabel(translations: string[]): string;

  abstract getChildLabel(translations: string[], tableName: string, tableId: number, businessService: ApiBusinessService): Promise<string>;

}
