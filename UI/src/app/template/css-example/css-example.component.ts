import { Component, OnInit } from '@angular/core';
import { CssExampleEnum } from './css-example.enum';
import { take } from 'rxjs/operators';
import { AmcsNotificationService } from '@core-module/services/amcs-notification.service';
import { TemplateTranslationsService } from '../services/template-translations.service';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

@Component({
  selector: 'app-css-example',
  templateUrl: './css-example.component.html',
  styleUrls: ['./css-example.component.scss'],
})
@aiComponent('Template CSS Example')
export class CssExampleComponent implements OnInit {
  viewReady = new AiViewReady();
  selection: CssExampleEnum = CssExampleEnum.Encapsulated;
  CssExampleEnum = CssExampleEnum;
  hasBleedingBeenLoaded = false;

  constructor(private translationsService: TemplateTranslationsService, private notificationService: AmcsNotificationService) {}

  ngOnInit() {
    this.showMessage();
    this.viewReady.next();
  }

  selectionChanged(value: CssExampleEnum) {
    this.selection = value;
    this.showMessage();
  }

  private showMessage() {
    this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      if (this.selection === CssExampleEnum.Encapsulated) {
        this.notificationService.showLongNotification(translations['template.cssExample.encapsulatedLoaded']);
      } else {
        this.notificationService.showLongNotification(translations['template.cssExample.bleedingLoaded']);
        this.hasBleedingBeenLoaded = true;
      }
    });
  }
}
