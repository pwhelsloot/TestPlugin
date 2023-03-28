import { ActionReducerMap } from '@ngrx/store';
import * as fromAuth from '@auth-module/store/auth.reducers';
import * as fromCustomer from '@core-module/models/external-dependencies/customer.reducers.model';

// this interface defines the total redux state for the application.
// components should access state through this interface.
export interface AppState {
  auth: fromAuth.State;
  customer: fromCustomer.State;
}

// this object has a property for each reducer in the application.
export const reducers: ActionReducerMap<AppState> = {
  auth: fromAuth.authReducer,
  customer: fromCustomer.customerReducer
};
