/* eslint-disable amcs-ts-plugin/api-request-ban */
import { ApiBaseModelDecoratorKeys } from '@core-module/models/api/api-base-model-decorator-keys.model';
import { ApiBaseModel } from '@core-module/models/api/api-base.model';
import { IApiRequestGetOptions } from '@core-module/models/api/api-request-get-options.interface';
import { CoreApiRequest } from '@core-module/models/api/core-api-request';
import { Observable } from 'rxjs';
import { ApiRequest } from '../../models/api/api-request.model';
import { CountCollectionModel } from '../../models/api/count-collection.model';
import { IFilter } from '../../models/api/filters/iFilter';
import { EnhancedErpApiService } from '../enhanced-erp-api.service';

/**
 * @template ResponseType Type returned by any Get method
 * @template PostResponseType Type returned by the postMessage method
 */
export class ApiBaseDataService<ResponseType extends ApiBaseModel, PostResponseType extends ApiBaseModel = any> {
  constructor(
    private readonly enhancedErpApiService: EnhancedErpApiService,
    private readonly responseType: new () => ResponseType,
    private readonly postResponseType?: new () => PostResponseType
  ) {
    this.validate(responseType, enhancedErpApiService);
    this.setDecoratorUrl();
  }

  private decoratorUrl: string;

  /**
   * @param {IFilter[]} filters
   * @param {IApiRequestGetOptions} [options]
   * @returns {Observable<ResponseType>}
   * @memberof ApiBaseDataService
   */
  get(filters: IFilter[], options?: IApiRequestGetOptions): Observable<ResponseType> {
    const apiRequest = this.createGetRequest(filters, options);
    return this.enhancedErpApiService.get<ResponseType>(apiRequest, this.responseType);
  }

  /**
   *
   *
   * @param {IFilter[]} filters
   * @param {IApiRequestGetOptions} [options]
   * @returns {Promise<ResponseType>}
   * @memberof ApiBaseDataService
   */
  async getAsync(filters: IFilter[], options?: IApiRequestGetOptions): Promise<ResponseType> {
    return await this.get(filters, options).toPromise();
  }

  /**
   * @param {number} id
   * @returns {Observable<ResponseType>}
   * @memberof ApiBaseDataService
   */
  getById(id: number): Observable<ResponseType> {
    const apiRequest = this.createGetByIdRequest(id);
    return this.enhancedErpApiService.get<ResponseType>(apiRequest, this.responseType);
  }

  /**
   * @param {number} id
   * @returns {Observable<ResponseType>}
   * @memberof ApiBaseDataService
   */
  async getByIdAsync(id: number): Promise<ResponseType> {
    return await this.getById(id).toPromise();
  }

  /**
   * @param {IFilter[]} filters
   * @param {IApiRequestGetOptions} [options]
   * @returns {Observable<ResponseType[]>}
   * @memberof ApiBaseDataService
   */
  getArray(filters: IFilter[], options?: IApiRequestGetOptions): Observable<ResponseType[]> {
    const apiRequest = this.createGetRequest(filters, options);
    return this.enhancedErpApiService.getArray<ResponseType>(apiRequest, this.responseType);
  }

  /**
   * @param {IFilter[]} filters
   * @param {IApiRequestGetOptions} [options]
   * @returns {Promise<ResponseType[]>}
   * @memberof ApiBaseDataService
   */
  async getArrayAsync(filters: IFilter[], options?: IApiRequestGetOptions): Promise<ResponseType[]> {
    return await this.getArray(filters, options).toPromise();
  }

  /**
   * @param {IFilter[]} filters
   * @param {IApiRequestGetOptions} [options]
   * @returns {Observable<CountCollectionModel<ResponseType>>}
   * @memberof ApiBaseDataService
   */
  getArrayWithCount(filters: IFilter[], options?: IApiRequestGetOptions): Observable<CountCollectionModel<ResponseType>> {
    const apiRequest = this.createGetRequest(filters, options);
    if (apiRequest.includeCount === undefined) {
      apiRequest.includeCount = true;
    }
    return this.enhancedErpApiService.getArrayWithCount<ResponseType>(apiRequest, this.responseType);
  }

  /**
   * @param {IFilter[]} filters
   * @param {IApiRequestGetOptions} [options]
   * @returns {Promise<CountCollectionModel<ResponseType>>}
   * @memberof ApiBaseDataService
   */
  async getArrayWithCountAsync(filters: IFilter[], options?: IApiRequestGetOptions): Promise<CountCollectionModel<ResponseType>> {
    return this.getArrayWithCount(filters, options).toPromise();
  }

