import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';

@Injectable()
export class StoryBookTranslationService {
  //Hack to allow preview.js to send locale query param on language change
  static storyBookTranslationServiceReference: StoryBookTranslationService;
  constructor(private readonly translateService: TranslateService) {}

  loadTranslations(locale: string) {
    this.translateService.use(locale);
  }

  mapTranslations(translationArray: string[]) {}
}
