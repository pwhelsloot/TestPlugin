/**
 * @deprecated use TypedJSON and amcsJsonMember suite instead (see @core-module/models/api/api-base.model)
 */
export function postAlias(name: string) {

    return function(target: Object, propertyKey: string | symbol) {
        if (!target['constructor'].hasOwnProperty('_postAlias')) {
            target['constructor']['_postAlias'] = Object.assign({}, target['constructor']['_postAlias']);
        }
        target['constructor']['_postAlias'][propertyKey] = name;
    };
}
