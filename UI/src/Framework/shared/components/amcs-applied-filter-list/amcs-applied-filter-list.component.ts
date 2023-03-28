import { Component, ElementRef, Input, Renderer2 } from '@angular/core';
import { AmcsFilterService } from '@coreservices/amcs-filter.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-amcs-applied-filter-list',
  templateUrl: './amcs-applied-filter-list.component.html',
  styleUrls: ['./amcs-applied-filter-list.component.scss']
})
export class AmcsAppliedFilterListComponent extends AutomationLocatorDirective {
  @Input() clearAllText: string;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, public filterService: AmcsFilterService, private translationService: SharedTranslationsService) {
    super(elRef, renderer);
    this.translationService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      this.clearAllText = translations['filter.clearAllText'];
    });
  }

}
