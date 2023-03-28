import * as AuthActions from './auth.actions';

export interface State {
  initialised: boolean;
  authenticated: boolean;
  username: string;
  sysUserId: number;
  companyoutletid: number;
}

const initialState: State = {
  initialised: false,
  authenticated: false,
  username: null,
  sysUserId: null,
  companyoutletid: null,
};

export function authReducer(
  state = initialState,
  action: AuthActions.AuthActions
) {
  switch (action.type) {
    case AuthActions.INITIALISE:
      return {
        ...state,
        initialised: true,
      };
    case AuthActions.SIGNIN:
      return {
        ...state,
        authenticated: true,
        username: action.payload.username,
        sysUserId: action.payload.sysUserId,
        companyoutletid: action.payload.companyoutletid,
        isExternalLogoutRequired: action.payload.isExternalLogoutRequired,
      };
    case AuthActions.SIGNOUT:
      return {
        ...state,
        authenticated: false,
        companyOutletId: null,
        sysUserId: null,
      };
    default:
      return state;
  }
}
