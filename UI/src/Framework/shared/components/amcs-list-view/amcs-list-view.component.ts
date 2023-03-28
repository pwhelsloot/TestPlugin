import { animate, style, transition, trigger } from '@angular/animations';
import { Component, ElementRef, Input, Renderer2, TemplateRef } from '@angular/core';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-list-view',
  templateUrl: './amcs-list-view.component.html',
  styleUrls: ['./amcs-list-view.component.scss'],
  animations: [
    trigger('expandAndCollapse', [
      transition('void => *', [
        style({ height: 0, overflow: 'hidden' }),
        animate('200ms', style({ height: '*' }))
      ]),
      transition('* => void', [
        style({ height: '*', overflow: 'hidden' }),
        animate('200ms', style({ height: 0 }))
      ])
    ])]
})
export class AmcsListViewComponent extends AutomationLocatorDirective {

  @Input() addNewItem = false;
  @Input() items = [];
  @Input() expandable = false;
  @Input() listItemHeaderTemplate: TemplateRef<any>;
  @Input() listItemTemplate: TemplateRef<any>;
  @Input() listItemTotalTemplate: TemplateRef<any>;
  @Input() formTemplate: TemplateRef<any>;
  @Input() noDataAvailableTemplate: TemplateRef<any>;
  @Input() isBelowHeader = false;
  @Input() noBorder = false;
  @Input() noOuterBorder = false;
  @Input() noMargin = false;
  @Input() noPadding = false;
  @Input() isMobile = false;
  @Input() noSelectionBorder = false;
  @Input() alternateRowColor = true;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public tileUiService: InnerTileServiceUI) {
    super(elRef, renderer);
  }

  listItemClicked(item) {
    if (this.expandable) {
      item.isSelected = !item.isSelected;
    }
  }
}
