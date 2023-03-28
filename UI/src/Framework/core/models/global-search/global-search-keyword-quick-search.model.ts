
import { alias } from '@coreconfig/api-dto-alias.function';
import { SearchKeywordEnum } from '@shared-module/models/search-keyword.enum';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
export class GlobalSearchKeywordQuickSearch {
    @alias('Id')
    id: number;

    @alias('Type')
    type: SearchKeywordEnum;

    @alias('Description')
    description: string;

    @alias('CustomerSiteId')
    customerSiteId: number;

    @alias('BarcodeUnique')
    barcodeUnique: string;
}
