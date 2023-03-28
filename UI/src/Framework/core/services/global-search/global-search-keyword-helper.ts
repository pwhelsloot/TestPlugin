import { Injectable } from '@angular/core';
import { FrameworkAuthorisationClaimNames } from '@auth-module/models/framework-authorisation-claim-names.constants';
import { CoreAppRoutes } from '@core-module/config/routes/core-app-routes.constants';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { CoreTranslationsService } from '@core-module/services/translation/core-translation.service';
import { SearchKeywordEnum } from '@shared-module/models/search-keyword.enum';
import { take } from 'rxjs/operators';
import { AuthorisationService } from '../../../auth/services/authorisation.service';
import { HeaderComponentService } from '../header/header-component.service';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
@Injectable({ providedIn: 'root' })
export class GlobalSearchKeywordHelper {
    filteredKeywords: { description: string; type: SearchKeywordEnum; class: string }[] = [];
    keywordPlaceholder: string;
    keywordDescription: string;
    keywordCode: string;

    SearchKeywordEnum = SearchKeywordEnum;

    constructor(private translationsService: CoreTranslationsService, private authorisationService: AuthorisationService,
        public headerComponentService: HeaderComponentService) {
        this.loadKeywords();
    }

    private keywords: { keywordCode: string; description: string; type: SearchKeywordEnum; class: string }[];
    private AllKeywords: { keywordCode: string; description: string; type: SearchKeywordEnum; class: string }[];
    private keywordPlaceholders: string[] = [];

    setKeywordPlaceholderAndDescription(type: SearchKeywordEnum) {
        let keyword = null;
        switch (type) {
            case SearchKeywordEnum.CON:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.CON);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['con'];
                this.keywordCode = 'CON';
                break;

            case SearchKeywordEnum.INV:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.INV);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['inv'];
                this.keywordCode = 'INV';
                break;

