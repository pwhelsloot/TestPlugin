import { Injectable } from '@angular/core';
import { GlossaryLanguage } from '@core-module/models/glossary/glossary-language.model';
import { BaseService } from '@core-module/services/base.service';
import { GlossaryEntry } from '@core-module/services/glossary/glossary-entry.interface';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { ApiFilter } from '@core-module/services/service-structure/api-filter';
import { ApiFilters } from '@core-module/services/service-structure/api-filter-builder';
import { TranslateSettingService } from '@translate/translate-setting.service';
import { Observable, Subject } from 'rxjs';
import { filter, switchMap, takeUntil } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class GlossaryService extends BaseService {
  glossaryUpdated$: Observable<void>;

  constructor(private readonly businessService: ApiBusinessService, private readonly localSettings: TranslateSettingService) {
    super();
    this.glossaryUpdated$ = this.glossaryUpdatedSubject.asObservable();
    this.setupLanguageChangeStream();
  }

  private readonly glossaryUpdatedSubject = new Subject<void>();
  private glossaryMap: Map<string, GlossaryEntry>;
  private glossaryDisabled = false;

  disableGlossary() {
    this.glossaryDisabled = true;
  }

  enableGlossary() {
    this.glossaryDisabled = false;
  }

  getGlossaryTranslation(originalText: string): string {
    if (!originalText || typeof originalText !== 'string' || this.glossaryDisabled) {
      return originalText;
    }
    const glossaryEntry = this.glossaryMap?.get(originalText.toLowerCase());
    return glossaryEntry?.translation ?? originalText;
  }

  /**
   * Get the original text from a glossary translated value
   * @param translation the glossary translated value
   * @returns the original text or translation if not found
   */
  getOriginalText(translation: string): string {
    for (let [_key, value] of this.glossaryMap.entries()) {
      if (value.translation.toLowerCase() === translation.toLowerCase()) {
        return value.original;
      }
    }
    return translation;
  }

  private setupLanguageChangeStream() {
    this.localSettings.selectedLanguage
      .pipe(
        takeUntil(this.unsubscribe),
        filter((languageCode: string) => languageCode && languageCode !== ''),
        switchMap((languageCode: string) => {
          return this.businessService.getArray<GlossaryLanguage>(
            new ApiFilters().add(ApiFilter.equal('LanguageCode', languageCode)).build(),
            GlossaryLanguage,
            { isCoreRequest: true, suppressErrorModal: true } // Endpoint may not always be available so don't throw error on 404
          );
        })
      )
      .subscribe((results: GlossaryLanguage[]) => {
        this.createGlossaryMap(results);
        this.glossaryUpdatedSubject.next();
      });
  }

  private createGlossaryMap(glossaryEntries: GlossaryLanguage[]) {
    this.glossaryMap = new Map<string, GlossaryEntry>();
    if (glossaryEntries && glossaryEntries.length > 0) {
      glossaryEntries.forEach((glossaryEntry) => {
        this.glossaryMap.set(glossaryEntry.original.toLowerCase(), {
          original: glossaryEntry.original,
          translation: glossaryEntry.translated,
        });
      });
    }
  }
}
