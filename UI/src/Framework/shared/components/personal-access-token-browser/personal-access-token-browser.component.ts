import { AfterViewInit, Component, Input, OnDestroy, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { PersonalAccessToken } from '@core-module/models/personal-access-token/personal-access-token.model';
import { AmcsNotificationService } from '@core-module/services/amcs-notification.service';
import { PersonalAccessTokenBrowserService } from '@core-module/services/persona-access-token/personal-access-token-browser.service';
import { PreviousRouteService } from '@core-module/services/previous-route.service';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { BrowserOptions } from '@shared-module/components/layouts/amcs-browser-grid-layout/browser-options-model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subscription } from 'rxjs';
import { debounceTime, take } from 'rxjs/operators';

@Component({
    selector: 'app-personal-access-token-browser',
    templateUrl: './personal-access-token-browser.component.html',
    styleUrls: ['./personal-access-token-browser.component.scss'],
    providers: PersonalAccessTokenBrowserService.providers,
    animations: PersonalAccessTokenBrowserService.animations
})
export class PersonalAccessTokenBrowserComponent implements OnInit, OnDestroy, AfterViewInit {

    @Input() returnPath: string;
    @ViewChild('privateKeyColumnTemplate') privateKeyColumnTemplate: TemplateRef<any>;

    options: BrowserOptions;
    loading = true;
    columns: GridColumnConfig[];
    personalAccessTokens: PersonalAccessToken[];

    constructor(
        public service: PersonalAccessTokenBrowserService,
        public tileUiService: InnerTileServiceUI,
        private modalService: AmcsModalService,
        private translationsService: SharedTranslationsService,
        private notificationService: AmcsNotificationService,
        private previousRouteService: PreviousRouteService,
        private route: ActivatedRoute) { }

    private subscription: Subscription;

    ngOnInit() {
        this.options = new BrowserOptions();
        this.options.compName = 'PersonalAccessTokenBrowserComponent';
        this.subscription = this.service.personalAccessTokens$.subscribe((tokens: PersonalAccessToken[]) => {
            if (isTruthy(this.service.savedPAT)) {
                const savedToken = tokens.find(t => t.id === this.service.savedPAT.id);
                if (isTruthy(savedToken)) {
                    savedToken.privateKey = this.service.savedPAT.privateKey;
                    savedToken.rowHighlighted = true;
                }
            }
            this.personalAccessTokens = tokens;
            this.loading = false;
        });
        this.service.requestPersonalAccessTokens();
    }

    ngOnDestroy() {
        this.subscription.unsubscribe();
    }

    ngAfterViewInit() {
        this.translationsService.translations
            .pipe(debounceTime(1), take(1))
            .subscribe(translations => {
                this.options = new BrowserOptions();
                this.options.title = translations['personalAccessToken.title'];
                this.options.enableAdd = true;
                this.options.enableClose = true;
                this.columns = PersonalAccessToken.getGridColumns(translations, this.privateKeyColumnTemplate);
            });
    }

    deletePersonalAccessToken(id: number) {
        this.translationsService.translations.pipe(take(1)).subscribe(translations => {
            const dialog = this.modalService.createModal({
                template: translations['personalAccessToken.personalAccessTokenDeleteConfirmationDialog.message'],
                title: translations['personalAccessToken.personalAccessTokenDeleteConfirmationDialog.title'],
                type: 'confirmation',
                largeSize: false
            });

            dialog.afterClosed().pipe(take(1)).subscribe(result => {
                if (result) {
                    this.service.deletePersonalAccessToken(id);
                }
            });
        });
    }

    onCopied() {
        this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
            this.notificationService.showNotification(translations['grid.copied']);
        });
    }

    addPersonalAccessToken() {
        this.service.createToken();
    }

    navigateBack() {
        this.previousRouteService.navigationToPreviousUrl([this.returnPath], { relativeTo: this.route });
    }
}
