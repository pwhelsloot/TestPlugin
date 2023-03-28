import { Component, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2, TemplateRef, ViewEncapsulation } from '@angular/core';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsDropdownIconEnum } from './amcs-dropdown-icon-enum.model';
import { AmcsDropdownOpenChange } from './amcs-dropdown-open-change.model';

@Component({
    selector: 'app-amcs-dropdown',
    templateUrl: './amcs-dropdown.component.html',
    styleUrls: ['./amcs-dropdown.component.scss'],
    encapsulation: ViewEncapsulation.None
})
export class AmcsDropdownComponent extends AutomationLocatorDirective implements OnInit {

    @Input('isDisabled') isDisabled: boolean;
    @Input('dropdownIcon') dropdownIcon = AmcsDropdownIconEnum.Standard;
    @Input('useLargeEllipsis') useLargeEllipsis: boolean;
    @Input('customIconTemplate') customIconTemplate: TemplateRef<any>;
    @Input('attachToBody') attachToBody = true;
    @Input('placement') placement = 'bottom right';
    @Output('openChange') openChange = new EventEmitter<AmcsDropdownOpenChange>();
    @Input() loading = false;
    @Input('dropup') dropup = false;
    @Input() displayMode: FormControlDisplay = FormControlDisplay.Standard;
    @Input('menuCustomClass') menuCustomClass: string;
    @Input('disabledTooltip') disabledTooltip: string;
    containerbody: string;
    AmcsDropdownIconEnum = AmcsDropdownIconEnum;
    FormControlDisplay = FormControlDisplay;

    constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
        super(elRef, renderer);
    }

    ngOnInit() {
        if (this.attachToBody) {
            this.containerbody = 'body';
        }
    }
}
