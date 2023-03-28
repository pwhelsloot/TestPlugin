import { ChangeDetectionStrategy, Component, Input } from '@angular/core';
import { GridActionColumnHeaderIconEnum } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header-icon.enum';
import { GridActionColumnHeaderOptions } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header-options';
import { GridColumnAlignment } from '@shared-module/components/amcs-grid/grid-column-alignment.enum';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-grid-action-column-header',
  templateUrl: './amcs-grid-action-column-header.component.html',
  styleUrls: ['./amcs-grid-action-column-header.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AmcsGridActionColumnHeaderComponent extends AutomationLocatorDirective {
  GridActionColumnHeaderIconEnum = GridActionColumnHeaderIconEnum;
  GridColumnAlignment = GridColumnAlignment;
  @Input() options: GridActionColumnHeaderOptions;
}
