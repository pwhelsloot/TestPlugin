import { Injectable } from '@angular/core';
import { IApiRequestGetOptions } from '@core-module/models/api/api-request-get-options.interface';
import { Observable } from 'rxjs';
import { ApiBaseModel } from '../../models/api/api-base.model';
import { CountCollectionModel } from '../../models/api/count-collection.model';
import { IFilter } from '../../models/api/filters/iFilter';
import { EnhancedErpApiService } from './../enhanced-erp-api.service';
import { ApiBaseDataService } from './api-base-data.service';

/**
 * Call any API without setting up seperate services
 * This service does not retain any state.
 *
 * @export
 * @class ApiBusinessService
 */
@Injectable({ providedIn: 'root' })
export class ApiBusinessService {
  constructor(private readonly enhancedErpApiService: EnhancedErpApiService) {}

  /**
   *
   *
   * @template ResponseType
   * @param {IFilter[]} filters
   * @param {new () => ResponseType} responseType
   * @param options
   * @returns
   * @memberof ApiBusinessService
   */
  get<ResponseType extends ApiBaseModel>(
    filters: IFilter[],
    responseType: new () => ResponseType,
    options?: IApiRequestGetOptions
  ): Observable<ResponseType> {
    const service = this.createDataService(responseType);
    return service.get(filters, options);
  }

  /**
   *
   *
   * @template ReponseType
   * @param {IFilter[]} filters
   * @param {new () => ResponseType} responseType
   * @param options
   * @returns
   * @memberof ApiBusinessService
   */
  async getAsync<ReponseType extends ApiBaseModel>(
    filters: IFilter[],
    responseType: new () => ReponseType,
    options?: IApiRequestGetOptions
  ): Promise<ReponseType> {
    const service = this.createDataService(responseType);
    return service.getAsync(filters, options);
  }

  /**
   *
   * @template ResponseType
   * @param {number} id
   * @param {new () => ResponseType} responseType
   * @returns
   * @memberof ApiBusinessService
   */
  getById<ResponseType extends ApiBaseModel>(id: number, responseType: new () => ResponseType): Observable<ResponseType> {
    const service = this.createDataService(responseType);
    return service.getById(id);
  }

  /**
   *
   * @template ResponseType
   * @param {number} id
   * @param {new () => ResponseType} responseType
   * @returns
   * @memberof ApiBusinessService
   */
  async getByIdAsync<ResponseType extends ApiBaseModel>(id: number, responseType: new () => ResponseType): Promise<ResponseType> {
    const service = this.createDataService(responseType);
    return service.getByIdAsync(id);
  }

  /**
   *
   * @template ResponseType
   * @param {IFilter[]} filters
   * @param {new () => ResponseType} responseType
   * @param options
   * @returns
   * @memberof ApiBusinessService
   */
  getArray<ResponseType extends ApiBaseModel>(
    filters: IFilter[],
    responseType: new () => ResponseType,
    options?: IApiRequestGetOptions
  ): Observable<ResponseType[]> {
    const service = this.createDataService(responseType);
    return service.getArray(filters, options);
  }

  /**
   *
   * @template ResponseType
   * @param {IFilter[]} filters
   * @param {new () => ResponseType} responseType
   * @returns {Promise<ResponseType[]>}
   * @memberof ApiBusinessService
   */
  async getArrayAsync<ResponseType extends ApiBaseModel>(
    filters: IFilter[],
    responseType: new () => ResponseType,
    options?: IApiRequestGetOptions
  ): Promise<ResponseType[]> {
    const service = this.createDataService(responseType);
    return service.getArrayAsync(filters, options);
  }

  /**
   *
   *
   * @template ResponseType
   * @param {IFilter[]} filters
   * @param {new () => ResponseType} responseType
   * @returns {Observable<CountCollectionModel<ResponseType>>}
   * @memberof ApiBusinessService
   */
  getArrayWithCount<ResponseType extends ApiBaseModel>(
    filters: IFilter[],
    responseType: new () => ResponseType,
    options?: IApiRequestGetOptions
  ): Observable<CountCollectionModel<ResponseType>> {
    const service = this.createDataService(responseType);
    return service.getArrayWithCount(filters, options);
  }

