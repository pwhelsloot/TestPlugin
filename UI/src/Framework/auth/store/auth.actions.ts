/* eslint-disable max-classes-per-file */
/* tslint:disable:max-classes-per-file */
import { AuthorizedUser } from '@auth-module/models/authorized-user.model';
import { Action } from '@ngrx/store';

export const TRY_INITIALISE = 'TRY_INITIALISE';
export const INITIALISE = 'INITIALISE';
export const SIGNIN = 'SIGNIN';
export const TRY_SIGNOUT = 'TRY_SIGNOUT';
export const SIGNOUT = 'SIGNOUT';
export const INVALID_SIGNIN = 'INVALID_SIGNIN';
export const ACCOUNT_LOCKED = 'ACCOUNT_LOCKED';
export const NO_ACTION = 'NO_ACTION';

export class TryInitialise implements Action {
  readonly type = TRY_INITIALISE;
}

export class Signin implements Action {
  readonly type = SIGNIN;

  constructor(public payload: AuthorizedUser) { }
}

export class TrySignout implements Action {
  readonly type = TRY_SIGNOUT;
  constructor(public returnURL?: string) { }
}

export class Signout implements Action {
  readonly type = SIGNOUT;
}

export class Initialise implements Action {
  readonly type = INITIALISE;
}

export class NoAction implements Action {
  readonly type = NO_ACTION;
}

export type AuthActions = TryInitialise | Initialise | Signin | TrySignout | Signout | NoAction;
