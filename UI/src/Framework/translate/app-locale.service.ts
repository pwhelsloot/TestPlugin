import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { TranslateSettingService } from './translate-setting.service';
import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class AppLocaleService extends BaseService {

    constructor(private localSettings: TranslateSettingService) {
        super();

        this.localSettings.selectedLanguage
            .pipe(takeUntil(this.unsubscribe))
            .subscribe((language: string) => {
                this.locale = isTruthy(language) ? language : 'en';
            });
    }

    private locale = 'en';

    getLocale() {
        return this.locale;
    }
}
