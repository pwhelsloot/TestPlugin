import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { APIRequestFormatterService } from '@core-module/services/api-request-formatter.service';
import { ApiResourceResponse } from '../../config/api-resource-response.interface';

/**
 * @deprecated use TypedJSON and amcsJsonMember suite instead (see @core-module/models/api/api-base.model)
 */
export class ClassBuilder {
    // For inward we use the use the aliases on the source
    // i.e Create a destination object with properites destinaton[fieldName] = source[aliasName];
    static buildClassInward<T>(destination: any, sourceArgs: any, mappingFields: { name: string; isDate: boolean }[]): T {
        for (const field in mappingFields) {
            if (mappingFields[field]) {
                const aliasField = mappingFields[field];
                if (aliasField.isDate) {
                    if (sourceArgs[aliasField.name]) {
                        destination[field] = APIRequestFormatterService.formatDateFromUTC(sourceArgs[aliasField.name]);
                    } else {
                        destination[field] = null;
                    }
                } else {
                    destination[field] = sourceArgs[aliasField.name];
                }
            }
        }
        return destination;
    }

    // For outward we use the use the aliases on the destination
    // i.e Create a destination object with properites destinaton[aliasName] = source[fieldName];
    static buildClassOutward<T>(destination: any, sourceArgs: any, mappingFields: any, datesToUTCString: boolean): T {
        for (const field in mappingFields) {
            if (mappingFields[field]) {
                const aliasField = mappingFields[field];
                if (sourceArgs[field] !== undefined && sourceArgs[field] !== null && typeof sourceArgs[field].getTime === 'function') {
                    if (datesToUTCString) {
                        destination[aliasField] = APIRequestFormatterService.formatDateToUTC(sourceArgs[field]);
                    } else {
                        destination[aliasField] = AmcsDate.createFromKeepTime(sourceArgs[field]);
                    }
                } else {
                    destination[aliasField] = sourceArgs[field];
                }
            }
        }
        return destination;
    }

    static buildFromArgs<T>(args: any, type: new () => T): any {
        const instance = new type();
        const fields = (type as any)._alias;
        return ClassBuilder.buildClassInward(instance, args, fields);
    }

    static buildFromApiResourceResponse<T>(apiResourceResponse: ApiResourceResponse, type: new () => T): any {
        let result: T;
        if (apiResourceResponse) {
            const instance = new type();
            const fields = (type as any)._alias;
            result = ClassBuilder.buildClassInward(instance, apiResourceResponse, fields);
        }
        return result;
    }

    static buildFromApiResourceResults<T>(apiResults: ApiResourceResponse[], type: new () => T): any[] {
        const transformed = new Array<any>();

        if (apiResults) {
            for (const apiResult of apiResults) {
                const item = ClassBuilder.buildFromApiResourceResponse<T>(apiResult, type);
                transformed.push(item);
            }
        }

        return transformed;
    }

    static buildFromFormModel<S, D>(formModel: any, sourceType: new () => S, destinationType: new () => D): D {
        const instance = new destinationType();
        const fields = (sourceType as any)._formAlias;
        return ClassBuilder.buildClassOutward<D>(instance, formModel, fields, false);
    }

    static buildPost<T>(source: any, type: new () => T): any {
        const instance = {};
        const fields = (type as any)._postAlias;
        return ClassBuilder.buildClassOutward<T>(instance, source, fields, true);
    }

    static buildPostArray<T>(source: any[], type: new () => T): any[] {
        const transformed = new Array<any>();

        if (source) {
            for (const sourceItem of source) {
                const item = ClassBuilder.buildPost<T>(sourceItem, type);
                transformed.push(item);
            }
        }

        return transformed;
    }
}