            case SearchKeywordEnum.PPC:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.PPC);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['ppc'];
                break;

            case SearchKeywordEnum.FED:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.FED);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['fed'];
                break;

            case SearchKeywordEnum.SON:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.SON);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['son'];
                break;

            case SearchKeywordEnum.PON:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.PON);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['pon'];
                break;

            case SearchKeywordEnum.TNA:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.TNA);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['tna'];
                break;

            case SearchKeywordEnum.RPT:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.RPT);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['rpt'];
                break;

            case SearchKeywordEnum.WDN:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.WDN);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['wdn'];
                break;

            case SearchKeywordEnum.STN:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.STN);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['stn'];
                break;

            case SearchKeywordEnum.BCN:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.BCN);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['bcn'];
                break;

            case SearchKeywordEnum.ACC:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.ACC);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['acc'];
                break;

            case SearchKeywordEnum.MAT:
                keyword = this.keywords.find(x => x.type === SearchKeywordEnum.MAT);
                this.keywordDescription = keyword.description;
                this.keywordCode = keyword.keywordCode;
                this.keywordPlaceholder = this.keywordPlaceholders['mat'];
                break;
        }
    }

    filterKeywords(term: string) {
        this.keywords = this.AllKeywords;
        if (!this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.materialManagementFeature)) {
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.PON);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.SON);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.BCN);
        }
        if (this.headerComponentService.headerMenuService.activeItem == null || this.headerComponentService.headerMenuService.activeItem.text !== 'Materials') {
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.BCN);
        }
        if (!this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.canAccessWasteDeclarationNumberSearch)) {
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.WDN);
        }
        if (!this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.canAccessMatrikkelnummerSearch)) {
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.MAT);
        }
        if (!this.authorisationService.hasAuthorisation(FrameworkAuthorisationClaimNames.customerFeature)) {
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.ACC);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.CON);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.FED);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.INV);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.PON);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.PPC);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.SON);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.STN);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.TNA);
            this.keywords = this.keywords.filter(keyword => keyword.type !== SearchKeywordEnum.WDN);
        }
        // Remove the @ and filter items
        if (term !== null && term.length > 0 && term[0] === '@') {
            const termLower = term.slice(1).toLowerCase();
            this.filteredKeywords = termLower.length === 0 ? this.keywords.slice() : this.keywords.filter(keyWord => keyWord.description.toLowerCase().includes(termLower));
            // Remove the 'no results' option
            this.filteredKeywords = this.filteredKeywords.filter(keyWord => keyWord.type !== null);
            // If no results add the 'no results' option
            if (this.filteredKeywords.length <= 0) {
                this.filteredKeywords = this.keywords.filter(keyWord => keyWord.type === null);
            }
        } else {
            this.filteredKeywords = [];
        }
        // Sort results alphabetically
        this.filteredKeywords = this.filteredKeywords.sort((a, b) => {
            const aDescription = isTruthy(a.description) ? a.description : '';
            const bDescription = isTruthy(b.description) ? b.description : '';
            return aDescription.localeCompare(bDescription);
        });
    }

    getSearchRouteForKeyword(type: SearchKeywordEnum, selectedId: number): string {
        switch (type) {
            case SearchKeywordEnum.CON:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchcontainer;

            case SearchKeywordEnum.INV:
                // Should obviously be different in future
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchinvoicenumber;

            case SearchKeywordEnum.PPC:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchprepaycard;

            case SearchKeywordEnum.FED:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchfederalid;

            case SearchKeywordEnum.SON:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchsalesordernumber;

            case SearchKeywordEnum.PON:
                if (isTruthy(this.headerComponentService.headerMenuService.activeItem) && this.headerComponentService.headerMenuService.activeItem.text === 'Materials') {
                    return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchpurchaseordernumber;
                } else {
                    return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchpurchaseorder;
                }

            case SearchKeywordEnum.TNA:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchtradingname;

            case SearchKeywordEnum.RPT:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchreport;

            case SearchKeywordEnum.WDN:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchwastedeclarationnumber;

            case SearchKeywordEnum.STN:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchweighingticket;

            case SearchKeywordEnum.BCN:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searchbarcode;

            case SearchKeywordEnum.ACC:
                return CoreAppRoutes.homeModule + '/' + CoreAppRoutes.search + '/' + CoreAppRoutes.searcharaccountcode;

            case SearchKeywordEnum.MAT:
                return CoreAppRoutes.customerModule + '/' + selectedId?.toString() + '/' + CoreAppRoutes.dashboard;
        }
    }

    private loadKeywords() {
        this.keywords = [];
        this.translationsService.translations.pipe(take(1)).subscribe((translations: string[]) => {
            for (const keyword in SearchKeywordEnum) {
                if (!Number(keyword)) {
                    const type = this.getEnumFromString(keyword);
                    this.keywords.push(this.createKeywordOption(type, translations));
                }
            }
            // Always add a 'not found' option.
            this.keywords.push(
                { keywordCode: null, description: translations['globalsearch.searchkeyword.noresults'], type: null, class: 'mat-option' }
            );
            this.AllKeywords = this.keywords;
            this.loadKeywordPlaceholders(translations);
        });
    }

    // These are custom placeholders for the search term bar depending on what keyword is selected.
    private loadKeywordPlaceholders(translations: string[]) {
        this.keywordPlaceholders['con'] = translations['globalsearch.searchkeyword.placeholder.con'];
        this.keywordPlaceholders['inv'] = translations['globalsearch.searchkeyword.placeholder.inv'];
        this.keywordPlaceholders['ppc'] = translations['globalsearch.searchkeyword.placeholder.ppc'];
        this.keywordPlaceholders['fed'] = translations['globalsearch.searchkeyword.placeholder.fed'];
        this.keywordPlaceholders['son'] = translations['globalsearch.searchkeyword.placeholder.son'];
        this.keywordPlaceholders['pon'] = translations['globalsearch.searchkeyword.placeholder.pon'];
        this.keywordPlaceholders['tna'] = translations['globalsearch.searchkeyword.placeholder.tna'];
        this.keywordPlaceholders['rpt'] = translations['globalsearch.searchkeyword.placeholder.rpt'];
        this.keywordPlaceholders['wdn'] = translations['globalsearch.searchkeyword.placeholder.wdn'];
        this.keywordPlaceholders['stn'] = translations['globalsearch.searchkeyword.placeholder.stn'];
        this.keywordPlaceholders['bcn'] = translations['globalsearch.searchkeyword.placeholder.bcn'];
        this.keywordPlaceholders['acc'] = translations['globalsearch.searchkeyword.placeholder.acc'];
        this.keywordPlaceholders['mat'] = translations['globalsearch.searchkeyword.placeholder.mat'];
    }

    private createKeywordOption(type: SearchKeywordEnum, translations: string[]): { keywordCode: string; description: string; type: SearchKeywordEnum; class: string } {
        switch (type) {
            case SearchKeywordEnum.CON:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.con'],
                    type,
                    description: translations['globalsearch.searchkeyword.con'],
                    class: 'mat-option con'
                };

            case SearchKeywordEnum.INV:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.inv'],
                    type,
                    description: translations['globalsearch.searchkeyword.inv'],
                    class: 'mat-option inv'
                };

            case SearchKeywordEnum.PPC:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.ppc'],
                    type,
                    description: translations['globalsearch.searchkeyword.ppc'],
                    class: 'mat-option ppc'
                };

            case SearchKeywordEnum.FED:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.fed'],
                    type,
                    description: translations['globalsearch.searchkeyword.fed'],
                    class: 'mat-option fed'
                };

            case SearchKeywordEnum.SON:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.son'],
                    type,
                    description: translations['globalsearch.searchkeyword.son'],
                    class: 'mat-option son'
                };

            case SearchKeywordEnum.PON:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.pon'],
                    type,
                    description: translations['globalsearch.searchkeyword.pon'],
                    class: 'mat-option pon'
                };

            case SearchKeywordEnum.TNA:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.tna'],
                    type,
                    description: translations['globalsearch.searchkeyword.tna'],
                    class: 'mat-option tna'
                };

            case SearchKeywordEnum.RPT:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.rpt'],
                    type,
                    description: translations['globalsearch.searchkeyword.rpt'],
                    class: 'mat-option rpt'
                };

            case SearchKeywordEnum.WDN:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.wdn'],
                    type,
                    description: translations['globalsearch.searchkeyword.wdn'],
                    class: 'mat-option wdn'
                };

            case SearchKeywordEnum.STN:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.stn'],
                    type,
                    description: translations['globalsearch.searchkeyword.stn'],
                    class: 'mat-option stn'
                };

            case SearchKeywordEnum.BCN:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.bcn'],
                    type,
                    description: translations['globalsearch.searchkeyword.bcn'],
                    class: 'mat-option bcn'
                };

            case SearchKeywordEnum.ACC:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.acc'],
                    type,
                    description: translations['globalsearch.searchkeyword.acc'],
                    class: 'mat-option acc'
                };

            case SearchKeywordEnum.MAT:
                return {
                    keywordCode: translations['globalsearch.searchkeyword.key.mat'],
                    type,
                    description: translations['globalsearch.searchkeyword.mat'],
                    class: 'mat-option fed'
                };

            default:
                throw new Error('No translation found for SearchKeywordEnum');
        }
    }

    private getEnumFromString(type: string): SearchKeywordEnum {
        switch (type) {
            case 'CON':
                return SearchKeywordEnum.CON;

            case 'INV':
                return SearchKeywordEnum.INV;

            case 'PPC':
                return SearchKeywordEnum.PPC;

            case 'FED':
                return SearchKeywordEnum.FED;

            case 'SON':
                return SearchKeywordEnum.SON;

            case 'PON':
                return SearchKeywordEnum.PON;

            case 'TNA':
                return SearchKeywordEnum.TNA;

            case 'RPT':
                return SearchKeywordEnum.RPT;

            case 'WDN':
                return SearchKeywordEnum.WDN;

            case 'STN':
                return SearchKeywordEnum.STN;

            case 'BCN':
                return SearchKeywordEnum.BCN;

            case 'ACC':
                return SearchKeywordEnum.ACC;

            case 'MAT':
                return SearchKeywordEnum.MAT;

            default:
                throw new Error('SearchKeywordEnum not recognised, value was ' + type);
        }
    }
}
