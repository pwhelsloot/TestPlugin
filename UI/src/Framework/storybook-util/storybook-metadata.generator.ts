import { APP_BASE_HREF } from '@angular/common';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Provider } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { RouterModule } from '@angular/router';
import { CoreAppMessagingAdapter } from '@core-module/services/config/core-app-messaging.service';
import { FeatureFlagModule } from '@core-module/services/feature-flag/feature-flag.module';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { StorybookExampleFormComponent } from '@shared-module/components/amcs-form-modal/amcs-form-modal-storybook/storybook-example-form.component';
import { SharedModuleConstants } from '@shared-module/shared.module';
import { StoryBookTranslationService } from '@storybook-util/storybook-translations.service';
import { componentWrapperDecorator, moduleMetadata } from '@storybook/angular';
import { MultiTranslateHttpLoader } from '@translate/multi-translation-file-loader';
import { CalendarModule, DateAdapter } from 'angular-calendar';
import { adapterFactory } from 'angular-calendar/date-adapters/date-fns';
import { StorybookSharedTranslationsWrapperComponent } from './shared-translations-wrapper.component';
import { StorybookCoreUserPreferencesService } from './storybook-core-user-preferences.service';
import { StoryBookMessagingService } from './storybook-messaging.service';

class StorybookMetadataGenerator {
  /**
   * Provides a visual wrapper around a FormControl to improve readability
   * @returns componentWrapperDecorator for use in a storybook story
   */
  generateFormControlWrapperDecorator(customStory: string = undefined) {
    return componentWrapperDecorator((story) => this.generateFormControlWrapper(customStory ? customStory : story));
  }

  /**
   * Provides a visual wrapper around a FormControl to improve readability
   * @returns form control wrapper
   */
  generateFormControlWrapper(story: string) {
    return `
    <div class="row">
      <div class="col-lg-3">
        <div class="portlet box grey child-portlet bordered"><div class="portlet-body">${story}</div></div>
      </div>
    </div>`;
  }

  generateSharedTranslationsWrapper(story: string) {
    return `
    <app-storybook-shared-translations-wrapper>
     ${story}
    </app-storybook-shared-translations-wrapper>`;
  }

  /**
   *
   * @param imports Module imports needed to create the Component
   * @param providers Providers needed to create the Component
   * @param declarations Declarations needed to create the Component
   * @param component Component
   * @returns ModuleMetadata used to create a Storybook module
   */
  generateStorybookModuleMetadata(imports: any[], providers: Provider[], declarations: any[], component) {
    return moduleMetadata({
      imports: [
        ...imports,
        FeatureFlagModule,
        CalendarModule.forRoot({
          provide: DateAdapter,
          useFactory: adapterFactory,
        }),
        TranslateModule.forRoot({
          loader: {
            provide: TranslateLoader,
            useFactory: storybookTranslateLoader,
            deps: [HttpClient],
          },
          isolate: true,
        }),
        BrowserModule,
        BrowserAnimationsModule,
        HttpClientModule,
        RouterModule.forRoot([]),
      ],
      providers: [
        { provide: APP_BASE_HREF, useValue: '/' },
        StoryBookTranslationService,
        StoryBookMessagingService,
        StorybookCoreUserPreferencesService,
        {
          provide: CoreUserPreferencesService,
          useExisting: StorybookCoreUserPreferencesService,
        },
        {
          provide: CoreAppMessagingAdapter,
          useExisting: StoryBookMessagingService,
        },
        ...providers,
      ],
      declarations: [
        ...StorybookMetadataGenerator.excludeComponentFromDeclarations(declarations, component),
        StorybookExampleFormComponent,
        StorybookSharedTranslationsWrapperComponent,
      ],
    });
  }

  private static excludeComponentFromDeclarations(declarations: any[], component: any): any[] {
    return declarations.filter((c) => c.toString() !== component.toString());
  }
}

function storybookTranslateLoader(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, [
    { prefix: './assets/i18n/app/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/shared/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/core/', suffix: '.json' },
  ]);
}

/**
 * Generate all the metadata storybook needs to run a story
 * @param component Component we want to generate a storybook module for
 * @param extraProviders Any extra providers that are needed to run the component/story
 * @returns moduleMetaData storybook uses to create the Story
 */
export function generateModuleMetaDataForStorybook(component: any, extraProviders: any[] = [], extraImports = [], extraDeclarations = []) {
  return new StorybookMetadataGenerator().generateStorybookModuleMetadata(
    [...SharedModuleConstants.moduleImports, ...extraImports],
    [...SharedModuleConstants.moduleProviders, ...extraProviders],
    [...SharedModuleConstants.moduleDeclarations, ...extraDeclarations],
    component
  );
}

export function generateFormControlWrapperDecorator() {
  return new StorybookMetadataGenerator().generateFormControlWrapperDecorator();
}

export function generateFormControlWrapper(story: string) {
  return new StorybookMetadataGenerator().generateFormControlWrapper(story);
}

export function generateSharedTranslationsWrapper(story: string) {
  return new StorybookMetadataGenerator().generateSharedTranslationsWrapper(story);
}
