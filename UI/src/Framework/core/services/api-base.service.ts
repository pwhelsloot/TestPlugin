import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ApiBaseModel } from '@core-module/models/api/api-base.model';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { EnhancedErpApiService } from '@core-module/services/enhanced-erp-api.service';
import { identity, Observable, ReplaySubject, Subject } from 'rxjs';
import { debounceTime, switchMap, takeUntil } from 'rxjs/operators';
import { CountCollectionModel } from '../models/api/count-collection.model';
import { ApiBaseServiceOptions } from './api-base-service-options';
import { BaseService } from './base.service';
import { ApiBaseDataService } from './service-structure/api-base-data.service';

/**
 * @template ResponseType Type returned by any get method
 * @template PostResponseType Type returned by the postMessage method
 */
export class ApiBaseService<ResponseType extends ApiBaseModel, PostResponseType extends ApiBaseModel = any> extends BaseService {
  readonly getResult$: Observable<ResponseType>;
  readonly getArrayResult$: Observable<ResponseType[]>;
  readonly getArrayWithCount$: Observable<CountCollectionModel<ResponseType>>;
  readonly postMessageResult$: Observable<PostResponseType>;

  constructor(
    enhancedErpApiService: EnhancedErpApiService,
    readonly responseType: new () => ResponseType,
    readonly postResponseType?: new () => PostResponseType,
    readonly options?: ApiBaseServiceOptions
  ) {
    super();
    this.dataService = new ApiBaseDataService<ResponseType, PostResponseType>(enhancedErpApiService, responseType, postResponseType);
    this.postMessageResult$ = this.postMessageResult.asObservable();
    this.getResult$ = this.getResult.asObservable();
    this.getArrayResult$ = this.getArrayResult.asObservable();
    this.getArrayWithCount$ = this.getArrayWithCountResult.asObservable();
    this.setupStreams();
  }

  private readonly dataService: ApiBaseDataService<ResponseType, PostResponseType>;
  private readonly postMessageResult = new ReplaySubject<PostResponseType>(1);
  private readonly getResult = new ReplaySubject<ResponseType>(1);
  private readonly getArrayResult = new ReplaySubject<ResponseType[]>(1);
  private readonly getArrayWithCountResult = new ReplaySubject<CountCollectionModel<ResponseType>>(1);
  private readonly getRequest = new Subject<IFilter[]>();
  private readonly getArrayRequest = new Subject<IFilter[]>();
  private readonly postMessageRequest = new Subject<ResponseType>();
  private readonly getArrayWithCountRequest = new Subject<IFilter[]>();

  /**
   * Fire get request, result will be in getResult$
   *
   * @param {*} data
   * @memberof ApiBaseDataService
   */
  get(filters: IFilter[]) {
    this.getRequest.next(filters);
  }

  /**
   * Get async
   * Does not trigger getResult$
   * Usage: await service.getAsync(...);
   *
   * @param {ApiGetRequest} request
   * @returns {Promise<ResponseType>} Awaitable promise
   * @memberof ApiBaseDataService
   */
  async getAsync(filters: IFilter[]): Promise<ResponseType> {
    return await this.dataService.get(filters).toPromise();
  }

  /**
   * Fire get array request, result will be in getArrayResult$
   *
   * @param {ApiGetArrayRequest} request
   * @memberof ApiBaseDataService
   */
  getArray(filters: IFilter[]) {
    this.getArrayRequest.next(filters);
  }

  /**
   * Get Array async
   * Does not trigger getArrayResult$
   * Usage: await service.getArrayAsync(...);
   *
   * @param {ApiGetArrayRequest} request
   * @returns {Promise<ResponseType[]>} Awaitable promise
   * @memberof ApiBaseDataService
   */
  async getArrayAsync(filters: IFilter[]): Promise<ResponseType[]> {
    return this.dataService.getArrayAsync(filters);
  }

  /**
   * Fire get array with count request, result will be in getArrayWithCount$
   *
   * @param {ApiGetArrayWithCountRequest} request
   * @memberof ApiBaseDataService
   */
  getArrayWithCount(filters: IFilter[]) {
    this.getArrayWithCountRequest.next(filters);
  }

