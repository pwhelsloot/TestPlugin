
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ApiBaseModelDecoratorKeys } from '@core-module/models/api/api-base-model-decorator-keys.model';
import { APIRequestFormatterService } from '@core-module/services/api-request-formatter.service';
import * as moment from 'moment-timezone';
import 'reflect-metadata';
import { IJsonMemberOptions, jsonArrayMember, jsonMapMember, jsonMember, jsonObject, TypedJSON } from 'typedjson';
import { IApiBaseModel } from './api-base.interface';

// Any model parsing/posting from/to the Api should extend from this
export abstract class ApiBaseModel implements IApiBaseModel {
    parse<T extends ApiBaseModel>(json: any, type: (new () => T)): T {
        return TypedJSON.parse<T>(json, type, { preserveNull: true });
    }

    parseArray<T extends ApiBaseModel>(json: any, type: (new () => T)): T[] {
        return TypedJSON.parseAsArray<T>(json, type, { preserveNull: true });
    }

    post<T extends ApiBaseModel>(model: T, type: (new () => T)): string {
        return TypedJSON.stringify<T>(model, type, { preserveNull: true });
    }

    postArray<T extends ApiBaseModel>(models: T[], type: (new () => T)): string {
        return TypedJSON.stringifyAsArray<T>(models, type, 1, { preserveNull: true });
    }

    filterByValues(filterTerm: string, fieldValuesToSearch: (string | number | undefined)[]): boolean {
        const filterTermSimplified = filterTerm.toLowerCase();

        const filterResults = fieldValuesToSearch.some(fieldValue => {
            const valueDataType = typeof fieldValue;

            if (valueDataType === 'string') {
                return (fieldValue as string).toLowerCase().includes(filterTermSimplified);
            } else if (valueDataType === 'number') {
                return (fieldValue as number).toString().includes(filterTermSimplified);
            } else if (valueDataType === 'undefined') {
                return false;
            } else {
                throw new Error('Unexpected data type when filtering');
            }
        });

        return filterResults;
    }
}

// ApiBaseModel Decorators
export function amcsApiUrl<T extends ApiBaseModel>(url: string, ignoreMock = false) {
  return function(constructor: (new () => T)) {
      Reflect.defineMetadata(ApiBaseModelDecoratorKeys.apiUrl, url, constructor);
      if (ignoreMock) {
        Reflect.defineMetadata(ApiBaseModelDecoratorKeys.apiIgnoreMock, {}, constructor);
      }
  };
}

// TypedJSON Decorator Wrappers
// Class Decorator: The class extending from ApiBaseModel must use this.
export function amcsJsonObject<T extends ApiBaseModel>() {
    return function(type: (new () => T)): void {
        const jsonObjectFunction = jsonObject();
        jsonObjectFunction(type);
    };
}
// Property Decorator: Any property to be parsed/posted must use this.
export function amcsJsonMember(name: string, isDate = false, options?: IJsonMemberOptions): PropertyDecorator {
    return function(target: Function, propertyKey: string | symbol) {
        if (!isTruthy(options)) {
            options = {};
        }
        if (isDate) {
            options = {
                ...options,
                deserializer: (json: any) => {
                    return APIRequestFormatterService.formatDateFromUTC(json);
                },
                serializer: (value: any) => {
                    return APIRequestFormatterService.formatDateToUTC(value);
                }
            };
        }

        options = {
            ...options,
            name
        };
        const jsonMemberFunction = jsonMember(options);
        jsonMemberFunction(target, propertyKey);
    };
}
// Property Decorator: Any array property to be parsed/posted must use this.
export function amcsJsonArrayMember<T extends Number | String | Boolean | ApiBaseModel | Object>(name: string, type: (new () => T), options?: IJsonMemberOptions): PropertyDecorator {
    return function(target: Function, propertyKey: string | symbol) {
        if (!isTruthy(options)) {
            options = {};
        }
        options = { ...options, name };
        const jsonArrayMemberFunction = jsonArrayMember(type, options);
        jsonArrayMemberFunction(target, propertyKey);
    };
}

export function amcsJsonMapMember<T extends Number | String | ApiBaseModel | Object>(name: string, keyType: (new () => T), valueType: (new () => T), options?: IJsonMemberOptions): PropertyDecorator {
    return function(target: Function, propertyKey: string | symbol) {
        if (!isTruthy(options)) {
            options = {};
        }
        options = { ...options, name };
        const jsonMapMemberFunction = jsonMapMember(keyType, valueType, options);
        jsonMapMemberFunction(target, propertyKey);
    };
}

export function amcsJsonLocalDateMember(name: string, options?: IJsonMemberOptions): PropertyDecorator {
    return function(target: Function, propertyKey: string | symbol) {
        if (!isTruthy(options)) {
            options = {};
        }
        options = {
            ...options,
            name,
            deserializer: (value: string) => {
                return APIRequestFormatterService.parseLocalDate(value);
            },
            serializer: (value: Date) => {
                return APIRequestFormatterService.serialiseLocalDate(value);
            }
        };
        const jsonMemberFunction = jsonMember(options);
        jsonMemberFunction(target, propertyKey);
    };
}

// Property Decorator: Any zoned date member to be parsed / posted must use this.
// This should be used with a property of type moment.Moment.
export function amcsJsonZonedDateMember(name: string, options?: IJsonMemberOptions): PropertyDecorator {
    return function(target: Function, propertyKey: string | symbol) {
        if (!isTruthy(options)) {
            options = {};
        }
        options = {
            ...options,
            name,
            deserializer: (value: { DateTime: string; TimeZone: string }) => {
                return APIRequestFormatterService.parseMomentFromZonedDateTimeText(value);
            },
            serializer: (value: moment.Moment) => {
                return APIRequestFormatterService.serialiseMomentToZonedDateTimeText(value);
            }
        };
        const jsonMemberFunction = jsonMember(options);
        jsonMemberFunction(target, propertyKey);
    };
}
