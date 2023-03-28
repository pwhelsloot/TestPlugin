import { ApiBaseModel } from '@core-module/models/api/api-base.model';
import { LocalizedDatePipe } from '@translate/localized-datePipe';
import { EntityHistory } from './entity-history.model';

export abstract class ChangeLog extends ApiBaseModel {

  abstract entityHistories: EntityHistory[];

  abstract getPrimaryKeyName(): string;

  abstract formatPropertyKey(translations: string[], val: string): string;

  abstract formatValue(translations: string[], key: string, val: string, datePipe: LocalizedDatePipe): string;
}