  /**
   * Get array with Count async
   * Does not trigger getArrayWithCount$
   * Usage: await service.getArrayWithCountAsync(...);
   *
   * @param {ApiGetArrayWithCountRequest} request
   * @returns {Promise<CountCollectionModel<ResponseType>>} Awaitable promise
   * @memberof ApiBaseDataService
   */
  async getArrayWithCountAsync(filters: IFilter[]): Promise<CountCollectionModel<ResponseType>> {
    return this.dataService.getArrayWithCountAsync(filters);
  }

  /**
   * Fire post message request, result will be in postMessageResult$
   *
   * @param {ApiPostMessageRequest<ResponseType>} request
   * @memberof ApiBaseDataService
   */
  postMessage(dataModel: ResponseType) {
    this.postMessageRequest.next(dataModel);
  }

  /**
   * Post message request async
   * Does not trigger postMessageResult$
   * Usage: await service.postMessageAsync(...);
   *
   * @param {ApiPostMessageRequest<ResponseType>} request
   * @returns {Promise<PostResponseType>} Awaitable promise
   * @memberof ApiBaseDataService
   */
  async postMessageAsync(dataModel: ResponseType): Promise<PostResponseType> {
    return this.dataService.postMessageAsync(dataModel);
  }

  /**
   * Save an Entity
   *
   * @param {ResponseType} dataModel
   * @param {string} successMessage
   * @returns Id of saved Entity
   * @memberof ApiBaseDataService
   */
  save(dataModel: ResponseType, successMessage?: string): Observable<number> {
    return this.dataService.save(dataModel, successMessage);
  }

  /**
   * Save an Entity
   * Usage: await service.saveAsync(...);
   *
   * @param {ResponseType} dataModel
   * @param {string} successMessage
   * @returns Awaitable promise with Id of saved Entity
   * @memberof ApiBaseDataService
   */
  async saveAsync(dataModel: ResponseType, successMessage?: string): Promise<number> {
    return this.dataService.saveAsync(dataModel, successMessage);
  }

  /**
   * Delete an Entity
   *
   * @param {number} id
   * @param {string} successMessage
   * @returns {Observable<boolean>} Delete succeeded true/false
   * @memberof ApiBaseDataService
   */
  delete(id: number, successMessage?: string): Observable<boolean> {
    return this.dataService.delete(id, successMessage);
  }

  /**
   * Delete an entity with given Id
   * Usage: await service.deleteAsync(...);
   *
   * @param {number} id
   * @param {string} successMessage
   * @returns {Promise<boolean>} Delete succeeded true/false
   * @memberof ApiBaseDataService
   */
  async deleteAsync(id: number, successMessage?: string): Promise<boolean> {
    return this.dataService.deleteAsync(id, successMessage);
  }

  /**
   * TODO: Find way to simplify this..
   * Setup all observable streams
   *
   * @private
   * @memberof ApiBaseDataService
   */
  private setupStreams() {
    this.getRequest
      .pipe(
        takeUntil(this.unsubscribe),
        this.getDebounceTimeFunction(),
        switchMap((request) => this.dataService.get(request))
      )
      .subscribe((data: ResponseType) => {
        this.getResult.next(data);
      });

    this.getArrayRequest
      .pipe(
        takeUntil(this.unsubscribe),
        this.getDebounceTimeFunction(),
        switchMap((request) => this.dataService.getArray(request))
      )
      .subscribe((data: ResponseType[]) => {
        this.getArrayResult.next(data);
      });

    this.getArrayWithCountRequest
      .pipe(
        takeUntil(this.unsubscribe),
        this.getDebounceTimeFunction(),
        switchMap((request) => this.dataService.getArrayWithCount(request))
      )
      .subscribe((data: CountCollectionModel<ResponseType>) => {
        this.getArrayWithCountResult.next(data);
      });

    this.postMessageRequest
      .pipe(
        takeUntil(this.unsubscribe),
        this.getDebounceTimeFunction(),
        switchMap((request) => {
          return this.dataService.postMessage(request);
        })
      )
      .subscribe((result) => {
        this.postMessageResult.next(result);
      });
  }

  private getDebounceTimeFunction<T>() {
    return isTruthy(this.options) && isTruthy(this.options.debounceInterval) && this.options.debounceInterval > 0
      ? debounceTime<T>(this.options.debounceInterval)
      : identity;
  }
}
