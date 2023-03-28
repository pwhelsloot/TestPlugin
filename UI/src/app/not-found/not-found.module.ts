import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { SharedModule } from '@shared-module/shared.module';
import { MultiTranslateHttpLoader } from '@translate/multi-translation-file-loader';
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { NotFoundRoutingModule } from './not-found-routing.module';
import { NotFoundComponent } from './not-found.component';

export function createTranslateLoader(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, [
    { prefix: './assets/i18n/app/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/shared/', suffix: '.json' },
    { prefix: './Framework/assets/i18n/core/', suffix: '.json' },
  ]);
}

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    NotFoundRoutingModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  declarations: [NotFoundComponent],
})
export class NotFoundModule {}
