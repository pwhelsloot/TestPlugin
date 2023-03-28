/**
 * @deprecated use TypedJSON and amcsJsonMember suite instead (see @core-module/models/api/api-base.model)
 */
export function formAlias(name: string) {

    return function(target: Object, propertyKey: string | symbol) {
        if (!target['constructor'].hasOwnProperty('_formAlias')) {
            target['constructor']['_formAlias'] = Object.assign({}, target['constructor']['_formAlias']);
        }
        target['constructor']['_formAlias'][propertyKey] = name;
    };
}
