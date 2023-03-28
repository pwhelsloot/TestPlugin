import { AlertType } from '@shared-module/components/amcs-alert/amcs-alert-config.enum';
import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { AmcsAlertConfig } from '@shared-module/components/amcs-alert/amcs-alert-config.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-alert',
  templateUrl: './amcs-alert.component.html',
  styleUrls: ['./amcs-alert.component.scss']
})
export class AmcsAlertComponent extends AutomationLocatorDirective implements OnInit, OnChanges {
  @Input('config') config: AmcsAlertConfig;
  showAlert: boolean;
  AlertType =  AlertType;

  ngOnInit() {
    this.refreshConfig();
  }

  ngOnChanges(changes: SimpleChanges) {
    this.refreshConfig();
    this.showAlert = true;
  }

  hide() {
    this.showAlert = false;
  }

  private refreshConfig() {
    if (this.config == null) {
      this.config = new AmcsAlertConfig();
    }
    // Sets some defaults but any config properties will take presentence
    this.config = {
      isOpen: true,
      dismissible: true,
      type: AlertType.info,
      ...this.config
    };
  }
}
