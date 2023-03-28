import { Component, OnDestroy, OnInit } from '@angular/core';
import { TemplateDashboardService } from '../services/template-dashboard.service';
import { Subscription } from 'rxjs';
import { TemplateFeature } from '../models/template-feature.model';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { TemplateFeatureService } from '../services/template-feature.service';
import { take } from 'rxjs/operators';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Component({
  selector: 'app-template-dashboard',
  templateUrl: './template-dashboard.component.html',
  styleUrls: ['./template-dashboard.component.scss'],
  providers: TemplateDashboardService.providers,
})
@aiComponent('Template Dashboard')
export class TemplateDashboardComponent implements OnInit, OnDestroy {
  dashboardTemplate: TemplateFeature = null;
  viewReady = new AiViewReady();

  constructor(public dashboardService: TemplateDashboardService, public featureService: TemplateFeatureService) {}

  private templateSubscription: Subscription;

  ngOnInit() {
    // Dashboard makes requestTemplate call each time it's initialised.
    this.dashboardService.requestTemplate();
    this.templateSubscription = this.featureService.template$.subscribe((template: TemplateFeature) => {
      this.dashboardTemplate = template;
      this.viewReady.next();
    });
  }

  ngOnDestroy() {
    if (isTruthy(this.templateSubscription)) {
      this.templateSubscription.unsubscribe();
    }
  }

  refreshTemplateDate() {
    this.featureService
      .refreshTemplateDate()
      .pipe(take(1))
      .subscribe((templateDate: Date) => {
        this.dashboardTemplate.templateDate = templateDate;
      });
  }

  refreshTemplate() {
    this.featureService
      .refreshTemplate()
      .pipe(take(1))
      .subscribe((template: TemplateFeature) => {
        this.dashboardTemplate = template;
      });
  }

  refreshAll() {
    this.featureService.requestTemplate();
  }
}
