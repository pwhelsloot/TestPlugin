import { Injectable } from '@angular/core';
import { FormBuilder, FormControl, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { FrameworkAuthorisationClaimNames } from '@auth-module/models/framework-authorisation-claim-names.constants';
import { AuthorisationService } from '@auth-module/services/authorisation.service';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { BusinessTypeEnum } from '@core-module/models/lookups/business-type-enum.model';
import { BusinessTypeLookup } from '@core-module/models/lookups/business-type-lookup.model';
import { GlobalSearchKeywordQuickSearch } from '@coremodels/global-search/global-search-keyword-quick-search.model';
import { environment } from '@environments/environment';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { GlobalSearchModeEnum } from '@shared-module/models/global-search-mode.enum';
import { SearchKeywordEnum } from '@shared-module/models/search-keyword.enum';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { take, takeUntil } from 'rxjs/operators';
import { BaseService } from '../base.service';
import { GlobalKeywordSearchServiceData } from './data/global-keyword-search.service.data';
import { GlobalSearchServiceData } from './data/global-search.service.data';
import { GlobalKeywordSearchService } from './global-keyword-search.service';
import { GlobalSearchKeywordHelper } from './global-search-keyword-helper';
import { GlobalSearchService } from './global-search.service';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
@Injectable({ providedIn: 'root' })
export class GlobalSearchFormService extends BaseService {
    static providers = [GlobalSearchFormService, GlobalKeywordSearchService, GlobalKeywordSearchServiceData, GlobalSearchService, GlobalSearchServiceData, GlobalSearchKeywordHelper];

    form: AmcsFormGroup;
    keywordQuickSearchResults: GlobalSearchKeywordQuickSearch[] = [];
    businessTypes: BusinessTypeLookup[] = [];

    searchMode = GlobalSearchModeEnum.Standard;
    GlobalSearchModeEnum = GlobalSearchModeEnum;
    businessTypeId = 0;
    businessTypeLoading = true;
    businessTypeAll: string;
    showBusinessTypeDropdown = environment.applicationApiPrefix === 'erp/api';
    enableMunicipalLoB: boolean;

    constructor(
        public standardSearchService: GlobalSearchService,
        public keywordSearchService: GlobalKeywordSearchService,
        public keywordHelper: GlobalSearchKeywordHelper,
        private router: Router,
        private formBuilder: FormBuilder,
        private translationsService: SharedTranslationsService,
        private authorisationService: AuthorisationService) {
        super();
        this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
            this.businessTypeAll = translations['globalSearch.businessType.all'];
        });
        this.enableMunicipalLoB = this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.enableMunicipalLoB);
        this.setUpForm();
        this.setUpFormListeners();
        if (this.showBusinessTypeDropdown) {
            this.standardSearchService.requestBusinessTypes();
        }
    }

    clearInput(event: MouseEvent) {
        // RDM - Really don't like this but for some reason the return key is firing this first and then doFullSearch().
        // enter key means no screen axis supplied so bit of a hack but works
        if (event.screenY !== 0 && event.screenX !== 0) {
            this.resetSearch();
        }
    }

    resetSearch() {
        this.form.formGroup.reset();
        this.form.formGroup.get('businessTypeId').setValue(0);
        this.form.formGroup.get('searchKeyword').clearValidators();
        this.form.formGroup.get('searchKeyword').updateValueAndValidity();
        this.keywordHelper.filteredKeywords = [];
        this.standardSearchService.requestSearch(null);
        this.keywordSearchService.clearQuickSearchResults();
        this.searchMode = GlobalSearchModeEnum.Standard;
    }

    doFullSearch() {
        if (this.form.formGroup.valid) {
            const keyword: SearchKeywordEnum = this.form.formGroup.get('searchKeyword').value;
            if (keyword !== null) {
                this.keywordSearchService.clearQuickSearchResults();
                this.navigateKeyWordSearchResult(keyword);
            } else if (this.form.formGroup.controls.searchTerm.valid) {
                this.standardSearchService.requestSearch(this.form.formGroup.controls.searchTerm.value, this.businessTypeId);
                this.router.navigate([CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search]);
            }
        }
    }

    quickSearchResultSelected(selectedDescription: string, selectedId: number) {
        this.keywordSearchService.quickSearchResults$.pipe(take(1)).subscribe((results: GlobalSearchKeywordQuickSearch[]) => {
            const option: GlobalSearchKeywordQuickSearch = results.find(x => x.id === selectedId || x.barcodeUnique === selectedId.toString());
            if (isTruthy(option)) {
                this.keywordSearchService.quickSearchResultSelected(option);
                this.navigateKeyWordSearchResult(option.type);
            }
        });
    }

    navigateKeyWordSearchResult(type: SearchKeywordEnum) {
        if (this.validateNavigation(type)) {
            this.router.navigate([this.keywordHelper.getSearchRouteForKeyword(type, this.keywordSearchService.selectedId)]);
            this.keywordSearchService.selectedId = null;
        }
    }

    validateNavigation(type: SearchKeywordEnum): boolean {
        switch (type) {
            case SearchKeywordEnum.MAT:
                return this.keywordSearchService?.selectedId > 0;
            default:
                return true;
        }
    }

    setSearchKeyword(keyword: SearchKeywordEnum) {
        if (keyword !== null) {
            this.businessTypeId = 0;
            this.searchMode = GlobalSearchModeEnum.KeywordSelected;
            this.form.formGroup.get('searchKeyword').setValue(keyword);
            // Fresh search term now that we have a keyword.
            this.form.formGroup.get('searchTerm').setValue(null);
            this.keywordHelper.setKeywordPlaceholderAndDescription(keyword);
            this.keywordHelper.filteredKeywords = [];
        } else {
            // This means they've selected the 'no matching items' option so just reset form.
            this.form.formGroup.reset();
            this.form.formGroup.get('businessTypeId').setValue(0);
        }
    }

    onBackspaceClearSearchKeyword() {
        const term: string = this.form.formGroup.get('searchTerm').value;
        if (term === null || term.length <= 0) {
            // This doesnt clear standardSearchService as we've only hit backspace not clear
            this.form.formGroup.reset();
            this.form.formGroup.get('businessTypeId').setValue(0);
            this.form.formGroup.get('searchKeyword').clearValidators();
            this.form.formGroup.get('searchKeyword').updateValueAndValidity();
            this.keywordHelper.filteredKeywords = [];
            this.keywordSearchService.clearQuickSearchResults();
            this.searchMode = GlobalSearchModeEnum.Standard;
        }
    }

    private setUpForm() {
        this.standardSearchService.businessTypes$.pipe(takeUntil(this.unsubscribe)).subscribe((results: BusinessTypeLookup[]) => {
            const businessType = new BusinessTypeLookup();
            businessType.id = 0;
            businessType.description = this.businessTypeAll;
            this.businessTypes.push(businessType);
            const index = results.findIndex(item => item.id === BusinessTypeEnum.MunicipalOfferings);
            if (!this.enableMunicipalLoB && index != null) {
              results.splice(index, 1);
            }
            this.businessTypes = this.businessTypes.concat(results);
            this.form.formGroup.get('businessTypeId').setValue(0);
            this.businessTypeLoading = false;
        });

        this.form = new AmcsFormGroup(this.formBuilder.group({
            searchTerm: new FormControl(null, [Validators.required, Validators.minLength(3)]),
            searchKeyword: new FormControl(null),
            businessTypeId: new FormControl(0)
        }));
    }

    private setUpFormListeners() {
        this.keywordSearchService.quickSearchResults$.pipe(takeUntil(this.unsubscribe)).subscribe((results: GlobalSearchKeywordQuickSearch[]) => {
            this.keywordQuickSearchResults = isTruthy(results) && results instanceof Array ? results : [];
        });

        this.form.formGroup.get('searchTerm').valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe((term: string) => {
            if (this.form.formGroup.get('searchKeyword').value === null) {
                // Listen for search term changes, if keyword hasn't been selected and @ is typed then make keyword required.
                if (term !== null && term.length === 1 && term === '@') {
                    this.form.formGroup.get('searchKeyword').setValidators([Validators.required]);
                    this.form.formGroup.get('searchKeyword').updateValueAndValidity();
                } else if (term === null || term.length <= 0) {
                    this.searchMode = GlobalSearchModeEnum.Standard;
                    this.form.formGroup.get('businessTypeId').setValue(0);
                    this.form.formGroup.get('searchKeyword').clearValidators();
                    this.form.formGroup.get('searchKeyword').updateValueAndValidity();
                }
                if (term !== null && !this.attemptToAutoSelectKeyword(term)) {
                    this.keywordHelper.filterKeywords(term);
                }
            } else {
                this.updateQuickSearchResults(term);
            }
        });

        this.form.formGroup.get('businessTypeId').valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe((id: number) => {
            this.businessTypeId = id;
            this.doFullSearch();
        });
    }

    private updateQuickSearchResults(term: string) {
        // Need this timeout as formGroup.valid won't be properly set yet
        setTimeout(() => {
            if (this.form.formGroup.valid) {
                this.keywordSearchService.requestQuickSearch(term, this.form.formGroup.get('searchKeyword').value);
            } else if (this.keywordQuickSearchResults.length > 0) {
                // form not valid then clear results
                this.keywordSearchService.clearQuickSearchResults();
            }
        }, 0);
    }

    // In this case the users typed '@CON ' without actually clicking/selecting via auto-complete we want to manually set the keyword
    // if we have an exact term match with a space
    private attemptToAutoSelectKeyword(term: string): boolean {
        if (this.keywordHelper.filteredKeywords.length === 1 && term.length > 0) {
            const keyword: { description: string; type: SearchKeywordEnum; class: string } = this.keywordHelper.filteredKeywords[0];
            if (keyword.type !== null && term.slice(1).toLowerCase() === keyword.description.toLowerCase() + ' ') {
                this.setSearchKeyword(keyword.type);
                return true;
            }
        }
        return false;
    }
}
