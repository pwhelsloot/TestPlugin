
import { Injectable } from '@angular/core';
import { AbstractControl, Validators } from '@angular/forms';
import { GpsCoordinateHelper } from '@core-module/helpers/gps/coordinate-helper';
import { isNullOrUndefined, isTruthy } from '@core-module/helpers/is-truthy.function';
import { nameof } from '@core-module/helpers/name-of.function';
import { AddressControlTypeEnum } from '@core-module/models/address/address-control-type.enum';
import { AddressDynamicControl } from '@core-module/models/address/address-dynamic-control.model';
import { AddressFieldDisplayConfiguration } from '@core-module/models/address/address-field-display-configuration.model';
import { AddressSearchCountryEnum } from '@core-module/models/address/address-search-country.enum';
import { AddressSearchMethod } from '@core-module/models/address/address-search-method.enum';
import { AddressSearchProviderEnum } from '@core-module/models/address/address-search-provider.enum';
import { AddressValidationForm } from '@core-module/models/address/address-validation-form.model';
import { AddressValidatorMapConfig } from '@core-module/models/address/address-validation-map-config.constants';
import { AddressValidationSearchResult } from '@core-module/models/external-dependencies/address-validation-search-result.model';
import { AddressValidation } from '@coremodels/address/address-validation.model';
import { ApplicationConfiguration } from '@coremodels/application-configuration.model';
import { Country } from '@coremodels/lookups/country.model';
import { AmcsLeafletMapService } from '@coreservices/amcs-leaflet-map.service';
import { AmcsLeafletDataType } from '@shared-module/amcs-leaflet-map/enums/amcs-leaflet-datatype.enum';
import { AmcsLeafletMapGrouping } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapMarker } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-marker.model';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { ApplicationConfigurationStore } from '../config/application-configuration.store';
import { ErrorNotificationService } from '../error-notification.service';
import { AddressValidationService } from './address-validation.service';
import { AddressValidationServiceData } from './address-validation.service.data';
import { BrregAddressService } from './brreg-address.service';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
@Injectable()
export class AddressValidationFormService extends BaseService {

    static providers = [AddressValidationFormService, AddressValidationService, AddressValidationServiceData];

    form: AmcsFormGroup;
    countries: Country[];
    countryLat: number;
    countryLong: number;
    mapZoomLevel = 5;
    mapGroupings: AmcsLeafletMapGrouping[] = [];
    isReadOnlyMode: boolean;
    dynamicControls: AddressDynamicControl[] = [];
    colSpan = 12;

    item: AddressValidationSearchResult = null;
    searchTextSubject = new Subject<string>();
    searchResult: AddressValidationSearchResult;

    brregIntegrationEnabled: boolean;
    brregError = false;
    brregSet = false;
    currentBrregCode: string;
    secondarySite: AddressValidationSearchResult;
    secondarySiteAddress: string;
    createSecondarySite = false;
    loadingBrregContent = false;
    currentSearchMethod: AddressSearchMethod;
    isGeonorgeEnabled = false;
    isPdokEnabled = false;

    constructor(
        public validationService: AddressValidationService,
        private mapService: AmcsLeafletMapService,
        private translateService: SharedTranslationsService,
        private appConfig: ApplicationConfigurationStore,
        private errorNotificationService: ErrorNotificationService,
        private brregService: BrregAddressService) {
        super();

        this.translateService.translations.pipe(take(1)).subscribe(translations => {
            this.translations = translations;
        });

        this.appConfig.configuration$.pipe(take(1))
            .subscribe((config: ApplicationConfiguration) => {
                this.isReadOnlyMode = config.generalConfiguration.isTabControlReadOnly;
                this.brregIntegrationEnabled = config.brregIntegrationEnabled;
            });

        this.searchTextSubject.pipe(takeUntil(this.unsubscribe)).subscribe((text: string) => {
            this.mapService.clearState();
            const country = this.getCountry();
            if (isTruthy(country)) {
                if (this.isGeonorgeEnabled && country.countryId !== AddressSearchCountryEnum.NOR) {
                    this.errorNotificationService.notifyError(this.translateService.getTranslation('addressMapper.invalidCountryAndProvider'));
                } else if (this.isPdokEnabled && country.iSOTwoChar !== 'NL') {
                    this.errorNotificationService.notifyError(this.translateService.getTranslation('addressMapper.invalidCountryAndProviderPdok'));
                } else {
                    this.validationService.requestAddressSearchResults(text, country.countryId);
                }
            }
        });
    }

