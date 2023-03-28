import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Use the AmcsBrowserTileComponent instead
 */
@Component({
  selector: 'app-amcs-browser-tile-deprecated',
  templateUrl: './amcs-browser-tile-deprecated.component.html',
  styleUrls: ['./amcs-browser-tile-deprecated.component.scss'],
})
export class AmcsBrowserTileDeprecatedComponent extends AutomationLocatorDirective implements OnInit {
  @Input() caption: string;
  @Input() deexpander: { action };
  @Input() add: { action };
  @Input() edit: { action };
  @Input() delete: { action };
  @Input() isChild = false;
  @Input() loading = false;
  @Input() close: { action };
  @Input() buttons: { cssClass; caption; link; action; icon; disabled; extraClasses?}[] = [];
  @Input() menuTemplate: TemplateRef<any>;
  @Input() removePadding = false;

  ngOnInit() {
    this.getButtonClasses();
  }

  getButtonClasses() {
    this.buttons?.forEach((button) => {
      button.cssClass = 'btn noMargin';
      if (button.icon) {
        button.cssClass = `${button.cssClass} button.icon`;
      }
      if (button.link) {
        button.cssClass = `${button.cssClass} button.link`;
      }
      if (button.extraClasses) {
        button.cssClass = `${button.cssClass} ${button.extraClasses}`;
      }
    });
  }
}