  /**
   *
   *
   * @template ResponseType
   * @param {IFilter[]} filters
   * @param {new () => ResponseType} responseType
   * @returns {Promise<CountCollectionModel<ResponseType>>}
   * @memberof ApiBusinessService
   */
  async getArrayWithCountAsync<ResponseType extends ApiBaseModel>(
    filters: IFilter[],
    responseType: new () => ResponseType,
    options?: IApiRequestGetOptions
  ): Promise<CountCollectionModel<ResponseType>> {
    const service = this.createDataService(responseType);
    return service.getArrayWithCountAsync(filters, options);
  }

  /**
   *
   *
   * @template ResponseType
   * @param {ResponseType} entity
   * @param {string} successMessage
   * @param {new () => ResponseType} responseType
   * @returns {Observable<number>}
   * @memberof ApiBusinessService
   */
  save<ResponseType extends ApiBaseModel>(
    entity: ResponseType,
    successMessage: string,
    responseType: new () => ResponseType
  ): Observable<number> {
    const service = this.createDataService(responseType);
    return service.save(entity, successMessage);
  }

  /**
   *
   *
   * @template ResponseType
   * @param {ResponseType} entity
   * @param {string} successMessage
   * @param {new () => ResponseType} responseType
   * @returns {Promise<number>}
   * @memberof ApiBusinessService
   */
  async saveAsync<ResponseType extends ApiBaseModel>(
    entity: ResponseType,
    successMessage: string,
    responseType: new () => ResponseType
  ): Promise<number> {
    const service = this.createDataService(responseType);
    return service.saveAsync(entity, successMessage);
  }

  /**
   *
   *
   * @template ResponseType
   * @param {number} id
   * @param {string} successMessage
   * @param {new () => ResponseType} responseType
   * @returns {Observable<boolean>}
   * @memberof ApiBusinessService
   */
  delete<ResponseType extends ApiBaseModel>(id: number, successMessage: string, responseType: new () => ResponseType): Observable<boolean> {
    const service = this.createDataService(responseType);
    return service.delete(id, successMessage);
  }

  /**
   *
   * @template ResponseType
   * @param {number} id
   * @param {string} successMessage
   * @param {new () => ResponseType} responseType
   * @returns {Promise<boolean>}
   * @memberof ApiBusinessService
   */
  async deleteAsync<ResponseType extends ApiBaseModel>(
    id: number,
    successMessage: string,
    responseType: new () => ResponseType
  ): Promise<boolean> {
    const service = this.createDataService(responseType);
    return service.deleteAsync(id, successMessage);
  }

  /**
   *
   * @template ResponseType
   * @template PostResponseType
   * @param {ResponseType} payload
   * @param {new () => ResponseType} responseType
   * @param {new () => PostResponseType} postResponseType
   * @returns {Observable<PostResponseType>}
   * @memberof ApiBusinessService
   */
  postMessage<ResponseType extends ApiBaseModel, PostResponseType extends ApiBaseModel = any>(
    payload: ResponseType,
    responseType: new () => ResponseType,
    postResponseType: new () => PostResponseType
  ): Observable<PostResponseType> {
    const service = this.createDataService(responseType, postResponseType);
    return service.postMessage(payload);
  }

  /**
   *
   * @template ResponseType
   * @template ResponseType
   * @param {ResponseType} payload
   * @param {new () => ResponseType} responseType
   * @param {new () => PostResponseType} postResponseType
   * @returns {Promise<ResponseType>}
   * @memberof ApiBusinessService
   */
  async postMessageAsync<ResponseType extends ApiBaseModel, PostResponseType extends ApiBaseModel = any>(
    payload: ResponseType,
    responseType: new () => ResponseType,
    postResponseType: new () => PostResponseType
  ): Promise<PostResponseType> {
    const service = this.createDataService(responseType, postResponseType);
    return service.postMessageAsync(payload);
  }

  createDataService<ResponseType extends ApiBaseModel, PostResponseType extends ApiBaseModel = any>(
    responseType: new () => ResponseType,
    postResponseType?: new () => PostResponseType
  ) {
    return new ApiBaseDataService(this.enhancedErpApiService, responseType, postResponseType);
  }
}