    private translations: string[] = [];
    private addressFieldDisplayConfigurations: AddressFieldDisplayConfiguration[] = [];

    // Must pass in an existing form built from a AddressValidationForm class
    initialise<T extends AddressValidation>(form: AmcsFormGroup, model: T, enforceAddressSearch: boolean, countries: Country[],
        addressFieldDisplayConfigurations: AddressFieldDisplayConfiguration[], addressProvider: string = null) {
        this.brregService.currentSecondarySiteAddressContext$.pipe(take(1)).subscribe(secondaryAddress => {
            this.form = form;
            this.isGeonorgeEnabled = addressProvider === AddressSearchProviderEnum.Geonorge;
            this.isPdokEnabled = addressProvider === AddressSearchProviderEnum.Pdok;
            this.setSecondaryAddressContext(secondaryAddress);
            this.countries = countries;
            const country = this.getCountry();
            if (country) {
                this.countryLat = +country.latitude;
                this.countryLong = +country.longitude;
            }
            // RDM - Temp. disabling of this feature until we decide how to handle it for new customers / brreg /
            // editing service locations. Problem is 'formGroup' isn't just address information now it can also
            // contain template business types and other info.
            // if (enforceAddressSearch) {
            //     this.form.formGroup.disable();
            // }
            if (model) {
                this.mapGroupings = this.createMapMarkerFromLatLong(model);
                this.refreshMapState(this.mapGroupings);
            }

            this.setupBrregEventListener();
        });
        this.addressFieldDisplayConfigurations = addressFieldDisplayConfigurations;
        this.colSpan = addressFieldDisplayConfigurations.filter(c => c.isVisible).length > 7 ? 6 : 12;
        this.getFormControls();
    }

