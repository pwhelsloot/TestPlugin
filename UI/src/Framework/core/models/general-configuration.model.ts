import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated To be deleted
 */
export class GeneralConfiguration {

    @alias('IsTabControlReadOnly')
    isTabControlReadOnly: boolean;

    @alias('HideSiteOrderActions')
    hideSiteOrderActions: boolean;

    @alias('CanCancelCallLog')
    canCancelCallLog: boolean;

    @alias('DurationToDisplayCallLogNotification')
    durationToDisplayCallLogNotification: number;

    @alias('MaximumPricesForScaleTicket')
    maximumPricesForScaleTicket: number;
}
