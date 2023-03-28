import { Injectable } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { ChangeLog } from '@core-module/models/change-log/change-log.model';
import { EntityHistoryTypeEnum } from '@core-module/models/change-log/entity-history-type.enum';
import { EntityHistory } from '@core-module/models/change-log/entity-history.model';
import { MultiLevelChangeLog } from '@core-module/models/change-log/multi-level-change-log.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { LocalizedDatePipe } from '@translate/localized-datePipe';
import { map } from 'rxjs/operators';
import { BaseService } from './base.service';
import { ApiBusinessService } from './service-structure/api-business.service';
import { ApiFilter } from './service-structure/api-filter';
import { ApiFilters } from './service-structure/api-filter-builder';

@Injectable()
export class AmcsChangeLogService extends BaseService {

  constructor(private businessServiceManager: ApiBusinessService,
    private translationService: SharedTranslationsService,
    private datePipe: LocalizedDatePipe) {
    super();
  }

  getChangeLogDetails<T extends ChangeLog>(changeLogType: new () => T, primaryKeyId: number, translations: string[], page: number) {
    const filter = new ApiFilters();
    const changeLogInstance = new changeLogType();
    const primaryKeyName = changeLogInstance.getPrimaryKeyName();
    filter.add(ApiFilter.equal(primaryKeyName, primaryKeyId));
    return this.businessServiceManager.getArrayWithCount(
      filter.build(),
      changeLogType,
      { max: 5, includeCount: true, page }
    ).pipe(
      map(data => {
        let changeLog: CountCollectionModel<ChangeLog> = data;
        if (!isTruthy(changeLog.results[0]) || !(changeLog.results[0] instanceof ChangeLog)) {
          throw new Error('ChangeLog results missing or incorrect format.');
        }

        changeLog.results[0].entityHistories.forEach(async (entityHistory: EntityHistory) => {
          this.addFormattedChangeDescription(entityHistory);

          if (changeLogInstance instanceof MultiLevelChangeLog) {
            this.addTypeLabels(entityHistory, changeLogInstance, translations);
          }

          if (entityHistory.entityHistoryTypeId !== EntityHistoryTypeEnum.Insert) {
            this.formatChanges(entityHistory, changeLogInstance, translations);
          }
        });

        return changeLog;
      }));
  }

  private addFormattedChangeDescription(entityHistory: EntityHistory) {
    if (entityHistory.entityHistoryTypeId === EntityHistoryTypeEnum.Insert) {
      let changeDate = this.datePipe.transform(entityHistory.date, 'short');
      entityHistory.formattedChangeDescription = this.translationService.getTranslation('changeLog.recordCreated').replace('{{date}}', changeDate);
    } else if (entityHistory.entityHistoryTypeId === EntityHistoryTypeEnum.Update) {
      let changeDate = this.datePipe.transform(entityHistory.date, 'short');
      entityHistory.formattedChangeDescription = this.translationService.getTranslation('changeLog.recordUpdated').replace('{{date}}', changeDate);
    }
  }

  private async addTypeLabels(entityHistory: EntityHistory, changeLogInstance: MultiLevelChangeLog, translations: string[]) {
    const typeLabel = entityHistory.table === changeLogInstance.getParentTable()
      ? changeLogInstance.getParentLabel(translations)
      : await changeLogInstance.getChildLabel(translations, entityHistory.table, entityHistory.tableId, this.businessServiceManager);

    entityHistory.formattedChangeDescription = typeLabel + ' - ' + entityHistory.formattedChangeDescription;
  }

  private formatChanges(entityHistory: EntityHistory, changeLogInstance: ChangeLog | MultiLevelChangeLog, translations: string[]) {
    entityHistory.typedChanges.forEach(typedChange => {
      typedChange.formattedPropertyKey = changeLogInstance.formatPropertyKey(translations, typedChange.propertyKey);
      typedChange.oldValue = changeLogInstance.formatValue(translations, typedChange.propertyKey, typedChange.oldValue, this.datePipe);
      typedChange.newValue = changeLogInstance.formatValue(translations, typedChange.propertyKey, typedChange.newValue, this.datePipe);
    });
  }
}
