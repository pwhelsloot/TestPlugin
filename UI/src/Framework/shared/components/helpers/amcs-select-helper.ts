import { ElementRef } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { GlossaryService } from '@core-module/services/glossary/glossary.service';
import { IGroupedLookupItem } from '@coremodels/lookups/grouped-lookup-item.interface';
import { ILookupItem } from '@coremodels/lookups/lookup-item.interface';

export class AmcsSelectHelper {

    /**
    * @deprecated Only used by deprecated components due to legacy complexities
    */
    static applyGlossaryTranslations(elRef: ElementRef, glossaryService: GlossaryService) {
        const allOptions = elRef.nativeElement.querySelectorAll('option');
        if (!allOptions) {
            return;
        }
        allOptions.forEach(option => {
            if (option.innerText) {
                option.innerText = glossaryService.getGlossaryTranslation(option.innerText.trim());
            }
        });
    }

    static doDeprecatedSort(selectElement: HTMLSelectElement) {
        const optionsArray = [];
        const sortedOptions = selectElement.options;
        for (let i = 0; i < sortedOptions.length; i++) {
            optionsArray.push(sortedOptions[i]);
        }
        optionsArray.sort((a, b) => {
            if (a.text < b.text) {
                return -1;
            }
            if (a.text > b.text) {
                return 1;
            }
            return 0;
        });
        for (let i = 0; i < selectElement.length; i++) {
            selectElement.options[i] = null;
        }
        optionsArray.forEach((option) => {
            selectElement.add(option);
        });
    }

    static doSort(options: ILookupItem[] | IGroupedLookupItem[]) {
        if (!this.areOptionsGrouped(options) && options) {
            options = options.sort((a, b) => {
                const aDescription = isTruthy(a.description) ? a.description : '';
                const bDescription = isTruthy(b.description) ? b.description : '';
                return aDescription.localeCompare(bDescription);
            });
        } else if (this.areOptionsGrouped(options) && options) {
            options.forEach((data) => {
                data.items.sort((a, b) => {
                    const aDescription = isTruthy(a.description) ? a.description : '';
                    const bDescription = isTruthy(b.description) ? b.description : '';
                    return aDescription.localeCompare(bDescription);
                });
            });
        }
    }

    static areOptionsGrouped(options: ILookupItem[] | IGroupedLookupItem[]): options is IGroupedLookupItem[] {
        return (
            (options as IGroupedLookupItem[]) &&
            (options as IGroupedLookupItem[]).length > 0 &&
            (options as IGroupedLookupItem[])[0].key !== undefined
        );
    }
}
