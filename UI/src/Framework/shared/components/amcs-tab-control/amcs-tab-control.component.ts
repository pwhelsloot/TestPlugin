import { AfterContentInit, Component, ContentChildren, ElementRef, EventEmitter, Input, Output, QueryList, Renderer2 } from '@angular/core';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { ApplicationConfiguration } from '@coremodels/application-configuration.model';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { AmcsTabControlService } from '@coreservices/tab-control/amcs-tab-control.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { take } from 'rxjs/operators';
import { AmcsTabComponent } from '../amcs-tab/amcs-tab.component';

@Component({
    selector: 'app-amcs-tab-control',
    templateUrl: './amcs-tab-control.component.html',
    styleUrls: ['./amcs-tab-control.component.scss']
})
export class AmcsTabControlComponent extends AutomationLocatorDirective implements AfterContentInit {
    stretchTabs = null;

    @Input() title: string;
    @Input() titleIcon: string;
    @Input() tabContentHeight: string;
    @Input() isReadOnlyMode: boolean;
    @Input() isChild = false;
    @Input() noPadding = false;
    @Input() isStretchTabs = false;
    @Input() customClass: string;
    @Output() onSelectedTabIdChanged: EventEmitter<number> = new EventEmitter<number>();
    @Output() onSelectedTabIdBeforeChange: EventEmitter<number> = new EventEmitter<number>();
    @Output() onSelectedTabInitialised: EventEmitter<any> = new EventEmitter<any>();
    @Output() onSubmit: EventEmitter<any> = new EventEmitter<any>();
    @Output() onCancel: EventEmitter<any> = new EventEmitter<any>();

    @ContentChildren(AmcsTabComponent) tabs: QueryList<AmcsTabComponent>;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        public tabService: AmcsTabControlService,
        public tileUiService: InnerTileServiceUI,
        private appConfig: ApplicationConfigurationStore
    ) {
        super(elRef, renderer);
        this.appConfig.configuration$.pipe(take(1))
            .subscribe((config: ApplicationConfiguration) => {
                this.isReadOnlyMode = config.generalConfiguration.isTabControlReadOnly;
            });
    }

    ngAfterContentInit() {
        setTimeout(() => {
            this.tabService.onSelectedTabIdChanged = this.onSelectedTabIdChanged;
            this.tabService.onSelectedTabIdBeforeChange = this.onSelectedTabIdBeforeChange;
            this.tabService.onSelectedTabInitialised = this.onSelectedTabInitialised;
            this.tabService.onSubmit = this.onSubmit;
            this.tabService.onCancel = this.onCancel;
            this.tabService.title = this.title;
            this.tabService.tabContentHeight = this.tabContentHeight;
            this.tabService.tabs = this.tabs;
            this.tabService.isReadOnlyMode = this.isReadOnlyMode;
            if (this.isStretchTabs) {
                this.stretchTabs = this.isStretchTabs;
            }
        }, 1);
    }
}
