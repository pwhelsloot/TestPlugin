/**
 * @deprecated use TypedJSON and amcsJsonMember suite instead (see @core-module/models/api/api-base.model)
 */
export function alias(name: string, isDate = false) {

    return function(target: Object, propertyKey: string | symbol) {
        if (!target['constructor'].hasOwnProperty('_alias')) {
            target['constructor']['_alias'] = Object.assign({}, target['constructor']['_alias']);
        }
        target['constructor']['_alias'][propertyKey] = { name, isDate };
    };
}
