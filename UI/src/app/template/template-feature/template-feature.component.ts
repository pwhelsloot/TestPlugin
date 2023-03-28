import { TemplateFeature } from '@app/template/models/template-feature.model';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { TemplateTranslationsService } from '../services/template-translations.service';
import { TemplateFeatureService } from '../services/template-feature.service';
import { Subscription } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-template-feature',
  templateUrl: './template-feature.component.html',
  styleUrls: ['./template-feature.component.scss'],
  providers: TemplateFeatureService.providers,
})
export class TemplateFeatureComponent implements OnInit, OnDestroy {
  featureTemplate: TemplateFeature = null;

  constructor(public featureService: TemplateFeatureService, public translationsService: TemplateTranslationsService) {}

  private templateSubscription: Subscription;

  ngOnInit() {
    // Notice that no requestTemplate call is automatically made, this is done in dashboard component
    this.templateSubscription = this.featureService.template$.subscribe((template: TemplateFeature) => {
      this.featureTemplate = template;
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
        this.featureTemplate.templateDate = templateDate;
      });
  }

  refreshTemplate() {
    this.featureService
      .refreshTemplate()
      .pipe(take(1))
      .subscribe((template: TemplateFeature) => {
        this.featureTemplate = template;
      });
  }

  refreshAll() {
    this.featureService.requestTemplate();
  }

  navbarTitleClicked(event) {
    console.log('title clicked');
    console.log(event);
  }

  favouriteActionsChanged(event) {
    console.log('favourite actions changed');
    console.log(event);
  }
}
