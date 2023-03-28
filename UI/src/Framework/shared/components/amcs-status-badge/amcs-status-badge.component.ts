import { Component, Input, OnInit } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { StatusBadgeDisplayModeEnum, StatusBadgeTypeEnum } from './status-badge.enum';

/**
 * Shows a status-badge component.
 * @param label Text that is being displayed on the badge, if label is blank (null) it displays progress.
 * @param progress Progress of the status (0-100%), input is optional.
 * @param matchParentWidth If true, takes up 100% of parents width (i.e. for grid), if false puts 10px padding on the status text.
 * @param badgeType Dictates the display color scheme of the badge. Use StatusBadgeTypeEnum to select.
 * @param displayMode Dictates the display mode of the badge. Use StatusBadgeTypeEnum to select (default TextOnly).
 * @param faIconClass FontAwesome icon to display, can take color classes too i.e. 'far fa-info-circle text-warning'.
 */
@Component({
  selector: 'app-amcs-status-badge',
  templateUrl: './amcs-status-badge.component.html',
  styleUrls: ['./amcs-status-badge.component.scss']
})
export class AmcsStatusBadgeComponent extends AutomationLocatorDirective implements OnInit{
  @Input() label: any = null;
  @Input() progress = null;
  @Input() matchParentWidth = false;
  @Input() badgeType: StatusBadgeTypeEnum;
  @Input() displayMode: StatusBadgeDisplayModeEnum = StatusBadgeDisplayModeEnum.TextOnly;
  @Input() faIconClass: string = null;

  StatusBadgeTypeEnum = StatusBadgeTypeEnum;
  StatusBadgeDisplayModeEnum = StatusBadgeDisplayModeEnum;

  ngOnInit(){
    if (this.label === null ){
      this.label = this.progress;
    }
  }
}
