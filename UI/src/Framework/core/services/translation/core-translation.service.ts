import { Injectable } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { BaseTranslationsService } from './base-translations.service';

@Injectable({ providedIn: 'root' })
export class CoreTranslationsService extends BaseTranslationsService {
  constructor(readonly translateService: TranslateService, readonly localSettings: TranslateSettingService) {
    super(translateService, localSettings);
  }

  mapTranslations(translationArray: string[]) {
    translationArray['columnCustomiser.left'] = this.translateService.instant('columnCustomiser.left');
    translationArray['columnCustomiser.right'] = this.translateService.instant('columnCustomiser.right');
    translationArray['columnCustomiser.gridTile.title'] = this.translateService.instant('columnCustomiser.gridTile.title');
    translationArray['columnCustomiser.accordianTile.title'] = this.translateService.instant('columnCustomiser.accordianTile.title');
    translationArray['columnCustomiser.unusedTile.title'] = this.translateService.instant('columnCustomiser.unusedTile.title');

    translationArray['csrAuthentication.userNotAllowedWebUi'] = this.translateService.instant('csrAuthentication.userNotAllowedWebUi');

    translationArray['globalsearch.searchkeyword.con'] = this.translateService.instant('globalsearch.searchkeyword.con');
    translationArray['globalsearch.searchkeyword.inv'] = this.translateService.instant('globalsearch.searchkeyword.inv');
    translationArray['globalsearch.searchkeyword.ppc'] = this.translateService.instant('globalsearch.searchkeyword.ppc');
    translationArray['globalsearch.searchkeyword.fed'] = this.translateService.instant('globalsearch.searchkeyword.fed');
    translationArray['globalsearch.searchkeyword.son'] = this.translateService.instant('globalsearch.searchkeyword.son');
    translationArray['globalsearch.searchkeyword.pon'] = this.translateService.instant('globalsearch.searchkeyword.pon');
    translationArray['globalsearch.searchkeyword.tna'] = this.translateService.instant('globalsearch.searchkeyword.tna');
    translationArray['globalsearch.searchkeyword.rpt'] = this.translateService.instant('globalsearch.searchkeyword.rpt');
    translationArray['globalsearch.searchkeyword.wdn'] = this.translateService.instant('globalsearch.searchkeyword.wdn');
    translationArray['globalsearch.searchkeyword.stn'] = this.translateService.instant('globalsearch.searchkeyword.stn');
    translationArray['globalsearch.searchkeyword.bcn'] = this.translateService.instant('globalsearch.searchkeyword.bcn');
    translationArray['globalsearch.searchkeyword.acc'] = this.translateService.instant('globalsearch.searchkeyword.acc');
    translationArray['globalsearch.searchkeyword.mat'] = this.translateService.instant('globalsearch.searchkeyword.mat');

    translationArray['globalsearch.searchkeyword.key.con'] = this.translateService.instant('globalsearch.searchkeyword.key.con');
    translationArray['globalsearch.searchkeyword.key.inv'] = this.translateService.instant('globalsearch.searchkeyword.key.inv');
    translationArray['globalsearch.searchkeyword.key.ppc'] = this.translateService.instant('globalsearch.searchkeyword.key.ppc');
    translationArray['globalsearch.searchkeyword.key.fed'] = this.translateService.instant('globalsearch.searchkeyword.key.fed');
    translationArray['globalsearch.searchkeyword.key.son'] = this.translateService.instant('globalsearch.searchkeyword.key.son');
    translationArray['globalsearch.searchkeyword.key.pon'] = this.translateService.instant('globalsearch.searchkeyword.key.pon');
    translationArray['globalsearch.searchkeyword.key.tna'] = this.translateService.instant('globalsearch.searchkeyword.key.tna');
    translationArray['globalsearch.searchkeyword.key.rpt'] = this.translateService.instant('globalsearch.searchkeyword.key.rpt');
    translationArray['globalsearch.searchkeyword.key.wdn'] = this.translateService.instant('globalsearch.searchkeyword.key.wdn');
    translationArray['globalsearch.searchkeyword.key.stn'] = this.translateService.instant('globalsearch.searchkeyword.key.stn');
    translationArray['globalsearch.searchkeyword.key.bcn'] = this.translateService.instant('globalsearch.searchkeyword.key.bcn');
    translationArray['globalsearch.searchkeyword.key.acc'] = this.translateService.instant('globalsearch.searchkeyword.key.acc');
    translationArray['globalsearch.searchkeyword.key.mat'] = this.translateService.instant('globalsearch.searchkeyword.key.mat');

    translationArray['globalsearch.searchkeyword.noresults'] = this.translateService.instant('globalsearch.searchkeyword.noresults');
    translationArray['globalsearch.searchkeyword.placeholder.con'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.con'
    );
    translationArray['globalsearch.searchkeyword.placeholder.inv'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.inv'
    );
    translationArray['globalsearch.searchkeyword.placeholder.ppc'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.ppc'
    );
    translationArray['globalsearch.searchkeyword.placeholder.fed'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.fed'
    );
    translationArray['globalsearch.searchkeyword.placeholder.son'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.son'
    );
    translationArray['globalsearch.searchkeyword.placeholder.pon'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.pon'
    );
    translationArray['globalsearch.searchkeyword.placeholder.tna'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.tna'
    );
    translationArray['globalsearch.searchkeyword.placeholder.rpt'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.rpt'
    );
    translationArray['globalsearch.searchkeyword.placeholder.wdn'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.wdn'
    );
    translationArray['globalsearch.searchkeyword.placeholder.stn'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.stn'
    );
    translationArray['globalsearch.searchkeyword.placeholder.bcn'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.bcn'
    );
    translationArray['globalsearch.searchkeyword.placeholder.acc'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.acc'
    );
    translationArray['globalsearch.searchkeyword.placeholder.mat'] = this.translateService.instant(
      'globalsearch.searchkeyword.placeholder.mat'
    );

    translationArray['headerMenu.accounting'] = this.translateService.instant('headerMenu.accounting');
    translationArray['headerMenu.customerService'] = this.translateService.instant('headerMenu.customerService');
    translationArray['headerMenu.design'] = this.translateService.instant('headerMenu.design');
    translationArray['headerMenu.equipmentInventory'] = this.translateService.instant('headerMenu.equipmentInventory');
    translationArray['headerMenu.materials'] = this.translateService.instant('headerMenu.materials');
    translationArray['headerMenu.inventory'] = this.translateService.instant('headerMenu.inventory');
    translationArray['headerMenu.transportdispatch'] = this.translateService.instant('headerMenu.transportdispatch');
    translationArray['headerMenu.erp'] = this.translateService.instant('headerMenu.erp');
    translationArray['headerMenu.pricesAndProducts'] = this.translateService.instant('headerMenu.pricesAndProducts');
    translationArray['headerMenu.reportsAnalytics'] = this.translateService.instant('headerMenu.reportsAnalytics');
    translationArray['headerMenu.routing'] = this.translateService.instant('headerMenu.routing');
    translationArray['headerMenu.scale'] = this.translateService.instant('headerMenu.scale');
    translationArray['headerMenu.settings'] = this.translateService.instant('headerMenu.settings');
    translationArray['headerMenu.vendors'] = this.translateService.instant('headerMenu.vendors');
    translationArray['headerMenu.extensibility'] = this.translateService.instant('headerMenu.extensibility');
    translationArray['app.initialising'] = this.translateService.instant('app.initialising');
    translationArray['workflow.started'] = this.translateService.instant('workflow.started');
  }
}
