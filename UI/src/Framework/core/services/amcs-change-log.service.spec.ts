import { fakeAsync, TestBed, tick } from '@angular/core/testing';
import { IApiRequestGetOptions } from '@core-module/models/api/api-request-get-options.interface';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { ChangeLog } from '@core-module/models/change-log/change-log.model';
import { EntityHistoryTypeEnum } from '@core-module/models/change-log/entity-history-type.enum';
import { EntityHistory } from '@core-module/models/change-log/entity-history.model';
import { MockMultiLevelChangeLog } from '@core-module/models/change-log/test-examples/mock-multi-level-change-log.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { LocalizedDatePipe } from '@translate/localized-datePipe';
import { MockProvider } from 'ng-mocks';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { take } from 'rxjs/operators';
import { MockChangeLog } from '../models/change-log/test-examples/mock-change-log.model';
import { AmcsChangeLogService } from './amcs-change-log.service';
import { ApiBusinessService } from './service-structure/api-business.service';
import { ApiFilter } from './service-structure/api-filter';
import { ApiFilters } from './service-structure/api-filter-builder';

describe('Service: ChangeLogService', () => {
  const translations = [];

  let service: AmcsChangeLogService;
  let businessService: ApiBusinessService;
  let translationsService: SharedTranslationsService;

  let entityHistory1 = new EntityHistory();
  entityHistory1.entityHistoryId = 1;
  entityHistory1.entityHistoryTypeId = EntityHistoryTypeEnum.Insert;
  entityHistory1.table = 'ParentTable';
  entityHistory1.tableId = 1;
  entityHistory1.typedChanges = [];

  let entityHistory2 = new EntityHistory();
  entityHistory2.entityHistoryId = 2;
  entityHistory2.entityHistoryTypeId = EntityHistoryTypeEnum.Update;
  entityHistory2.table = 'ParentTable';
  entityHistory2.tableId = 1;
  entityHistory2.typedChanges = [];

  let entityHistory3 = new EntityHistory();
  entityHistory3.entityHistoryId = 3;
  entityHistory3.entityHistoryTypeId = EntityHistoryTypeEnum.Insert;
  entityHistory3.table = 'ChildTable';
  entityHistory3.tableId = 1;
  entityHistory3.typedChanges = [];

  let entityHistory4 = new EntityHistory();
  entityHistory4.entityHistoryId = 4;
  entityHistory4.entityHistoryTypeId = EntityHistoryTypeEnum.Update;
  entityHistory4.table = 'ChildTable';
  entityHistory4.tableId = 1;
  entityHistory4.typedChanges = [];

  let mockChangeLog1 = new MockChangeLog();
  mockChangeLog1.entityHistories = [
    entityHistory1, entityHistory2, entityHistory3, entityHistory4
  ];

  let makeBelieveChanges: MockChangeLog[] = [
    mockChangeLog1
  ];

  beforeEach(() => {
    const mockApiBusinessServiceStub = () => ({
      getArrayWithCount: (
        filters: IFilter[],
        responseType,
        options: IApiRequestGetOptions
      ): Observable<CountCollectionModel<ChangeLog>> => {
        return of(new CountCollectionModel<ChangeLog>(makeBelieveChanges, 1));
      }
    });

    const translationsServiceStub = () => ({
      getTranslation: (translationKey: string): string => { return translations[translationKey]; }
    });

    TestBed.configureTestingModule({
      providers: [
        AmcsChangeLogService,
        { provide: ApiBusinessService, useFactory: mockApiBusinessServiceStub },
        { provide: SharedTranslationsService, useFactory: translationsServiceStub },
        MockProvider(LocalizedDatePipe)
      ],
    });

    businessService = TestBed.inject(ApiBusinessService);
    service = TestBed.inject(AmcsChangeLogService);
    translationsService = TestBed.inject(SharedTranslationsService);

    spyOn(businessService, 'getArrayWithCount').and.callThrough();

    translations['changeLog.recordCreated'] = 'record-created';
    translations['changeLog.recordUpdated'] = 'record-updated';
    translationsService.translations = new BehaviorSubject<string[]>(translations);
  });

  it('get ChangeLogs', fakeAsync(async () => {
    // arrange
    let changeLog: ChangeLog;
    let entityHistoryResult1: EntityHistory;
    let entityHistoryResult2: EntityHistory;

    // act
    service.getChangeLogDetails(MockChangeLog, 1, [], 1)
      .pipe(take(1))
      .subscribe((data) => {
        changeLog = data.results[0];
        entityHistoryResult1 = changeLog.entityHistories.find(e => e.entityHistoryId === 1);
        entityHistoryResult2 = changeLog.entityHistories.find(e => e.entityHistoryId === 2);
      });

    tick();

    // assert
    expect(businessService.getArrayWithCount).toHaveBeenCalledOnceWith(
      new ApiFilters()
        .add(ApiFilter.equal('primaryKey', 1))
        .build(),
        MockChangeLog,
        { max: 5, includeCount: true, page: 1 }
    );
    expect(entityHistoryResult1.formattedChangeDescription).toEqual('record-created');
    expect(entityHistoryResult2.formattedChangeDescription).toEqual('record-updated');

  }));

  it('get MultiLevelChangeLogs', fakeAsync(async () => {
    // arrange
    let changeLog: ChangeLog;
    let entityHistoryResult1: EntityHistory;
    let entityHistoryResult2: EntityHistory;
    let entityHistoryResult3: EntityHistory;
    let entityHistoryResult4: EntityHistory;

    // act
    await service.getChangeLogDetails(MockMultiLevelChangeLog, 1, [], 1)
      .pipe(take(1))
      .subscribe((data) => {
        changeLog = data.results[0];
        entityHistoryResult1 = changeLog.entityHistories.find(e => e.entityHistoryId === 1);
        entityHistoryResult2 = changeLog.entityHistories.find(e => e.entityHistoryId === 2);
        entityHistoryResult3 = changeLog.entityHistories.find(e => e.entityHistoryId === 3);
        entityHistoryResult4 = changeLog.entityHistories.find(e => e.entityHistoryId === 4);
      });

    tick();

    // assert
    expect(businessService.getArrayWithCount).toHaveBeenCalledOnceWith(
      new ApiFilters()
        .add(ApiFilter.equal('primaryKey', 1))
        .build(),
        MockMultiLevelChangeLog,
        { max: 5, includeCount: true, page: 1 }
    );
    expect(entityHistoryResult1.formattedChangeDescription).toEqual('parent - record-created');
    expect(entityHistoryResult2.formattedChangeDescription).toEqual('parent - record-updated');
    expect(entityHistoryResult3.formattedChangeDescription).toEqual('child - record-created');
    expect(entityHistoryResult4.formattedChangeDescription).toEqual('child - record-updated');

  }));
});


