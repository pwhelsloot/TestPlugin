import { isTruthy } from './is-truthy.function';

export class BitmaskHelper {

    static convertBitmaskToArray(bitmask: number): number[] {
        if (!isTruthy(bitmask) || bitmask === 0) {
            return [];
        }

        const possibleValues: number[] = [1, 2, 4, 8, 16, 32, 64, 128, 256, 512, 1024];
        return possibleValues
            // tslint:disable:no-bitwise
            // eslint-disable-next-line
            .filter(possibleValue => (bitmask & possibleValue) === possibleValue)
            .map(possibleValue => possibleValue);
    }

    static convertArrayToBitmask(bitmaskArray: number[]): number {
        return isTruthy(bitmaskArray) ? bitmaskArray.reduce((sum, current) => sum + current) : 0;
    }
}
