import { Component, ElementRef, Input, OnChanges, Output, QueryList, Renderer2, SimpleChanges, ViewChildren } from '@angular/core';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { Filter } from '@coremodels/filter/Filter';
import { FilterConfig } from '@coremodels/filter/FilterConfig';
import { AmcsFilterService } from '@coreservices/amcs-filter.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Observable } from 'rxjs';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { take } from 'rxjs/operators';
import { FilterDisplayMode } from '@core-module/models/filter/filter-display-mode.enum';

@Component({
  selector: 'app-amcs-filter',
  templateUrl: './amcs-filter.component.html',
  styleUrls: ['./amcs-filter.component.scss']
})
export class AmcsFilterComponent extends AutomationLocatorDirective implements OnChanges {

  @Input() config: FilterConfig;

  /**
   * the applied filter list is always visible. the filters themselves are only visible when open is true.
   */
  @Input() open = false;

  @Input() clearAllText: string;
  @Input() tooltipPlacement = 'right';

  @Output() filter$: Observable<Filter>;

  @ViewChildren('letter') letters: QueryList<ElementRef>;

  FormControlDisplay = FormControlDisplay;
  AmcsFilterDisplayMode = FilterDisplayMode;

  constructor(public filterService: AmcsFilterService, protected elRef: ElementRef, protected renderer: Renderer2, public translationService: SharedTranslationsService) {
    super(elRef, renderer);
    this.filter$ = this.filterService.filter$;

    this.translationService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      this.clearAllText = translations['filter.clearAllText'];
    });
  }

  ngOnChanges(simpleChanges: SimpleChanges) {
    if (simpleChanges['config'] && simpleChanges['config'].currentValue) {
      this.filterService.configure(this.config);
    }
  }

  handleExpansion(filterDefId: number) {
    this.filterService.expand(filterDefId);
  }

  onFilterFilterKeyup(filterDefinitionId: number, pattern: string) {
    this.filterService.filterFilterOptions(filterDefinitionId, pattern);
  }

  clearAllOptionsInSelectedFilterDefinition() {
    this.filterService.clearFilters(this.filterService.singleFilterModeFilterDefinition.id);
    this.filterService.switchToMultiFilterMode();
  }

  switchToMultiFilterMode() {
    this.filterService.switchToMultiFilterMode();
  }

  switchToSingleFilterMode(filterDefId: number) {
    this.filterService.switchToSingleFilterMode(filterDefId);
  }

  scrollTo(letter: string) {
    setTimeout(() => {
      const letterElement: ElementRef = this.letters.find(x => x.nativeElement.id === letter);
      letterElement.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start', inline: 'start' });
    });
  }
}