    onBrregGetAddressClick(brregType: number) {

        const currentBrregCode = this.form.formGroup.get('federalId').value.replace(/\s/g, '');

        if (!isTruthy(currentBrregCode)) {
            return;
        }

        this.loadingBrregContent = true;
        this.validationService.requestExternalAddressSearchResults(currentBrregCode, brregType);

        this.validationService.brregAddressSearchResult$.pipe(take(1)).subscribe(data => {

            if (isTruthy(data) && data.searchResults.length > 0) {
                this.brregError = false;
                // searchResults[0] = business address
                // searchResults[1] = postal address
                // If postal address is avaiable, we want to use that as the primary address, otherwise default to business
                const primaryAddress = isTruthy(data.searchResults[1]) ? data.searchResults[1] : data.searchResults[0];
                const secondaryAddress = isTruthy(data.searchResults[1]) ? data.searchResults[0] : null;
                const result = new AddressValidationSearchResult();
                if (primaryAddress.address1.includes(' ')) {
                    result.houseNumber = primaryAddress.address1.substr(primaryAddress.address1.lastIndexOf(' ') + 1);
                    result.address1 = primaryAddress.address1.substr(0, primaryAddress.address1.lastIndexOf(' '));
                } else {
                    result.address1 = primaryAddress.address1;
                }

                result.address2 = primaryAddress.address2;
                result.address3 = primaryAddress.address3;
                result.address4 = primaryAddress.address4;
                result.address5 = primaryAddress.address5;
                result.postcode = primaryAddress.postcode;
                result.federalId = currentBrregCode;
                result.latitude = null;
                result.longitude = null;

                this.brregService.setBrregAddressContext({
                    name: primaryAddress.siteName,
                    federalId: currentBrregCode,
                    sicCode: isTruthy(primaryAddress.sicCode) ? primaryAddress.sicCode.replace('.', '') : null,
                    municipalityNo: primaryAddress.municipalityNo
                });

                this.populateFormFromAddressValidationSearchResult(result, false);

                if (isTruthy(secondaryAddress)) {
                    this.form.formGroup.controls.secondarySiteAvailable.setValue(true);
                    this.secondarySite = new AddressValidationSearchResult();
                    this.secondarySite.siteName = secondaryAddress.siteName;
                    if (secondaryAddress.address1.includes(' ')) {
                        this.secondarySite.houseNumber = secondaryAddress.address1.substr(secondaryAddress.address1.lastIndexOf(' ') + 1);
                        this.secondarySite.address1 = secondaryAddress.address1.substr(0, secondaryAddress.address1.lastIndexOf(' '));
                    } else {
                        this.secondarySite.address1 = secondaryAddress.address1;
                    }

                    this.secondarySite.address2 = secondaryAddress.address2;
                    this.secondarySite.address3 = secondaryAddress.address3;
                    this.secondarySite.address4 = secondaryAddress.address4;
                    this.secondarySite.address5 = secondaryAddress.address5;
                    this.secondarySite.postcode = secondaryAddress.postcode;
                    this.secondarySite.federalId = currentBrregCode;
                    this.secondarySite.sicCode = isTruthy(secondaryAddress.sicCode) ? secondaryAddress.sicCode.replace('.', '') : null;
                    this.secondarySite.municipalityNo = secondaryAddress.municipalityNo;
                    this.secondarySiteAddress = this.secondarySite.address1;

                    if (this.secondarySiteAddress.length > 20) {
                        this.secondarySiteAddress = `${this.secondarySiteAddress.substring(0, 20)} ...`;
                    }

                    this.brregService.setCurrentSecondarySiteAddressContext(this.secondarySite);
                }

                this.brregSet = true;
            } else {
                this.resetAddressValidationSearchResult();
                this.brregError = true;
                this.brregSet = false;
            }

            this.loadingBrregContent = false;
        });
    }

    addressSelected(item: AddressValidationSearchResult) {
        if (!isTruthy(item)) {
            this.searchResult = null;
            return;
        }
        // there are two possibilities depending on the address search provider.
        // either the search result contains all the address fields OR it contains
        // an address id which we must then use to retrieve a search result object
        // populated with all the address fields.

        if (item.href) {
            this.validationService.address$.pipe(take(1)).subscribe((address: AddressValidationSearchResult) => {
                this.populateFormFromAddressValidationSearchResult(address);
            });

            const countryId = this.getCountryId();
            if (isTruthy(countryId)) {
                this.validationService.requestAddress(item.href, countryId);
            }
        } else {
            this.populateFormFromAddressValidationSearchResult(item);
        }
    }

    addressHighlighted(item: AddressValidationSearchResult) {
        this.refreshMapState(this.createMapMarkerFromSearchResult(item));
    }

    markerDragged(latLang: [number, number]) {
        const countryId = this.getCountryId();
        if (isTruthy(countryId)) {
            this.validationService.requestAddressReverseSearchResults(latLang[0], latLang[1], countryId);
        } else {
            this.validationService.requestAddressReverseSearchResults(latLang[0], latLang[1], null);
        }

        this.validationService.addressReverseSearchResults$.pipe(take(1)).subscribe((result: AddressValidationSearchResult[]) => {
            if (result && result[0]) {
                this.addressSelected(result[0]);
            } else {
                // No results so just update the coordinates on the input fields
                this.form.formGroup.controls.latitude.setValue(latLang[0]);
                this.form.formGroup.controls.longitude.setValue(latLang[1]);
            }
        });
    }

    refreshMapState(newGroupings: AmcsLeafletMapGrouping[]) {
        if (newGroupings) {
            this.mapGroupings = newGroupings;
        }
        this.mapService.setState(this.mapGroupings);
    }

    resetAddressValidationSearchResult(resetFederalId = false) {
        this.brregService.setSecondarySite(null);
        this.brregService.setBrregAddressContext(null);
        this.secondarySite = null;
        this.brregError = false;

        if (resetFederalId) {
            this.form.formGroup.controls.federalId.setValue(null);
        }
    }

