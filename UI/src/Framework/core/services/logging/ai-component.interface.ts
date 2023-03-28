import { Component } from '@angular/core';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

export interface IAiComponent extends Component {
  viewReady: AiViewReady;
}
