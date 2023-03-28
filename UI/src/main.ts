import { enableProdMode } from '@angular/core';
import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';

import { AppModule } from './app/app.module';
import { environment } from './environments/environment';

if (environment.production) {
  enableProdMode();
}

platformBrowserDynamic().bootstrapModule(AppModule).catch(err => console.log(err));

const sNew = document.createElement('script');
sNew.async = true;
sNew.src = environment.exagoApiScriptUrl;
const script = document.getElementsByTagName('script')[0];
script.parentNode.insertBefore(sNew, script);


