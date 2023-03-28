import { AfterContentInit, Component, ContentChild, OnDestroy, TemplateRef, ViewChild } from '@angular/core';
import { BehaviorSubject, of, Subscription } from 'rxjs';
import { AmcsTabState } from './amcs-tab-state.model';
import { IAmcsTab } from './amcs-tab.interface';

@Component({
    selector: 'app-amcs-tab',
    templateUrl: './amcs-tab.component.html',
    styleUrls: ['./amcs-tab.component.scss']
})
export class AmcsTabComponent implements AfterContentInit, OnDestroy {
    @ViewChild(TemplateRef) content: TemplateRef<any>;
    @ContentChild('tab') innerComponent: IAmcsTab;

    loading = true;
    state: AmcsTabState;

    constructor() {
        this.state = new AmcsTabState();
    }

    private loadingSubscription: Subscription;

    ngAfterContentInit() {
        // service is optional, if not supplied then just use mock
        if (this.innerComponent.formService == null) {
            this.innerComponent.formService = {
                form: null,
                initialised: new BehaviorSubject<boolean>(true),
                checkValidation: () => of(true),
                initialise: () => { }
            };
        }
        this.loadingSubscription = this.innerComponent.formService.initialised.subscribe((initalised: boolean) => {
            this.loading = !initalised;
        });
    }

    ngOnDestroy() {
        this.loadingSubscription.unsubscribe();
    }
}
