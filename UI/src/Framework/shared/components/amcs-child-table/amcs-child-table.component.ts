import { Component, Input, TemplateRef } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ITableItem } from '@shared-module/models/itable-item.model';

/**
 * @deprecated Marked for removal, please use the amcs-grid instead
 */
@Component({
  selector: 'app-amcs-child-table',
  templateUrl: './amcs-child-table.component.html',
  styleUrls: ['./amcs-child-table.component.scss']
})
export class AmcsChildTableComponent extends AutomationLocatorDirective {

  @Input() rows: ITableItem[];
  @Input() rowTemplate: TemplateRef<any>;
  @Input() tableLayoutFixed = false;
}
