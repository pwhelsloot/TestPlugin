import { Component, Input } from '@angular/core';
import { TextLink } from '@core-module/models/links/text-link.interface';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @deprecated Move to PlatformUI
 */
@Component({
  selector: 'app-amcs-workcentre-task-tile',
  templateUrl: './amcs-workcentre-task-tile.component.html',
  styleUrls: ['./amcs-workcentre-task-tile.component.scss']
})
export class AmcsWorkCentreTaskTileComponent extends AutomationLocatorDirective {

  @Input() imageSrc: string;
  @Input() headerText: string;
  @Input() links: TextLink[];
}
