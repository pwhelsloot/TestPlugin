import { Directive, ElementRef, HostBinding, HostListener, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { Subject, Subscription } from 'rxjs';

@Directive({
    selector: '[appFocusOnError]'
})
export class FocusOnErrorDirective implements OnInit, OnDestroy, OnChanges {
    @Input() appFocusOnError: Subject<void>;
    @HostBinding('attr.autocomplete') autocomplete = 'off';

    constructor(private el: ElementRef, private router: Router) {}

    private errorSubscription: Subscription;

    ngOnInit() {
        this.setErrorSubscription();
    }

    ngOnChanges(changes: SimpleChanges): void {
        if (changes['appFocusOnError']) {
            this.setErrorSubscription();
        }
    }

    ngOnDestroy() {
        if (isTruthy(this.errorSubscription)) {
            this.errorSubscription.unsubscribe();
        }
    }

    // 'onManualFocusError' is a DOM event fired by the different shared component, whereas 'submit' is fired by a form element
    @HostListener('onManualFocusError')
    @HostListener('submit')
    private checkForErrors() {
        const invalidElement = this.el.nativeElement.querySelector('.ng-invalid:not(form)');
        if (invalidElement) {
            // Note: We have all of our form controls wrapped in custom components, so in order for focus to work,
            // we need to grab the actual input, select, etc. element
            const firstElement = invalidElement.querySelector('input, select, mat-form-field') as HTMLElement;
            if (firstElement) {
                firstElement.scrollIntoView(true);

                // Account for the fixed header
                if (window.scrollY) {
                    let scrollPosition = 80;
                    const key = this.getMenuItemKeyFromUrl(this.router.url);
                    if (key === CoreAppRoutes.customerModule) {
                        // Customer nav is 50px height so 80+50
                        scrollPosition = 130;
                    }
                    const yPosition = firstElement.getBoundingClientRect().y;
                    // Don't adjust scroll if control is 'near' bottom. Near = within 300px of bottom, this should be
                    // enough vertical height to ensure it's always within view
                    if (yPosition + 300 < window.innerHeight) {
                        window.scrollBy(0, -scrollPosition);
                    }
                }
                firstElement.focus();
            }
        }
    }

    // Gets the menu item for a URL
    private getMenuItemKeyFromUrl(url: string): string {
        let key: string = null;

        if (isTruthy(url)) {
            const splUrl = url.split('/');
            if (splUrl.length >= 2) {
                key = splUrl[1];
            }
        }
        return key;
    }

    private setErrorSubscription() {
        if (isTruthy(this.errorSubscription)) {
            this.errorSubscription.unsubscribe();
        }
        if (!isTruthy(this.appFocusOnError)) {
            return;
        }
        this.errorSubscription = this.appFocusOnError.subscribe(() => {
            this.checkForErrors();
        });
    }
}
