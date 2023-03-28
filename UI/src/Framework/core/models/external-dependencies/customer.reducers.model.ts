import { Filter } from '@coremodels/filter/Filter';
import * as CustomerActions from './customer-actions';
/**
 * @deprecated Move to PlatformUI
 */
export interface State {
    operationsState: OperationsState;
}

export interface OperationsState {
    selectedTransportJobId: number;
    selectedJobfilters: Filter;
    showJobFilter: boolean;
    selectedTransportJobDateRange: [Date, Date];
    selectedTransportJobDate: Date;
    showServiceAgreementFilter: boolean;
    selectedServiceAgreementFilters: Filter;
    selectedServiceAgreementDateRange: [Date, Date];
    selectedCompany: Number;
}

const initialState: State = {
    operationsState: {
        selectedTransportJobId: null,
        selectedJobfilters: null,
        showJobFilter: false,
        selectedTransportJobDateRange: null,
        selectedTransportJobDate: null,
        showServiceAgreementFilter: false,
        selectedServiceAgreementFilters: null,
        selectedServiceAgreementDateRange: null,
        selectedCompany: null
    }
};

export function customerReducer(state = initialState, action: CustomerActions.CustomerActions) {
    switch (action.type) {
        case CustomerActions.SAVE_OPERATIONS_SELECTED_JOB_STATE:
            return {
                ...state,
                operationsState: {
                    ...state.operationsState,
                    selectedTransportJobId: action.payload.id,
                    selectedTransportJobDate: action.payload.date
                }
            };
        case CustomerActions.SAVE_OPERATIONS_JOB_SELECTED_FILTERS_STATE:
            return {
                ...state,
                operationsState: {
                    ...state.operationsState,
                    selectedJobfilters: action.payload
                }
            };
        case CustomerActions.SAVE_OPERATIONS_JOB_SHOW_FILTER_STATE:
            return {
                ...state,
                operationsState: {
                    ...state.operationsState,
                    showJobFilter: action.payload
                }
            };
        case CustomerActions.SAVE_OPERATIONS_SELECTED_DATE_RANGE_FILTER_STATE:
            return {
                ...state,
                operationsState: {
                    ...state.operationsState,
                    selectedTransportJobDateRange: action.payload
                }
            };
        case CustomerActions.SAVE_SERVICE_AGREEMENT_SHOW_FILTER_STATE:
            return {
                ...state,
                operationsState: {
                    ...state.operationsState,
                    showServiceAgreementFilter: action.payload
                }
            };
        case CustomerActions.SAVE_SERVICE_AGREEMENT_SELECTED_FILTERS_STATE:
            return {
                ...state,
                operationsState: {
                    ...state.operationsState,
                    selectedServiceAgreementFilters: action.payload
                }
            };
        case CustomerActions.SAVE_SERVICE_AGREEMENT_SELECTED_DATE_RANGE_FILTER_STATE:
            return {
                ...state,
                operationsState: {
                    ...state.operationsState,
                    selectedServiceAgreementDateRange: action.payload
                }
            };
        case CustomerActions.SAVE_JOURNAL_SELECTED_COMPANY_STATE:
            return {
                ...state,
                operationsState: {
                    ...state.operationsState,
                    selectedCompany: action.payload
                }
            };
        default:
            return state;
    }
}