    getFormControls() {
        const address5Value = this.form.formGroup.value.address5;
        this.buildDynamicControls(this.form.formGroup.controls.houseNumber, nameof<AddressValidationForm>('houseNumber'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.address1, nameof<AddressValidationForm>('address1'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.address2, nameof<AddressValidationForm>('address2'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.address3, nameof<AddressValidationForm>('address3'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.address4, nameof<AddressValidationForm>('address4'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.address5, nameof<AddressValidationForm>('address5'),
            AddressControlTypeEnum.select, 50, this.countries);

        this.buildDynamicControls(this.form.formGroup.controls.address6, nameof<AddressValidationForm>('address6'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.address7, nameof<AddressValidationForm>('address7'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.address8, nameof<AddressValidationForm>('address8'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.address9, nameof<AddressValidationForm>('address9'), AddressControlTypeEnum.input, 50);

        this.buildDynamicControls(this.form.formGroup.controls.postcode, nameof<AddressValidationForm>('postcode'), AddressControlTypeEnum.input, 50);

        this.dynamicControls.sort(function(a, b) {
            return a.position - b.position;
        });
        this.form.formGroup.controls.address5.setValue(address5Value);
    }

    buildDynamicControls(formControl: AbstractControl, name: string, type: AddressControlTypeEnum, maxLength: number, options?: any[]) {
        if (this.addressFieldDisplayConfigurations) {
            const config = this.addressFieldDisplayConfigurations.filter(c => c.name.toLowerCase() === name.toLowerCase());
            const control = new AddressDynamicControl();
            control.isDisplay = config[0]?.isVisible;
            control.isMandatory = config[0]?.isMandatory;
            control.name = name;
            control.options = options;
            const translationKey = 'addressMapper.' + AddressValidationFormService.getCamelCase(config[0]?.text);
            control.label = this.translations[translationKey];
            if (control.isMandatory) {
                formControl.setValidators([Validators.required]);
            } else {
                formControl.setValidators([Validators.nullValidator]);
            }
            control.type = type;
            control.position = config[0]?.order;
            control.maxLength = maxLength;
            control.control = formControl;
            this.dynamicControls = this.dynamicControls.filter(c => c.name !== name);
            this.dynamicControls.push(control);
        }
    }

    private static getCamelCase(text: string) {
        return text.charAt(0).toLowerCase() + text.substr(1);
    }

    private secondarySiteChanged(isChecked: boolean) {
        this.brregService.setSecondarySite(isChecked ? this.secondarySite : null);
    }

    private setSecondaryAddressContext(data: AddressValidationSearchResult) {
        if (!isNullOrUndefined(data)) {
            this.form.formGroup.controls.secondarySiteAvailable.setValue(!isNullOrUndefined(data));
            this.secondarySite = data;
            this.secondarySiteAddress = this.secondarySite.address1;

            if (this.secondarySiteAddress.length > 20) {
                this.secondarySiteAddress = `${this.secondarySiteAddress.substring(0, 20)} ...`;
            }

            this.secondarySiteChanged(this.form.formGroup.get('createSecondarySite').value as boolean);
        }
    }

    private createMapMarkerFromSearchResult(item: AddressValidationSearchResult): AmcsLeafletMapGrouping[] {
        const mapGrouping = new AmcsLeafletMapGrouping();
        const marker = new AmcsLeafletMapMarker(AmcsLeafletDataType.Default, [Number(item.latitude), Number(item.longitude)], item.formattedAddress,
            AddressValidatorMapConfig.markerIconOptions, !this.isReadOnlyMode);
        mapGrouping.markers.push(marker);
        return [mapGrouping];
    }

    private createMapMarkerFromLatLong(model: AddressValidation): AmcsLeafletMapGrouping[] {
        const mapGrouping = new AmcsLeafletMapGrouping();
        if (GpsCoordinateHelper.IsValidCoordinateFromStrings(model.latitude, model.longitude)) {
            const marker = new AmcsLeafletMapMarker(AmcsLeafletDataType.Default, [Number(model.latitude), Number(model.longitude)], model.fullAddress(),
                AddressValidatorMapConfig.markerIconOptions, !this.isReadOnlyMode);
            mapGrouping.markers.push(marker);
        }
        return [mapGrouping];
    }

    private populateFormFromAddressValidationSearchResult(item: AddressValidationSearchResult, setMapState = true) {
        this.form.formGroup.enable();
        if (item.houseNumber != null && item.houseNumber !== '') {
            this.form.formGroup.controls.houseNumber.setValue(item.houseNumber);
        } else {
            this.form.formGroup.controls.houseNumber.setValue('');
        }

        if (item.address1 != null && item.address1 !== '') {
            this.form.formGroup.controls.address1.setValue(item.address1);
        } else {
            this.form.formGroup.controls.address1.setValue('');
        }

        if (item.address2 != null && item.address2 !== '') {
            this.form.formGroup.controls.address2.setValue(item.address2);
        } else {
            this.form.formGroup.controls.address2.setValue('');
        }

        if (item.address3 != null && item.address3 !== '') {
            this.form.formGroup.controls.address3.setValue(item.address3);
        } else {
            this.form.formGroup.controls.address3.setValue('');
        }

        if (item.address4 != null && item.address4 !== '') {
            this.form.formGroup.controls.address4.setValue(item.address4);
        } else {
            this.form.formGroup.controls.address4.setValue('');
        }

        if (item.address6 != null && item.address6 !== '') {
            this.form.formGroup.controls.address6.setValue(item.address6);
        } else {
            this.form.formGroup.controls.address6.setValue('');
        }

        if (item.address7 != null && item.address7 !== '') {
            this.form.formGroup.controls.address7.setValue(item.address7);
        } else {
            this.form.formGroup.controls.address7.setValue('');
        }

        if (item.address8 != null && item.address8 !== '') {
            this.form.formGroup.controls.address8.setValue(item.address8);
        } else {
            this.form.formGroup.controls.address8.setValue('');
        }

        if (item.address9 !== null && item.address9 !== '') {
            this.form.formGroup.controls.address9.setValue(item.address9);
        } else {
            this.form.formGroup.controls.address9.setValue('');
        }

        if (item.postcode != null && item.postcode !== '') {
            this.form.formGroup.controls.postcode.setValue(item.postcode);
        } else {
            this.form.formGroup.controls.postcode.setValue('');
        }

        if (item.latitude != null && item.latitude !== '') {
            this.form.formGroup.controls.latitude.setValue(item.latitude);
        } else {
            this.form.formGroup.controls.latitude.setValue('');
        }

        if (item.longitude != null && item.longitude !== '') {
            this.form.formGroup.controls.longitude.setValue(item.longitude);
        } else {
            this.form.formGroup.controls.longitude.setValue('');
        }

        this.searchResult = item;

        if (isTruthy(item.address5)) {
            const country = this.countries.find(x => x.description === item.address5);
            if (isTruthy(country)) {
                this.form.formGroup.get('address5').setValue(country.description);
            }
        }

        if (setMapState) {
            this.refreshMapState(this.createMapMarkerFromSearchResult(item));
        }
    }

    private getCountry(): Country {
        return this.countries.find(x => x.description === this.form.formGroup.value.address5);
    }

    private getCountryId(): number {
        let countryId: number;
        const country = this.getCountry();
        if (isTruthy(country)) {
            countryId = country.countryId;
        }
        return countryId;
    }

    private setupBrregEventListener() {
        if (this.form && this.form.formGroup.get('federalId')) {
            this.form.formGroup.get('federalId').valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe(() => {
                if (this.brregSet) {
                    this.resetAddressValidationSearchResult();
                }
            });
        }

        if (this.form && this.form.formGroup.get('createSecondarySite')) {
            this.form.formGroup.get('createSecondarySite').valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe((val: boolean) => {
                this.secondarySiteChanged(val);
            });
        }
    }
}
