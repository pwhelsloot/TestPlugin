import { Component, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2 } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsMenuItem } from '@shared-module/models/amcs-menu-item.model';

/**
 * @deprecated Marked for removal, use app-amcs-select
 */
@Component({
    selector: 'app-amcs-dropdown-select',
    templateUrl: './amcs-dropdown-select.component.html',
    styleUrls: ['./amcs-dropdown-select.component.scss']
})
export class AmcsDropdownSelectComponent extends AutomationLocatorDirective implements OnInit {
    @Input() isDisabled: boolean;
    @Input() dropMenuRight = true;
    @Input() listHeaderText;
    @Input() attachToBody = false;
    @Input() buttonHeight = 34;
    @Input() listMinWidth = 150;
    @Input() buttonMinWidth: number;
    @Input() showTextInButton = false;
    @Input() showIconInButton = true;
    @Input() showSelectIcon = true;
    @Input() items: AmcsMenuItem[] = [];
    // eslint-disable-next-line @angular-eslint/no-output-native
    @Output() change: EventEmitter<AmcsMenuItem> = new EventEmitter<AmcsMenuItem>();

    dropDownMargin = 5;
    selectedItem: AmcsMenuItem;

    constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
        super(elRef, renderer);
    }

    get dropDownPlacement() {
        if (!this.attachToBody) {
            return null;
        }

        return this.dropMenuRight ? 'bottom right' : null;
    }

    get dropDownMarginTop() {
        return this.dropMenuRight && this.attachToBody ? this.buttonHeight / 2 + this.dropDownMargin + 'px' : this.dropDownMargin + 'px';
    }

    ngOnInit() {
        // this component only supports one selected item at a time
        const selectedItems = this.items.filter((x) => x.isSelected);
        if (selectedItems.length > 0) {
            this.selectedItem = selectedItems[0];
            this.change.next(this.selectedItem);
        } else {
            this.selectedItem = new AmcsMenuItem('', '', '', '', true);
        }

        this.items.forEach((x) => {
            if (x !== this.selectedItem) {
                x.isSelected = false;
            }
        });
    }

    itemClicked(item: AmcsMenuItem) {
        item.isSelected = true;

        if (this.selectedItem) {
            this.selectedItem.isSelected = false;
        }

        this.selectedItem = item;
        this.change.next(item);
    }
}