  /**
   * @param {ResponseType} entity
   * @param {string} successMessage
   * @returns {Observable<number>}
   * @memberof ApiBaseDataService
   */
  save(entity: ResponseType, successMessage: string): Observable<number> {
    const apiRequest: ApiRequest = new ApiRequest();
    apiRequest.urlResourcePath = [this.decoratorUrl];
    return this.enhancedErpApiService.save<ResponseType>(apiRequest, entity, this.responseType, successMessage);
  }

  /**
   *
   *
   * @param {ResponseType} entity
   * @param {string} successMessage
   * @returns {Promise<number>}
   * @memberof ApiBaseDataService
   */
  async saveAsync(entity: ResponseType, successMessage: string): Promise<number> {
    return await this.save(entity, successMessage).toPromise();
  }

  /**
   * @param {number} id
   * @param {string} successMessage
   * @returns {Observable<boolean>}
   * @memberof ApiBaseDataService
   */
  delete(id: number, successMessage: string): Observable<boolean> {
    const apiRequest = new ApiRequest();
    apiRequest.urlResourcePath = [this.decoratorUrl, id];
    return this.enhancedErpApiService.delete(apiRequest, successMessage);
  }

  /**
   *
   *
   * @param {number} id
   * @param {string} successMessage
   * @returns {Promise<boolean>}
   * @memberof ApiBaseDataService
   */
  async deleteAsync(id: number, successMessage: string): Promise<boolean> {
    return await this.delete(id, successMessage).toPromise();
  }

  /**
   * @param {ResponseType} payload
   * @returns {Observable<PostResponseType>}
   * @memberof ApiBaseDataService
   */
  postMessage(payload: ResponseType): Observable<PostResponseType> {
    if (!this.postResponseType) {
      throw Error('Response type must be set to use PostMessage');
    }

    const apiRequest: ApiRequest = new ApiRequest();
    apiRequest.urlResourcePath = [this.decoratorUrl];
    return this.enhancedErpApiService.postMessage(apiRequest, payload, this.responseType, this.postResponseType);
  }

  /**
   *
   *
   * @param {ResponseType} payload
   * @returns {Promise<PostResponseType>}
   * @memberof ApiBaseDataService
   */
  async postMessageAsync(payload: ResponseType): Promise<PostResponseType> {
    return await this.postMessage(payload).toPromise();
  }

  /**
   * Creates a ApiRequest for a Get operation
   *
   * @private
   * @param {IFilter[]} [filters=[]]
   * @param {IApiRequestGetOptions} [options]
   * @returns
   * @memberof ApiBaseDataService
   */
  private createGetRequest(filters: IFilter[] = [], options?: IApiRequestGetOptions) {
    let apiRequest = options && options.isCoreRequest ? new CoreApiRequest(options) : new ApiRequest(options);
    apiRequest.urlResourcePath = [this.decoratorUrl];
    apiRequest.filters = filters;
    return apiRequest;
  }

  /**
   * Creates a ApiRequest for a GetById operation
   *
   * @private
   * @param {number} [id]
   * @returns
   * @memberof ApiBaseDataService
   */
  private createGetByIdRequest(id: number) {
    const apiRequest: ApiRequest = new ApiRequest();
    apiRequest.urlResourcePath = [this.decoratorUrl, id];
    return apiRequest;
  }

  /**
   * Validate parameters
   *
   * @private
   * @param {new () => ResponseType} ResponseType Type returned by any Get method
   * @param {EnhancedErpApiService} enhancedErpApiService
   * @memberof ApiBaseDataService
   */
  private validate(responseType: new () => ResponseType, enhancedErpApiService: EnhancedErpApiService) {
    if (!responseType) {
      throw new Error('responseType cannot be null');
    }

    if (!enhancedErpApiService) {
      throw new Error('enhancedErpApiService cannot be null');
    }
  }

  /**
   *  Gets url from request type metadata and throws error if url isn't provided
   *
   * @private
   * @memberof ApiBaseDataService
   */
  private setDecoratorUrl() {
    this.decoratorUrl = Reflect.getMetadata(ApiBaseModelDecoratorKeys.apiUrl, this.responseType);
    if (!this.decoratorUrl) {
      throw Error(`Cannot create ApiBaseDataService. amcsApiUrl decorator is missing on the request model.`);
    }
  }
}
