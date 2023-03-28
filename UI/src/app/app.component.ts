import { Component, OnInit } from '@angular/core';
import { TemplateInitialisationService } from '@app/template-adapter-services/template-initialisation-service';
import { CoreTranslationsService } from '@core-module/services/translation/core-translation.service';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { combineLatest } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-body',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
  initialisingMessage: string;
  appInitialised = false;

  constructor(
    public localSettings: TranslateSettingService,
    private templateInitialisationService: TemplateInitialisationService,
    private translationsService: CoreTranslationsService
  ) {}

  ngOnInit() {
    this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      this.initialisingMessage = translations['app.initialising'];
    });
    combineLatest([this.templateInitialisationService.authStatusFinished, this.templateInitialisationService.initialised$])
      .pipe(take(1))
      .subscribe(() => {
        this.appInitialised = true;
      });
  }
}
