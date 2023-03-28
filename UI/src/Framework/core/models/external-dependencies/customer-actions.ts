/* eslint-disable max-classes-per-file */
/* tslint:disable:max-classes-per-file */
import { Filter } from '@coremodels/filter/Filter';
import { Action } from '@ngrx/store';

/**
 * @deprecated Move to PlatformUI
 */
export const SAVE_OPERATIONS_SELECTED_JOB_STATE = 'SAVE_OPERATIONS_SELECTED_JOB_STATE';
export const SAVE_OPERATIONS_JOB_SELECTED_FILTERS_STATE = 'SAVE_OPERATIONS_JOB_SELECTED_FILTERS_STATE';
export const SAVE_OPERATIONS_JOB_SHOW_FILTER_STATE = 'SAVE_OPERATIONS_JOB_SHOW_FILTER_STATE';
export const SAVE_OPERATIONS_SELECTED_DATE_RANGE_FILTER_STATE = 'SAVE_OPERATIONS_SELECTED_DATE_RANGE_FILTER_STATE';
export const SAVE_OPERATIONAL_AREA_SHOW_FILTER_STATE = 'SAVE_OPERATIONAL_AREA_SHOW_FILTER_STATE';
export const SAVE_OPERATIONAL_AREA_SELECTED_FILTERS_STATE = 'SAVE_OPERATIONAL_AREA_SELECTED_FILTERS_STATE';
export const SAVE_SERVICE_AGREEMENT_SHOW_FILTER_STATE = 'SAVE_SERVICE_AGREEMENT_SHOW_FILTER_STATE';
export const SAVE_SERVICE_AGREEMENT_SELECTED_FILTERS_STATE = 'SAVE_SERVICE_AGREEMENT_SELECTED_FILTERS_STATE';
export const SAVE_SERVICE_AGREEMENT_SELECTED_DATE_RANGE_FILTER_STATE = 'SAVE_SERVICE_AGREEMENT_SELECTED_DATE_RANGE_FILTER_STATE';
export const SAVE_JOURNAL_SELECTED_COMPANY_STATE = 'SAVE_JOURNAL_SELECTED_COMPANY_STATE';

export class SaveOperationsSelectedJobState implements Action {
    readonly type = SAVE_OPERATIONS_SELECTED_JOB_STATE;

    constructor(public payload: { id: number; date: Date }) { }
}

export class SaveOperationsJobSelectedFiltersState implements Action {
    readonly type = SAVE_OPERATIONS_JOB_SELECTED_FILTERS_STATE;

    constructor(public payload: Filter) { }
}

export class SaveOperationsJobShowFilterState implements Action {
    readonly type = SAVE_OPERATIONS_JOB_SHOW_FILTER_STATE;

    constructor(public payload: boolean) { }
}

export class SaveOperationsSelectedDateRangeFilterState implements Action {
    readonly type = SAVE_OPERATIONS_SELECTED_DATE_RANGE_FILTER_STATE;

    constructor(public payload: [Date, Date]) { }
}

export class SaveOperationalAreaShowFilterState implements Action {
    readonly type = SAVE_OPERATIONAL_AREA_SHOW_FILTER_STATE;

    constructor(public payload: boolean) { }
}

export class SaveOperationalAreaSelectedFiltersState implements Action {
    readonly type = SAVE_OPERATIONAL_AREA_SELECTED_FILTERS_STATE;

    constructor(public payload: Filter) { }
}

export class SaveServiceAgreementShowFilterState implements Action {
    readonly type = SAVE_SERVICE_AGREEMENT_SHOW_FILTER_STATE;

    constructor(public payload: boolean) { }
}

export class SaveServiceAgreementSelectedFiltersState implements Action {
    readonly type = SAVE_SERVICE_AGREEMENT_SELECTED_FILTERS_STATE;

    constructor(public payload: Filter) { }
}

export class SaveServiceAgreementSelectedDateRangeFilterState implements Action {
    readonly type = SAVE_SERVICE_AGREEMENT_SELECTED_DATE_RANGE_FILTER_STATE;

    constructor(public payload: [Date, Date]) { }
}

export class SaveJournalSelectedCompanyState implements Action {
    readonly type = SAVE_JOURNAL_SELECTED_COMPANY_STATE;

    constructor(public payload: Number) { }
}

export type CustomerActions = SaveOperationsSelectedJobState |
                              SaveOperationsJobSelectedFiltersState |
                              SaveOperationsJobShowFilterState |
                              SaveOperationsSelectedDateRangeFilterState |
                              SaveOperationalAreaShowFilterState |
                              SaveOperationalAreaSelectedFiltersState |
                              SaveServiceAgreementShowFilterState |
                              SaveServiceAgreementSelectedFiltersState |
                              SaveServiceAgreementSelectedDateRangeFilterState |
                              SaveJournalSelectedCompanyState;
