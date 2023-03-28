import { Component, ElementRef, Input, OnChanges, Output, Renderer2, SimpleChanges } from '@angular/core';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { Filter } from '@coremodels/filter/Filter';
import { FilterConfig } from '@coremodels/filter/FilterConfig';
import { FilterDefinition } from '@coremodels/filter/FilterDefinition';
import { AmcsFilterService } from '@coreservices/amcs-filter.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-amcs-filter-mobile',
  templateUrl: './amcs-filter-mobile.component.html',
  styleUrls: ['./amcs-filter-mobile.component.scss']
})
export class AmcsFilterMobileComponent extends AutomationLocatorDirective implements OnChanges {

  @Input() config: FilterConfig;

  @Output() filter$: Observable<Filter>;

  FormControlDisplay = FormControlDisplay;

  constructor(public filterService: AmcsFilterService, protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
    this.filter$ = this.filterService.filter$;
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

  openFilterExpandedView(filterDefinitionId: number) {
    this.filterService.switchToSingleFilterMode(filterDefinitionId);
  }

  display(filterDef: FilterDefinition) {
    if (this.filterService.singleFilterModeFilterDefinition === null) {
      return true;
    }

    return this.filterService.singleFilterModeFilterDefinition.id === filterDef.id;
  }

  getOptionsToDisplay(filterDef: FilterDefinition) {
    if (!this.inSingleFilterMode()) {
      return filterDef.getOptionsToDisplay();
    }

    if (this.filterService.singleFilterModeFilterDefinition.id === filterDef.id) {
      return filterDef.getAllOptionsMatchingPattern();
    }

    return null;
  }

  getNumberOfOptionsNotDisplayed(filterDef: FilterDefinition) {
    if (!this.inSingleFilterMode()) {
      return filterDef.getNumberOfOptionsNotDisplayed();
    }

    return 0;
  }

  inSingleFilterMode() {
    return this.filterService.singleFilterModeFilterDefinition !== null;
  }
}
