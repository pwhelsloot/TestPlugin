import { NgModule } from '@angular/core';
import { SharedModule } from '@shared-module/shared.module';
import { TranslateModule, TranslateLoader } from '@ngx-translate/core';
import { HttpClient } from '@angular/common/http';
import { TemplateHeaderComponent } from './header/header.component';
import { TemplatePageLayoutComponent } from './page-layout/page-layout.component';
import { TemplateHeaderDesktopComponent } from './header/header-desktop/header-desktop.component';
import { MultiTranslateHttpLoader } from '@translate/multi-translation-file-loader';
import { TemplateHeaderMobileComponent } from './header/header-mobile/header-mobile.component';
import { TemplateHeaderMenuComponent } from './header/header-menu/header-menu.component';

export function createTranslateLoader(http: HttpClient) {
  return new MultiTranslateHttpLoader(http, [{ prefix: './assets/i18n/template/', suffix: '.json' }]);
}

@NgModule({
  imports: [
    SharedModule,
    TranslateModule.forChild({
      loader: {
        provide: TranslateLoader,
        useFactory: createTranslateLoader,
        deps: [HttpClient],
      },
      isolate: true,
    }),
  ],
  exports: [TemplateHeaderComponent, TemplatePageLayoutComponent],
  providers: [],
  declarations: [
    TemplateHeaderComponent,
    TemplateHeaderDesktopComponent,
    TemplateHeaderMobileComponent,
    TemplatePageLayoutComponent,
    TemplateHeaderMenuComponent,
  ],
})
export class TemplateSharedModule {}
