import { EntityHistory } from '@core-module/models/change-log/entity-history.model';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { LocalizedDatePipe } from '@translate/localized-datePipe';
import { MultiLevelChangeLog } from '../multi-level-change-log.model';

export class MockMultiLevelChangeLog extends MultiLevelChangeLog {

  entityHistories: EntityHistory[];
  getPrimaryKeyName(): string {
    return 'primaryKey';
  }

  formatPropertyKey(translations: string[], val: string): string {
    return 'propertyKey';
  }

  formatValue(translations: string[], key: string, val: string, datePipe: LocalizedDatePipe): string {
    return 'value';
  }

  getParentTable(): string {
    return 'ParentTable';
  }

  getParentLabel(translations: string[]): string {
    return 'parent';
  }

  async getChildLabel(translations: string[], tableName: string, tableId: number, businessService: ApiBusinessService): Promise<string> {
    return 'child';
  }

}
