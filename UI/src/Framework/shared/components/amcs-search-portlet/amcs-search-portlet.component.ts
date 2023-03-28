import { Component, EventEmitter, Input, Output } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Move to PlatformUI, use app-dashboard-tile instead
 */
@Component({
  selector: 'app-amcs-search-portlet',
  templateUrl: './amcs-search-portlet.component.html',
  styleUrls: ['./amcs-search-portlet.component.scss']
})
export class AmcsSearchPortletComponent extends AutomationLocatorDirective {

  @Input('expandedMode') expandedMode = false;
  @Input('subHeader') subHeader: number;
  @Input('header') header: string;
  @Input('buttons') buttons: { caption; action; icon }[];
  @Input('loading') loading = false;
  @Output('subheaderSelected') subheaderSelected: EventEmitter<any> = new EventEmitter<any>();

  getButtonClasses(caption: string, icon: string, link?: boolean) {
    let css: string;
    css = 'btn noMargin';
    if (icon) {
      css = css + ' icon';
    }
    if (link) {
      css = css + ' link';
    }
    return css;
  }
}
