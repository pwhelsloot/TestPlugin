import { Component, ElementRef, OnDestroy, OnInit, Renderer2, ViewChild, ViewEncapsulation } from '@angular/core';
import { GlobalSearchFormService } from '@coreservices/global-search/global-search-form.service';
import { GlobalSearchServiceUI } from '@coreservices/global-search/global-search.service.ui';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
@Component({
    selector: 'app-amcs-global-search',
    templateUrl: './amcs-global-search.component.html',
    styleUrls: ['./amcs-global-search.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class AmcsGlobalSearchComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {

    showGo: boolean;

    @ViewChild('input') searchTermInput: ElementRef;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        private globalSearchServiceUI: GlobalSearchServiceUI,
        public service: GlobalSearchFormService) {
        super(elRef, renderer);

        this.focusSubscription = this.globalSearchServiceUI.focusRequested.subscribe(() => {
            this.searchTermInput.nativeElement.focus();
        });

        this.showGoSubscription = this.globalSearchServiceUI.showGo.subscribe((showGo: boolean) => {
            this.showGo = showGo;
        });
    }

    private focusSubscription: Subscription;
    private showGoSubscription: Subscription;

    ngOnInit() {
        this.service.resetSearch();
    }

    ngOnDestroy() {
        this.focusSubscription.unsubscribe();
        this.showGoSubscription.unsubscribe();
    }
}
