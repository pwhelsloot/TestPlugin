import { Injectable } from '@angular/core';
import { MatPaginatorIntl } from '@angular/material/paginator';
import { take } from 'rxjs/operators';
import { SharedTranslationsService } from './services/shared-translations.service';

@Injectable()
export class CustomPaginator extends MatPaginatorIntl {

      nextPageLabel: string;
      previousPageLabel: string;

      constructor(private translationService: SharedTranslationsService) {
          super();
          this.getTranslations();
      }

      getTranslations() {
        this.translationService.translations.pipe(take(1)).subscribe((translations: string[]) => {
            this.nextPageLabel = translations['pagination.nextPage'];
            this.previousPageLabel = translations['pagination.previousPage'];
          });
    }
}
