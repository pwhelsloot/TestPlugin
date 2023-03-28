import { animate, style, transition, trigger } from '@angular/animations';
import { Component, Input, TemplateRef } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-dashboard-tile',
  templateUrl: './dashboard-tile.component.html',
  styleUrls: ['./dashboard-tile.component.scss'],
  animations: [
    trigger('removeSkeleton', [
      transition(':leave', [
        style({ opacity: 1 }),
        animate('250ms', style({ opacity: 0 }))
      ])
    ])
  ]
})
export class DashboardTileComponent extends AutomationLocatorDirective {

  // represents a dashboard tile which is based on a metronic portlet.
  @Input() count: number;
  @Input() caption: string;
  @Input() buttons: { caption; action; icon; disabled; extraClasses?}[];
  @Input() closer: { action };
  @Input() expander: { action };
  @Input() deexpander: { action };
  @Input() inlineExpander: { expanded; action };
  @Input() images: { action };
  @Input() isChild = false;
  @Input() isDynamicHeight = false;
  @Input() isRemovePadding = false;
  @Input() loading = false;
  @Input() showRibbon = false;
  @Input() hasBorder = true;
  @Input() forceMinHeight = false;
  @Input() menuTemplate: TemplateRef<any>;

  getButtonClasses(caption: string, icon: string, link?: boolean, extraClasses?: string) {
    let css = 'btn noMargin';

    if (this.isChild) {
      css = `${css} btn-xs`;
    }

    if (icon) {
      css = `${css} icon`;
    }

    if (link) {
      css = `${css} link`;
    }

    if (extraClasses) {
      css = `${css} ${extraClasses}`;
    }

    return css;
  }
}
