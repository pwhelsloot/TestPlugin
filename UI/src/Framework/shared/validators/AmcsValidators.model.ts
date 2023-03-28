import { FormControl, FormGroup, ValidatorFn } from '@angular/forms';
import { BitmaskHelper } from '@core-module/helpers/bitmask-helper';
import { DateValidationHelper } from '@core-module/helpers/date-validation-helper';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { TypescriptDayOfWeekEnum } from '@coremodels/ts-day-of-week.enum';
import { Observable, timer } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { PricingBasisEnum } from '../../core/models/pricing-basis.enum';

export class AmcsValidators {
    /** Checks whether date selected is in supplied restricted dates */
    static datesInvalid(restrictedDates: Date[]): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            let isInvalid = false;
            if (control.value != null) {
                restrictedDates.forEach((dateElement) => {
                    control.value.setHours(0, 0, 0, 0);
                    dateElement.setHours(0, 0, 0, 0);
                    if (control.value.getTime() === dateElement.getTime()) {
                        isInvalid = true;
                        return;
                    }
                });
                if (isInvalid) {
                    return { datesinvalid: true };
                }
            }
            return null;
        };
    }

    static dateMin(earliestValidDate: Date): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            if (DateValidationHelper.isValidDate(control.value) && control.value < earliestValidDate) {
                return { dateInvalid: true };
            }
            return null;
        };
    }

    static dateWithinDateRanges(dates: [Date, Date][]) {
        return (control: FormControl): { [s: string]: boolean } => {
            const date = new Date(control.value);
            const check = dates.map((dateRange) => {
                const startDate = dateRange[0];
                const endDate = dateRange[1];

                return date >= startDate && date <= endDate;
            });

            return check.some((x) => x) ? null : { dateNotWithinDateRange: true };
        };
    }

    static forceDayOfWeek(day: TypescriptDayOfWeekEnum): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            if (isTruthy(control.value) && control.value.getDay() !== day) {
                return { dayOfWeekInvalid: true };
            }
            return null;
        };
    }

    static isInteger(): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            if (control.value && control.value !== parseInt(control.value, 10)) {
                return { numberInvalid: true };
            }
            return null;
        };
    }

    /** Checks whether value is not in the list */
    static valueNotInList(items: any[], isMatch: (item: any, value: string) => any): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            let valueNotInList = true;
            if (control.value != null) {
                items.forEach((item) => {
                    if (isMatch(item, control.value)) {
                        valueNotInList = false;
                        return;
                    }
                });
                if (valueNotInList) {
                    return { valueNotInList: true };
                }
            }
            return null;
        };
    }

    /** Checks whether date selected is a weekend. */
    static weekendInvalid(control: FormControl): { [s: string]: boolean } {
        if (control.value != null && (control.value.getDay() === 0 || control.value.getDay() === 6)) {
            return { weekendinvalid: true };
        } else {
            return null;
        }
    }

    static endDateBeforeStartDateOnlyIsTruthy(startDate: FormControl, endDate: FormControl) {
        return (group: FormGroup): { [key: string]: any } => {
            if (isTruthy(startDate.value) && isTruthy(endDate.value) && endDate.value < startDate.value) {
                return { endDateInvalid: true };
            } else {
                return null;
            }
        };
    }

    static endDateBeforeStartDateTyped(startDate: FormControl, endDate: FormControl) {
        return (group: FormGroup): { [key: string]: any } => {
            if ((!isTruthy(startDate.value) && isTruthy(endDate.value)) || (isTruthy(endDate.value) && endDate.value < startDate.value)) {
                return { endDateInvalid: true };
            } else {
                return null;
            }
        };
    }

    static endDateOnOrBeforeStartDateTyped(startDate: FormControl, endDate: FormControl) {
        return (group: FormGroup): { [key: string]: any } => {
            if ((!isTruthy(startDate.value) && isTruthy(endDate.value)) || (isTruthy(endDate.value) && endDate.value <= startDate.value)) {
                return { endDateInvalid: true };
            } else {
                return null;
            }
        };
    }

    static endDateBeforeStartDate(startDateKey: string, endDateKey: string) {
        return (group: FormGroup): { [key: string]: any } => {
            const startDate = group.controls[startDateKey];
            const endDate = group.controls[endDateKey];

            if ((!isTruthy(startDate.value) && isTruthy(endDate.value)) || (isTruthy(endDate.value) && endDate.value < startDate.value)) {
                return { endDateInvalid: true };
            } else {
                return null;
            }
        };
    }

    static endDateBeforeStartDateOrOldStartDate(oldStartDate: FormControl, startDate: FormControl, endDate: FormControl) {
        return (group: FormGroup): { [key: string]: any } => {
            const actualStartDate = isTruthy(startDate.value) ? startDate.value : oldStartDate.value;
            if ((!isTruthy(actualStartDate) && isTruthy(endDate.value)) || (isTruthy(endDate.value) && endDate.value < actualStartDate)) {
                return { endDateInvalid: true };
            } else {
                return null;
            }
        };
    }

    static checkEndDateIfStartDateAvailable(startDate: FormControl, endDate: FormControl) {
        return (group: FormGroup): { [key: string]: any } => {
            if (isTruthy(startDate.value) && isTruthy(endDate.value) && endDate.value < startDate.value) {
                return { endDateInvalid: true };
            } else {
                return null;
            }
        };
    }

    static endTimeBeforeStartTime(startTimeKey: string, endTimeKey: string) {
        return (group: FormGroup): { [key: string]: any } => {
            const startTime = group.controls[startTimeKey];
            const endTime = group.controls[endTimeKey];

            if (
                isTruthy(startTime) &&
                isTruthy(endTime) &&
                isTruthy(startTime.value) &&
                isTruthy(endTime.value) &&
                endTime.value < startTime.value
            ) {
                return { endTimeBeforeStartTime: true };
            }

            return null;
        };
    }

    static ceilingLowerThanFloor(floor: FormControl, ceiling: FormControl) {
        return (group: FormGroup): { [key: string]: any } => {
            if (isTruthy(floor.value) && isTruthy(ceiling.value) && ceiling.value < floor.value) {
                return { ceilingInvalid: true };
            } else {
                return null;
            }
        };
    }

    static maxLowerThanMin(min: FormControl, max: FormControl) {
        return (group: FormGroup): { [key: string]: any } => {
            if (isTruthy(min.value) && isTruthy(max.value) && max.value < min.value) {
                return { maxInvalid: true };
            } else {
                return null;
            }
        };
    }

    static validCoordinate(type?: 'latitude' | 'longitude'): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            if (!control.value) {
                return null;
            }

            if (!/-?[0-9]{1,3}[.][0-9]+$/g.test(control.value)) {
                return { invalidcoordinate: true };
            }

            if (type === 'latitude' && Math.abs(parseInt(control.value, 10)) >= 91) {
                return { latitudeoutofrange: true };
            }

            if (type === 'longitude' && Math.abs(parseInt(control.value, 10)) >= 181) {
                return { longitudeoutofrange: true };
            }

            return null;
        };
    }

    static validEmail(): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            if (isTruthy(control.value) && !/[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}/g.test(control.value)) {
                return { email: true };
            }

            return null;
        };
    }

    /** Checks whether time is outwidth supplied invalid start/end times */
    static timeInvalid(startTime: Date, endTime: Date): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            if (control.value != null) {
                if (
                    control.value.getHours() < startTime.getHours() ||
                    (control.value.getHours() === startTime.getHours() && control.value.getMinutes() < startTime.getMinutes())
                ) {
                    return null;
                }
                if (
                    control.value.getHours() > endTime.getHours() ||
                    (control.value.getHours() === endTime.getHours() && control.value.getMinutes() > endTime.getMinutes())
                ) {
                    return null;
                }
                return { timeinvalid: true };
            }
            return null;
        };
    }

    /** Async validator to check whether a username is available. Note this uses switchmap and a timer
     * to stop spamming request to server (fires 300ms after last keystroke) */
    static usernameAvailable(control: FormControl): Promise<any> | Observable<any> {
        return timer(300).pipe(
            switchMap(() => {
                return new Promise<any>((resolve, reject) => {
                    setTimeout(() => {
                        if (control.value === 'Robbie') {
                            resolve({ usernameunavailable: true });
                        } else {
                            resolve(null);
                        }
                    }, 100);
                });
            })
        );
    }

    /** Async validator to check whether a email is available. Note this uses switchmap and a timer
     * to stop spamming request to server (fires 300ms after last keystroke) */
    static emailAvailable(control: FormControl): Promise<any> | Observable<any> {
        return timer(300).pipe(
            switchMap(() => {
                return new Promise<any>((resolve, reject) => {
                    setTimeout(() => {
                        if (control.value === 'robmcil@gmail.com') {
                            resolve({ emailunavailable: true });
                        } else {
                            resolve(null);
                        }
                    }, 100);
                });
            })
        );
    }

    /** Validator to check whether document delivery types are valid. Not Required cannot be selected with any other type. */
    static documentDeliveryTypesValid(): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            if (control.value != null) {
                const controlArray = BitmaskHelper.convertBitmaskToArray(control.value);
                const notRequiredSelected = controlArray.filter((x) => x === 8 /*Not Required*/);
                const otherSelected = controlArray.filter((x) => x !== 8 /*Not Required*/);
                if (notRequiredSelected.length > 0 && otherSelected.length > 0) {
                    return { documentDeliveryTypesInvalid: true };
                }
            }
            return null;
        };
    }

    static validPriceRecord(
        charge: FormControl,
        pay: FormControl,
        taxCode: FormControl,
        minValue: FormControl,
        maxValue: FormControl,
        pricingBasisId?: FormControl,
        uomId?: FormControl
    ) {
        return (group: FormGroup): { [key: string]: any } => {
            if (isTruthy(charge.value)) {
                if (isTruthy(minValue.value) && minValue.value < charge.value) {
                    return { charge: true, minValue: true };
                }
                if (isTruthy(maxValue.value) && maxValue.value < charge.value) {
                    return { charge: true, maxValue: true };
                }
            }

            if (isTruthy(pay.value)) {
                if (isTruthy(minValue.value) && minValue.value < pay.value) {
                    return { pay: true, minValue: true };
                }
                if (isTruthy(maxValue.value) && maxValue.value < pay.value) {
                    return { pay: true, maxValue: true };
                }
            }

            if (isTruthy(minValue.value) && isTruthy(maxValue.value) && maxValue.value < minValue.value) {
                return { minValue: true, maxValue: true };
            }

            if ((isTruthy(charge.value) || isTruthy(pay.value)) && !taxCode.value) {
                return { taxCode: true };
            }

            if ((isTruthy(charge.value) || isTruthy(pay.value)) && pricingBasisId && !pricingBasisId.value) {
                return { pricingBasisId: true };
            }

            if (
                (isTruthy(charge.value) || isTruthy(pay.value)) &&
                uomId &&
                !uomId.value &&
                pricingBasisId &&
                (pricingBasisId.value === PricingBasisEnum.PerVolume || pricingBasisId.value === PricingBasisEnum.PerWeight)
            ) {
                return { uomId: true };
            }

            return null;
        };
    }

    static isAmcsEmailAddress(): ValidatorFn {
        return (control: FormControl): { [s: string]: boolean } => {
            if (
                control.value &&
                typeof control.value === 'string' &&
                !(control.value.toLocaleLowerCase() as string).endsWith('amcsgroup.com')
            ) {
                return { amcsEmailInvalid: true };
            }
            return null;
        };
    }
}
