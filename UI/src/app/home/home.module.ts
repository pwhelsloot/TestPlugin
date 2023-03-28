import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { HomeTranslationsService } from '@app/home/services/home-translations.service';
import { MultiTranslateHttpLoader } from '@translate/multi-translation-file-loader';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { SharedModule } from '@shared-module/shared.module';
import { HomeRoutingModule } from './home-routing.module';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { ProfileSettingsComponent } from './my-profile/profile-settings/profile-settings.component';

export function createTranslateLoader(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, [
    { prefix: './assets/i18n/home/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/shared/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/core/', suffix: '.json' },
  ]);
}

@NgModule({
  imports: [
    CommonModule,
    HomeRoutingModule,
    SharedModule,
    FlexLayoutModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  providers: [HomeTranslationsService],
  declarations: [MyProfileComponent, ProfileSettingsComponent],
})
export class HomeModule {}
