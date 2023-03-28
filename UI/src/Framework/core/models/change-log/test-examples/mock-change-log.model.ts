import { ChangeLog } from '@core-module/models/change-log/change-log.model';
import { EntityHistory } from '@core-module/models/change-log/entity-history.model';
import { LocalizedDatePipe } from '@translate/localized-datePipe';

export class MockChangeLog extends ChangeLog {

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

}
