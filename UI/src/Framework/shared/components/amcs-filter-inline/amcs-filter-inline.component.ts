import { Component, ElementRef, Input, OnChanges, Output, QueryList, Renderer2, SimpleChanges, ViewChildren } from '@angular/core';
import { FilterDisplayMode } from '@core-module/models/filter/filter-display-mode.enum';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { Filter } from '@coremodels/filter/Filter';
import { FilterConfig } from '@coremodels/filter/FilterConfig';
import { FilterDefinition } from '@coremodels/filter/FilterDefinition';
import { AmcsFilterService } from '@coreservices/amcs-filter.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Observable } from 'rxjs';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-amcs-filter-inline',
  templateUrl: './amcs-filter-inline.component.html',
  styleUrls: ['./amcs-filter-inline.component.scss']
})
export class AmcsFilterInlineComponent extends AutomationLocatorDirective implements OnChanges {

  @Input() config: FilterConfig;
  @Input() tooltipPlacement = 'right';
  @Input() singleFilterViewWidth = 800;
  @Input() optionsWidth: number;
  @Input() clearAllText: string;
  @Output() filter$: Observable<Filter>;

  @ViewChildren('letter') letters: QueryList<ElementRef>;

  FormControlDisplay = FormControlDisplay;
  AmcsFilterDisplayMode = FilterDisplayMode;

  constructor(public filterService: AmcsFilterService, protected elRef: ElementRef, protected renderer: Renderer2, private translationService: SharedTranslationsService) {
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

  onFilterSingleFilterKeyup(filterDefinitionId: number, pattern: string) {
    this.filterService.filterSingleFilterOptions(filterDefinitionId, pattern);
  }

  clearAllOptionsInSelectedFilterDefinition() {
    this.filterService.clearFilters(this.filterService.singleFilterModeFilterDefinition.id);
    this.filterService.switchToMultiFilterMode();
  }

  scrollTo(letter: string) {
    setTimeout(() => {
      const letterElement: ElementRef = this.letters.find(x => x.nativeElement.id === letter);
      letterElement.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'start' });
    });
  }

  openFilterExpandedView(filterDefinitionId: number) {
    this.filterService.switchToSingleFilterMode(filterDefinitionId);
  }

  displayMultiFilterView(filterDef: FilterDefinition) {
    if (this.filterService.singleFilterModeFilterDefinition === null) {
      return true;
    }

    return this.filterService.singleFilterModeFilterDefinition.id !== filterDef.id;
  }

  displaySingleFilterView(filterDef: FilterDefinition) {
    return this.filterService.singleFilterModeFilterDefinition !== null && this.filterService.singleFilterModeFilterDefinition.id === filterDef.id;
  }
}
