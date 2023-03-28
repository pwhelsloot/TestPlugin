import { animate, style, transition, trigger } from '@angular/animations';
import { Component, ElementRef, Input, Renderer2, TemplateRef } from '@angular/core';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { isTruthy } from '../../../core/helpers/is-truthy.function';

@Component({
    selector: 'app-amcs-inner-tile',
    templateUrl: './amcs-inner-tile.component.html',
    styleUrls: ['./amcs-inner-tile.component.scss'],
    animations: [
        trigger('removeSkeleton', [
            transition(':leave', [
                style({ opacity: 1 }),
                animate('250ms', style({ opacity: 0 }))
            ])
        ])
    ]
})
export class AmcsInnerTileComponent extends AutomationLocatorDirective {

    // represents a dashboard tile which is based on a metronic portlet.
    @Input() count: number;
    @Input() caption: string;
    @Input() buttons: { caption; action; icon }[];
    @Input() dropdown: { caption; options; action; id };
    @Input() edit: { action };
    @Input() add: { action };
    @Input() expander: { action };
    @Input() deexpander: { action };
    @Input() inlineExpander: { expanded; action };
    @Input() images: { action };
    @Input() isDynamicHeight = false;
    @Input() height = '320px';
    @Input() minHeight: number = null;
    @Input() maxHeight: number = null;
    @Input() isRemovePadding = false;
    @Input() isForm = false;
    @Input() loading = false;
    @Input() isChild = false;
    @Input() hasBorder = true;
    @Input() isSecondaryColour = false;
    @Input() isTertiaryColour = false;
    @Input() settingsMenu: TemplateRef<any>;
    @Input() overflowVisible = false;
    @Input() overflowAuto = false;
    @Input('customClass') customClass: string;
    @Input() buttonTemplate: TemplateRef<any>;
    showSettings = false;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        public uiService: InnerTileServiceUI) {
        super(elRef, renderer);
    }

    getButtonClasses(caption: string, icon: string, link?: boolean) {
        let css: string;
        if (isTruthy(this.customClass)) {
            css = this.customClass;
        } else {
            if (caption && caption.length > 5) {
                css = 'amcs-light-green btn noMargin btn-xs';
            } else {
                css = 'amcs-light-green btn tiny noMargin btn-xs';
            }

            if (icon) {
                css = css + ' icon';
            }

            if (link) {
                css = css + ' link';
            }
        }
        return css;
    }

    tilesChanged() {
        setTimeout(() => {
            this.uiService.innerTilesChanged.next();
        }, 0);
    }
}
