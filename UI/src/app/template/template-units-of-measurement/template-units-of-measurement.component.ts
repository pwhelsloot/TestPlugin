import { Component, OnInit } from '@angular/core';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Component({
  selector: 'app-template-units-of-measurement',
  templateUrl: './template-units-of-measurement.component.html',
  styleUrls: ['./template-units-of-measurement.component.scss'],
})
@aiComponent('Template Unit of Measurements')
export class TemplateUnitsOfMeasurementComponent implements OnInit {
  viewReady = new AiViewReady();

  constructor() {}

  ngOnInit() {
    this.viewReady.next();
  }
}
