import { Injectable } from '@angular/core';
import { BaseTranslationsService } from '@core-module/services/translation/base-translations.service';
import { TranslateService } from '@ngx-translate/core';
import { TranslateSettingService } from '@translate/translate-setting.service';

@Injectable({ providedIn: 'root' })
export class SharedTranslationsService extends BaseTranslationsService {
  constructor(readonly translateService: TranslateService, readonly localSettings: TranslateSettingService) {
    super(translateService, localSettings);
  }

  mapTranslations(translationArray: string[]) {
    translationArray['grid.filterPlaceholder'] = this.translateService.instant('grid.filterPlaceholder');
    translationArray['myProfile.myprofileSavedNotification'] = this.translateService.instant('myProfile.myprofileSavedNotification');

    translationArray['map.icon-legend.title'] = this.translateService.instant('map.icon-legend.title');
    translationArray['map.icon-legend.missedStop'] = this.translateService.instant('map.icon-legend.missedStop');
    translationArray['map.icon-legend.completedStop'] = this.translateService.instant('map.icon-legend.completedStop');
    translationArray['map.icon-legend.assignedStop'] = this.translateService.instant('map.icon-legend.assignedStop');
    translationArray['map.icon-legend.routeVisitCurrentCustomer'] = this.translateService.instant(
      'map.icon-legend.routeVisitCurrentCustomer'
    );
    translationArray['map.icon-legend.vehicle'] = this.translateService.instant('map.icon-legend.vehicle');
    translationArray['map.icon-legend.serviceLocation'] = this.translateService.instant('map.icon-legend.serviceLocation');
    translationArray['map.icon-legend.container'] = this.translateService.instant('map.icon-legend.container');
    translationArray['map.icon-legend.close'] = this.translateService.instant('map.icon-legend.close');

    translationArray['map.map-providers.satellite'] = this.translateService.instant('map.map-providers.satellite');
    translationArray['map.map-providers.terrain'] = this.translateService.instant('map.map-providers.terrain');
    translationArray['map.map-providers.classic'] = this.translateService.instant('map.map-providers.classic');
    translationArray['map.map-providers.norway'] = this.translateService.instant('map.map-providers.norway');
    translationArray['map.map-providers.topoGraphic'] = this.translateService.instant('map.map-providers.topoGraphic');
    translationArray['map.map-providers.topoGraphicGrayScale'] = this.translateService.instant('map.map-providers.topoGraphicGrayScale');
    translationArray['map.map-providers.kartdata'] = this.translateService.instant('map.map-providers.kartdata');
    translationArray['map.map-providers.norwayBaseMap'] = this.translateService.instant('map.map-providers.norwayBaseMap');
    translationArray['map.map-providers.norwayBaseMapGreyScale'] = this.translateService.instant('map.map-providers.norwayBaseMapGreyScale');
    translationArray['map.map-providers.european'] = this.translateService.instant('map.map-providers.european');
    translationArray['map.map-providers.seabed'] = this.translateService.instant('map.map-providers.seabed');
    translationArray['map.map-providers.nautical'] = this.translateService.instant('map.map-providers.nautical');
    translationArray['map.map-providers.toporaster'] = this.translateService.instant('map.map-providers.toporaster');
    translationArray['map.map-providers.fjellskygge'] = this.translateService.instant('map.map-providers.fjellskygge');
    translationArray['map.map-providers.basemap'] = this.translateService.instant('map.map-providers.basemap');

    translationArray['map.drawmap.polygon.nextpoint'] = this.translateService.instant('map.drawmap.polygon.nextpoint');
    translationArray['map.drawmap.polygon.start'] = this.translateService.instant('map.drawmap.polygon.start');
    translationArray['map.drawmap.polygon.continue'] = this.translateService.instant('map.drawmap.polygon.continue');
    translationArray['map.drawmap.polygon.createArea'] = this.translateService.instant('map.drawmap.polygon.createArea');
    translationArray['map.drawmap.polygon.intersecterror'] = this.translateService.instant('map.drawmap.polygon.intersecterror');
    translationArray['map.drawmap.polyline.intersecterror'] = this.translateService.instant('map.drawmap.polyline.intersecterror');

    translationArray['map.drawmap.toolbar.save'] = this.translateService.instant('map.drawmap.toolbar.save');
    translationArray['map.drawmap.toolbar.tooltips.save'] = this.translateService.instant('map.drawmap.toolbar.tooltips.save');
    translationArray['map.drawmap.toolbar.undo'] = this.translateService.instant('map.drawmap.toolbar.undo');
    translationArray['map.drawmap.toolbar.tooltips.undo'] = this.translateService.instant('map.drawmap.toolbar.tooltips.undo');
    translationArray['map.drawmap.toolbar.cancel'] = this.translateService.instant('map.drawmap.toolbar.cancel');
    translationArray['map.drawmap.toolbar.tooltips.cancel'] = this.translateService.instant('map.drawmap.toolbar.tooltips.cancel');

    translationArray['dateRangeFilter.custom'] = this.translateService.instant('dateRangeFilter.custom');
    translationArray['dateRangeFilter.lastMonth'] = this.translateService.instant('dateRangeFilter.lastMonth');
    translationArray['dateRangeFilter.nextMonth'] = this.translateService.instant('dateRangeFilter.nextMonth');
    translationArray['dateRangeFilter.lastSevenDays'] = this.translateService.instant('dateRangeFilter.lastSevenDays');
    translationArray['dateRangeFilter.lastThirtyDays'] = this.translateService.instant('dateRangeFilter.lastThirtyDays');
    translationArray['dateRangeFilter.last60Days'] = this.translateService.instant('dateRangeFilter.last60Days');
    translationArray['dateRangeFilter.last120Days'] = this.translateService.instant('dateRangeFilter.last120Days');
    translationArray['dateRangeFilter.nextSevenDays'] = this.translateService.instant('dateRangeFilter.nextSevenDays');
    translationArray['dateRangeFilter.lastThreeMonths'] = this.translateService.instant('dateRangeFilter.lastThreeMonths');
    translationArray['dateRangeFilter.thisMonth'] = this.translateService.instant('dateRangeFilter.thisMonth');
    translationArray['dateRangeFilter.today'] = this.translateService.instant('dateRangeFilter.today');
    translationArray['dateRangeFilter.yesterday'] = this.translateService.instant('dateRangeFilter.yesterday');
    translationArray['dateRangeFilter.requestedDate'] = this.translateService.instant('dateRangeFilter.requestedDate');
    translationArray['dateRangeFilter.requestedDatePlusSeven'] = this.translateService.instant('dateRangeFilter.requestedDatePlusSeven');
    translationArray['dateRangeFilter.requestedDatePlusThirty'] = this.translateService.instant('dateRangeFilter.requestedDatePlusThirty');

    translationArray['pagination.nextPage'] = this.translateService.instant('pagination.nextPage');
    translationArray['pagination.previousPage'] = this.translateService.instant('pagination.previousPage');

    translationArray['tooltip.clearAll'] = this.translateService.instant('tooltip.clearAll');
    translationArray['navbarActions.moreActions'] = this.translateService.instant('navbarActions.moreActions');
    translationArray['navbarActions.addToFavourites'] = this.translateService.instant('navbarActions.addToFavourites');
    translationArray['grid.copied'] = this.translateService.instant('grid.copied');

    translationArray['externalPayment.title'] = this.translateService.instant('externalPayment.title');

    translationArray['directDebit.accountName'] = this.translateService.instant('directDebit.accountName');
    translationArray['directDebit.authorisedSignatory'] = this.translateService.instant('directDebit.authorisedSignatory');
    translationArray['directDebit.accountNo'] = this.translateService.instant('directDebit.accountNo');
    translationArray['directDebit.sortCode'] = this.translateService.instant('directDebit.sortCode');
    translationArray['directDebit.bic'] = this.translateService.instant('directDebit.bic');
    translationArray['directDebit.iban'] = this.translateService.instant('directDebit.iban');
    translationArray['directDebit.umr'] = this.translateService.instant('directDebit.umr');
    translationArray['directDebit.nationalBankCode'] = this.translateService.instant('directDebit.nationalBankCode');
    translationArray['directDebit.nationalCheckDigits'] = this.translateService.instant('directDebit.nationalCheckDigits');
    translationArray['directDebit.rib'] = this.translateService.instant('directDebit.rib');
    translationArray['directDebit.branchCode'] = this.translateService.instant('directDebit.branchCode');
    translationArray['directDebit.dateAuthorised'] = this.translateService.instant('directDebit.dateAuthorised');
    translationArray['directDebit.ddRunConfig'] = this.translateService.instant('directDebit.ddRunConfig');
    translationArray['directDebit.processAsFirst'] = this.translateService.instant('directDebit.processAsFirst');
    translationArray['directDebit.isVerified'] = this.translateService.instant('directDebit.isVerified');
    translationArray['directDebit.accountType'] = this.translateService.instant('directDebit.accountType');

    translationArray['calendar.filterEvents'] = this.translateService.instant('calendar.filterEvents');
    translationArray['calendar.on'] = this.translateService.instant('calendar.on');
    translationArray['calendar.off'] = this.translateService.instant('calendar.off');

    translationArray['componentFilter.filterTypes.equal'] = this.translateService.instant('componentFilter.filterTypes.equal');
    translationArray['componentFilter.filterTypes.notEqual'] = this.translateService.instant('componentFilter.filterTypes.notEqual');
    translationArray['componentFilter.filterTypes.startsWith'] = this.translateService.instant('componentFilter.filterTypes.startsWith');
    translationArray['componentFilter.filterTypes.endsWith'] = this.translateService.instant('componentFilter.filterTypes.endsWith');
    translationArray['componentFilter.filterTypes.contains'] = this.translateService.instant('componentFilter.filterTypes.contains');
    translationArray['componentFilter.filterTypes.isEmpty'] = this.translateService.instant('componentFilter.filterTypes.isEmpty');
    translationArray['componentFilter.filterTypes.isNotEmpty'] = this.translateService.instant('componentFilter.filterTypes.isNotEmpty');
    translationArray['componentFilter.filterTypes.greaterThan'] = this.translateService.instant('componentFilter.filterTypes.greaterThan');
    translationArray['componentFilter.filterTypes.greaterThanOrEqual'] = this.translateService.instant(
      'componentFilter.filterTypes.greaterThanOrEqual'
    );
    translationArray['componentFilter.filterTypes.lessThan'] = this.translateService.instant('componentFilter.filterTypes.lessThan');
    translationArray['componentFilter.filterTypes.lessThanOrEqual'] = this.translateService.instant('componentFilter.filterTypes.lessThanOrEqual');
    translationArray['componentFilter.editor.propertyPlaceholder'] = this.translateService.instant('componentFilter.editor.propertyPlaceholder');
    translationArray['componentFilter.editor.filterTypePlaceholder'] = this.translateService.instant('componentFilter.editor.filterTypePlaceholder');
    translationArray['componentFilter.editor.booleanTypePlaceholder'] = this.translateService.instant('componentFilter.editor.booleanTypePlaceholder');
    translationArray['componentFilter.editor.enumTypePlaceholder'] = this.translateService.instant('componentFilter.editor.enumTypePlaceholder');
    translationArray['componentFilter.editor.booleanFilter.true'] = this.translateService.instant('componentFilter.editor.booleanFilter.true');
    translationArray['componentFilter.editor.booleanFilter.false'] = this.translateService.instant('componentFilter.editor.booleanFilter.false');

    translationArray['reportsearchresult.specialType.standard'] = this.translateService.instant('reportsearchresult.specialType.standard');
    translationArray['reportsearchresult.specialType.myReports'] = this.translateService.instant('reportsearchresult.specialType.myReports');
    translationArray['reportsearchresult.specialType.shared'] = this.translateService.instant('reportsearchresult.specialType.shared');
    translationArray['reportsearchresult.specialType.ssrs'] = this.translateService.instant('reportsearchresult.specialType.ssrs');
    translationArray['globalSearch.businessType.all'] = this.translateService.instant('globalSearch.businessType.all');

    translationArray['filter.clearAllText'] = this.translateService.instant('filter.clearAllText');
    translationArray['filter.all'] = this.translateService.instant('filter.all');

    translationArray['personalAccessToken.personalAccessTokenDeleteConfirmationDialog.message'] = this.translateService.instant(
      'personalAccessToken.personalAccessTokenDeleteConfirmationDialog.message'
    );
    translationArray['personalAccessToken.personalAccessTokenDeleteConfirmationDialog.title'] = this.translateService.instant(
      'personalAccessToken.personalAccessTokenDeleteConfirmationDialog.title'
    );
    translationArray['personalAccessToken.description'] = this.translateService.instant('personalAccessToken.description');
    translationArray['personalAccessToken.userName'] = this.translateService.instant('personalAccessToken.userName');
    translationArray['personalAccessToken.creationDate'] = this.translateService.instant('personalAccessToken.creationDate');
    translationArray['personalAccessToken.expiryDate'] = this.translateService.instant('personalAccessToken.expiryDate');
    translationArray['personalAccessToken.value'] = this.translateService.instant('personalAccessToken.value');
    translationArray['personalAccessToken.title'] = this.translateService.instant('personalAccessToken.title');
    translationArray['personalAccessToken.personalAccessTokenDeletedNotification'] = this.translateService.instant(
      'personalAccessToken.personalAccessTokenDeletedNotification'
    );
    translationArray['personalAccessToken.expireOptions.in1Year'] = this.translateService.instant(
      'personalAccessToken.expireOptions.in1Year'
    );
    translationArray['personalAccessToken.expireOptions.in2Years'] = this.translateService.instant(
      'personalAccessToken.expireOptions.in2Years'
    );
    translationArray['personalAccessToken.expireOptions.never'] = this.translateService.instant('personalAccessToken.expireOptions.never');

    translationArray['amcsMobileLayout.cancel'] = this.translateService.instant('amcsMobileLayout.cancel');
    translationArray['amcsMobileLayout.save'] = this.translateService.instant('amcsMobileLayout.save');

    translationArray['addressMapper.apartmentNumber'] = this.translateService.instant('addressMapper.apartmentNumber');
    translationArray['addressMapper.building'] = this.translateService.instant('addressMapper.building');
    translationArray['addressMapper.city'] = this.translateService.instant('addressMapper.city');
    translationArray['addressMapper.region'] = this.translateService.instant('addressMapper.region');
    translationArray['addressMapper.county'] = this.translateService.instant('addressMapper.county');
    translationArray['addressMapper.state'] = this.translateService.instant('addressMapper.state');
    translationArray['addressMapper.province'] = this.translateService.instant('addressMapper.province');
    translationArray['addressMapper.cOAddress'] = this.translateService.instant('addressMapper.cOAddress');
    translationArray['addressMapper.municipalityName'] = this.translateService.instant('addressMapper.municipalityName');
    translationArray['addressMapper.houseNumber'] = this.translateService.instant('addressMapper.houseNumber');
    translationArray['addressMapper.country'] = this.translateService.instant('addressMapper.country');
    translationArray['addressMapper.postcode'] = this.translateService.instant('addressMapper.postcode');
    translationArray['addressMapper.title'] = this.translateService.instant('addressMapper.title');
    translationArray['addressMapper.streetName'] = this.translateService.instant('addressMapper.streetName');
    translationArray['addressMapper.town'] = this.translateService.instant('addressMapper.town');
    translationArray['addressMapper.address1'] = this.translateService.instant('addressMapper.address1');
    translationArray['addressMapper.address2'] = this.translateService.instant('addressMapper.address2');
    translationArray['addressMapper.address3'] = this.translateService.instant('addressMapper.address3');
    translationArray['addressMapper.address4'] = this.translateService.instant('addressMapper.address4');
    translationArray['addressMapper.address6'] = this.translateService.instant('addressMapper.address6');
    translationArray['addressMapper.address7'] = this.translateService.instant('addressMapper.address7');
    translationArray['addressMapper.address8'] = this.translateService.instant('addressMapper.address8');
    translationArray['addressMapper.address9'] = this.translateService.instant('addressMapper.address9');
    translationArray['addressMapper.floor'] = this.translateService.instant('addressMapper.floor');
    translationArray['addressMapper.houseLetter'] = this.translateService.instant('addressMapper.houseLetter');
    translationArray['addressMapper.houseAddition'] = this.translateService.instant('addressMapper.houseAddition');
    translationArray['addressMapper.invalidCountryAndProvider'] = this.translateService.instant('addressMapper.invalidCountryAndProvider');
    translationArray['addressMapper.invalidCountryAndProviderPdok'] = this.translateService.instant('addressMapper.invalidCountryAndProviderPdok');

    translationArray['addressMapper.kommuneNr'] = this.translateService.instant('addressMapper.kommuneNr');
    translationArray['addressMapper.gnr'] = this.translateService.instant('addressMapper.gnr');
    translationArray['addressMapper.bnr'] = this.translateService.instant('addressMapper.bnr');
    translationArray['addressMapper.snr'] = this.translateService.instant('addressMapper.snr');
    translationArray['addressMapper.fnr'] = this.translateService.instant('addressMapper.fnr');

    translationArray['workflow.missingRoute'] = this.translateService.instant('workflow.missingRoute');

    translationArray['browserGridEditorLayout.deleteTitle'] = this.translateService.instant('browserGridEditorLayout.deleteTitle');
    translationArray['browserGridEditorLayout.deleteMessage'] = this.translateService.instant('browserGridEditorLayout.deleteMessage');
    translationArray['browserGridEditorLayout.unsavedChangedMessage'] = this.translateService.instant('browserGridEditorLayout.unsavedChangedMessage');
    translationArray['browserGridEditorLayout.unsavedChangedTitle'] = this.translateService.instant('browserGridEditorLayout.unsavedChangedTitle');

    translationArray['shared.header.notifications.followupDate'] = this.translateService.instant('shared.header.notifications.followupDate');
    translationArray['shared.header.notifications.assignedToSysUserId'] = this.translateService.instant('shared.header.notifications.assignedToSysUserId');
    translationArray['shared.header.notifications.communicationFollowupStatusId'] = this.translateService.instant('shared.header.notifications.communicationFollowupStatusId');

    translationArray['searchselect.select'] = this.translateService.instant('searchselect.select');
    translationArray['form.definition.mandatory'] = this.translateService.instant('form.definition.mandatory');
    translationArray['form.definition.notmandatory'] = this.translateService.instant('form.definition.notmandatory');
    translationArray['form.definition.editable'] = this.translateService.instant('form.definition.editable');
    translationArray['form.definition.noteditable'] = this.translateService.instant('form.definition.noteditable');
    translationArray['form.definition.visible'] = this.translateService.instant('form.definition.visible');
    translationArray['form.definition.notvisible'] = this.translateService.instant('form.definition.notvisible');
    translationArray['selectTypeahead.typeToSearch'] = this.translateService.instant('selectTypeahead.typeToSearch');
    translationArray['selectTypeahead.noResultsFound'] = this.translateService.instant('selectTypeahead.noResultsFound');
    translationArray['selectTypeahead.itemToCreate'] = this.translateService.instant('selectTypeahead.itemToCreate');
    translationArray['selectTypeahead.loadingText'] = this.translateService.instant('selectTypeahead.loadingText');
    translationArray['dragAndDrop.maxFilesReached'] = this.translateService.instant('dragAndDrop.maxFilesReached');
    translationArray['defaultActionSelector.columns.service'] = this.translateService.instant('defaultActionSelector.columns.service');
    translationArray['defaultActionSelector.columns.action'] = this.translateService.instant('defaultActionSelector.columns.action');
    translationArray['defaultActionSelector.columns.material'] = this.translateService.instant('defaultActionSelector.columns.material');
    translationArray['defaultActionSelector.columns.pricingBasis'] = this.translateService.instant(
      'defaultActionSelector.columns.pricingBasis'
    );
    translationArray['defaultActionSelector.columns.unitOfMeasure'] = this.translateService.instant(
      'defaultActionSelector.columns.unitOfMeasure'
    );
    translationArray['defaultActionSelector.title'] = this.translateService.instant('defaultActionSelector.title');
    translationArray['costeTemplateSelector.columns.service'] = this.translateService.instant('costeTemplateSelector.columns.service');
    translationArray['costeTemplateSelector.columns.action'] = this.translateService.instant('costeTemplateSelector.columns.action');
    translationArray['costeTemplateSelector.columns.material'] = this.translateService.instant('costeTemplateSelector.columns.material');
    translationArray['costeTemplateSelector.columns.pricingBasis'] = this.translateService.instant(
      'costeTemplateSelector.columns.pricingBasis'
    );
    translationArray['costeTemplateSelector.columns.unitOfMeasure'] = this.translateService.instant(
      'costeTemplateSelector.columns.unitOfMeasure'
    );
    translationArray['costeTemplateSelector.columns.supplierSite'] = this.translateService.instant(
      'costeTemplateSelector.columns.supplierSite'
    );
    translationArray['costTemplateSelector.title'] = this.translateService.instant('costTemplateSelector.title');

    translationArray['initiatePayments.dateRangeFilter.dueGreaterThanOrEqualTo61'] = this.translateService.instant('initiatePayments.dateRangeFilter.dueGreaterThanOrEqualTo61');
    translationArray['initiatePayments.dateRangeFilter.due31To60'] = this.translateService.instant('initiatePayments.dateRangeFilter.due31To60');
    translationArray['initiatePayments.dateRangeFilter.due1To30'] = this.translateService.instant('initiatePayments.dateRangeFilter.due1To30');
    translationArray['initiatePayments.dateRangeFilter.dueToday'] = this.translateService.instant('initiatePayments.dateRangeFilter.dueToday');
    translationArray['initiatePayments.dateRangeFilter.dueNegative1ToNegative30'] = this.translateService.instant('initiatePayments.dateRangeFilter.dueNegative1ToNegative30');
    translationArray['initiatePayments.dateRangeFilter.dueNegative31ToNegative60'] = this.translateService.instant('initiatePayments.dateRangeFilter.dueNegative31ToNegative60');
    translationArray['initiatePayments.dateRangeFilter.dueLessThanOrEqualToNegative61'] = this.translateService.instant('initiatePayments.dateRangeFilter.dueLessThanOrEqualToNegative61');

    translationArray['scheduleEditor.title'] = this.translateService.instant('scheduleEditor.title');

    translationArray['changeLog.recordCreated'] = this.translateService.instant('changeLog.recordCreated');
    translationArray['changeLog.recordUpdated'] = this.translateService.instant('changeLog.recordUpdated');

    translationArray['shared.modal.unsavedWarningMessage'] = this.translateService.instant('shared.modal.unsavedWarningMessage');
    translationArray['shared.modal.unsavedWarningTitle'] = this.translateService.instant('shared.modal.unsavedWarningTitle');
  }
}
