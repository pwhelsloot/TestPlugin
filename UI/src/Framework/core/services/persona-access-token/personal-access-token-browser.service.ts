import { animate, style, transition, trigger } from '@angular/animations';
import { Injectable } from '@angular/core';
import { UserLookup } from '@core-module/models/lookups/user-lookup.model';
import { PersonalAccessToken } from '@core-module/models/personal-access-token/personal-access-token.model';
import { BaseService } from '@coreservices/base.service';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Observable, Subject } from 'rxjs';
import { switchMap, take, takeUntil } from 'rxjs/operators';
import { PersonalAccessTokenBrowserServiceData } from './data/personal-access-token-browser.service.data';

@Injectable()
export class PersonalAccessTokenBrowserService extends BaseService {

    static animations = [
        trigger('expandAndCollapse', [
            transition('void => *', [
                style({ height: 0, overflow: 'hidden' }),
                animate('200ms', style({ height: '*' }))
            ]),
            transition('* => void', [
                style({ height: '*', overflow: 'hidden' }),
                animate('200ms', style({ height: 0 }))
            ])
        ])];

    static providers = [PersonalAccessTokenBrowserService, PersonalAccessTokenBrowserServiceData];

    addingToken = false;
    personalAccessTokens$: Observable<PersonalAccessToken[]>;
    users$: Observable<UserLookup[]>;
    savedPAT: PersonalAccessToken;

    constructor(private dataService: PersonalAccessTokenBrowserServiceData,
        private translationsService: SharedTranslationsService) {
        super();
        this.setupStream();
    }

    private personalAccessTokens: Subject<PersonalAccessToken[]> = new Subject<PersonalAccessToken[]>();
    private users: Subject<UserLookup[]> = new Subject<UserLookup[]>();
    private personalAccessTokensRequest = new Subject();
    private usersRequest = new Subject();

    requestPersonalAccessTokens() {
        this.personalAccessTokensRequest.next();
    }

    requestUsers() {
        this.setupUsersStream();
        this.usersRequest.next();
    }

    deletePersonalAccessToken(id: number) {
        this.translationsService.translations.pipe(
            take(1),
            switchMap(translations => {
                const successMessage = translations['personalAccessToken.personalAccessTokenDeletedNotification'];
                return this.dataService.deletePersonalAccessToken(successMessage, id);
            })).subscribe((result: boolean) => {
                if (result) {
                    this.requestPersonalAccessTokens();
                }
            });
    }

    editorReturn() {
        this.addingToken = false;
    }

    createToken() {
        if (this.addingToken) {
            return false;
        } else {
            this.addingToken = true;
            return true;
        }
    }

    savePersonalAccessToken(token: PersonalAccessToken) {
        return this.translationsService.translations.pipe(
            take(1),
            switchMap(translations => {
                const successMessage = translations['personalAccessToken.personalAccessTokenSavedNotification'];
                return this.dataService.savePersonalAccessToken(successMessage, token);
            }));
    }

    private setupStream() {
        this.personalAccessTokens$ = this.personalAccessTokens.asObservable();
        this.personalAccessTokensRequest
            .pipe(
                takeUntil(this.unsubscribe),
                switchMap(() => {
                    return this.dataService.getPersonalAccessTokens();
                })
            )
            .subscribe(tokens => {
                this.personalAccessTokens.next(tokens);
            });
    }

    private setupUsersStream() {
        this.users$ = this.users.asObservable();
        this.usersRequest
            .pipe(
                takeUntil(this.unsubscribe),
                switchMap(() => {
                    return this.dataService.getUsers();
                })
            )
            .subscribe(users => {
                users.forEach(user => {
                    user.description = user.forename + ' ' + user.surname;
                });
                this.users.next(users);
            });
    }
}
