import { isTruthy } from './is-truthy.function';
export abstract class TypeHelper {
    static isNumber(source: any): boolean {
        if (!isTruthy(source)) {
            return false;
        }
        const id = Number(source);
        if (id !== null && id !== undefined && !isNaN(id) && id >= -2147483648 && id <= 2147483647) {
            return true;
        }
        return false;
    }
}
