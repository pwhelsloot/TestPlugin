import { ChangeDetectionStrategy, Component } from '@angular/core';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';

@Component({
  selector: 'app-storybook-shared-translations-wrapper',
  template: '<div><ng-content></ng-content></div>',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class StorybookSharedTranslationsWrapperComponent {
  // @ts-ignore
  constructor(private readonly translationsService: SharedTranslationsService) {}
}
