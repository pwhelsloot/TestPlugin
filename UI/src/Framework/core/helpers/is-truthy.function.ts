// Note: When checking for truthy values, we should be checking for NaN, false, and 0 in addition to null and undefined.
// The proactive change to implement that caused breaking changes to be introduced, so reverting to just checking null and undefined.
// This should not be a permanent solution, however - JTW 2019-5-28
export function isTruthy(data: any): boolean {
    if (data !== null && data !== undefined) {
        return typeof data === 'string' && data.trim().length < 1 ? false : true;
    }

    return false;
}

export function isNullOrUndefined(data: any): boolean {
    return data === null || data === undefined ||
        (typeof data === 'string' && data.trim().length < 1);
}

export function ifNull(data: any, option: any): any {
    if (data !== null && data !== undefined) {
        return typeof data === 'string' && data.trim().length < 1 ? option : data;
    }

    return option;
}

export function ifStringNotNullPush(stringArray: string[], input: any): any {
    if (input != null && input.toString().length > 0) {
        stringArray.push(input.toString());
    }
    return stringArray;
}
