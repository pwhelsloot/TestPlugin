
import { Injectable } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FilterDisplayMode } from '@core-module/models/filter/filter-display-mode.enum';
import { Filter } from '@coremodels/filter/Filter';
import { FilterConfig } from '@coremodels/filter/FilterConfig';
import { FilterDefinition } from '@coremodels/filter/FilterDefinition';
import { FilterDefinitionSelected } from '@coremodels/filter/FilterDefinitionSelected';
import { FilterOption } from '@coremodels/filter/FilterOption';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { BehaviorSubject, Observable } from 'rxjs';
import { combineLatest, debounceTime, distinctUntilChanged, filter, map, take } from 'rxjs/operators';

@Injectable()
export class AmcsFilterService {

  // a stream of objects which represent the current UI state
  filterDefinitions$: Observable<FilterDefinition[]>;

  // a stream which provides a flat list of selected filters.
  selectedFilterOptions$: Observable<FilterOption[]>;

  // a stream which represents all selected filter options.
  filter$: Observable<Filter>;

  // the current filter definition in single-filter mode.
  singleFilterModeFilterDefinition: FilterDefinition;

  filterConfig: FilterConfig;
  translations: string[];

  constructor(public translationService: SharedTranslationsService) {
    this.filterDefinitions = new BehaviorSubject<FilterDefinition[]>([]);
    this.filterDefinitions$ = this.filterDefinitions.asObservable();
    this.singleFilterModeFilterDefinition = null;
    this.setupSelectedFilterOptionsStream();
    this.setupFilterStream();

    this.translationService.translations.pipe(take(1)).subscribe((translations: string[]) => {
      this.translations = translations;
    });
  }

  private filterDefinitions: BehaviorSubject<FilterDefinition[]>;

  // a stream of the filter state - either multi-filter (false) or single filter (true)
  private singleFilterMode$ = new BehaviorSubject<boolean>(false);

  configure(config: FilterConfig) {
    this.filterConfig = config;
    const filterDefs: FilterDefinition[] = config.filterDefinitions.map(def => def.clone());
    filterDefs.forEach(def => {
      def.numberOfOptionsToDisplay = config.numberOfOptionsToDisplay;
      def.filterOptions.forEach(x => x.filterDefinitionId = def.id);
      this.applyPatternToFilterOptions(def.filterOptions, def.pattern);
      if (!def.keepOriginalSorting) {
        this.applyDefaultSortToFilterOptions(def.filterOptions);
      }
      if (def.allowSelectAll && !def.filterOptions.find(x => x.id === 0)) {
        const allOption = new FilterOption(0, this.translations['filter.all'], true, def.id);
        def.filterOptions = [allOption, ...def.filterOptions];
      } else if (!def.allowSelectAll && def.filterOptions.find(x => x.id === 0)) {
        def.filterOptions.splice(def.filterOptions.findIndex(x => x.id === 0), 1);
      }
    });
    this.filterDefinitions.next(filterDefs);
  }

  expand(filterDefinitionId: number) {
    const filterDefs = this.filterDefinitions.getValue();
    const filterDef = filterDefs.find(x => x.id === filterDefinitionId);
    filterDef.expanded = !filterDef.expanded;
    this.filterDefinitions.next(filterDefs);
  }

  selectFilterOption(select: boolean, filterDefinitionId: number, filterOptionId: number) {
    const filterDefs = this.filterDefinitions.getValue();
    const filterDef = filterDefs.find(x => x.id === filterDefinitionId);
    if (filterDef.filterDisplayMode === FilterDisplayMode.RadioButton) {
      filterDef.filterOptions.forEach(x => x.selected = false);
    }
    // To handle the all option in filter segment
    if (filterDef.allowSelectAll && filterOptionId === 0) {
      filterDef.filterOptions.forEach(x => x.selected = select);
    }
    filterDef.filterOptions.find(x => x.id === filterOptionId).selected = select;
    if (filterDef.allowSelectAll && !select && filterDef.filterOptions.filter(x => x.id === 0).length > 0) {
      filterDef.filterOptions.find(x => x.id === 0).selected = false;
    }
    this.filterDefinitions.next(filterDefs);
  }

  clearFilters(filterDefinitionId: number) {
    const filterDefs = this.filterDefinitions.getValue();
    const filterDef = filterDefs.find(x => x.id === filterDefinitionId);
    filterDef.filterOptions.forEach(x => { x.selected = false; });
    filterDef.pattern = '';
    this.applyPatternToFilterOptions(filterDef.filterOptions, filterDef.pattern);
    this.filterDefinitions.next(filterDefs);
  }

  clearAllFilters() {
    const filterDefs = this.filterDefinitions.getValue();
    filterDefs.forEach(filterDef => {
      filterDef.filterOptions.forEach(opt => opt.selected = false);
      filterDef.pattern = '';
      this.applyPatternToFilterOptions(filterDef.filterOptions, filterDef.pattern);
    });
    // clear as well the singleFilterMode filters
    this.singleFilterModeFilterDefinition = null;
    this.singleFilterMode$.next(false);
    this.filterDefinitions.next(filterDefs);
  }

  filterFilterOptions(filterDefinitionId: number, pattern: string) {
    const filterDefs = this.filterDefinitions.getValue();
    const filterDef = filterDefs.find(x => x.id === filterDefinitionId);
    filterDef.pattern = pattern;
    this.applyPatternToFilterOptions(filterDef.filterOptions, pattern);
    this.filterDefinitions.next(filterDefs);
  }

  filterSingleFilterOptions(filterDefinitionId: number, pattern: string) {
    const filterDefs = this.filterDefinitions.getValue();
    const filterDef = filterDefs.find(x => x.id === filterDefinitionId);
    this.singleFilterModeFilterDefinition = filterDef;
    this.applyPatternToFilterOptions(filterDef.filterOptions, pattern);
    this.singleFilterModeFilterDefinition.setDistinctFilterOptionStartCharacters();
    this.singleFilterModeFilterDefinition.setAllOptionsMatchingPatternGroupedByFirstCharacter();
    this.singleFilterMode$.next(true);
  }

  switchToSingleFilterMode(filterDefinitionId: number) {
    const filterDefs = this.filterDefinitions.getValue();
    const filterDef = filterDefs.find(x => x.id === filterDefinitionId);
    this.singleFilterModeFilterDefinition = filterDef;
    this.singleFilterModeFilterDefinition.setDistinctFilterOptionStartCharacters();
    this.singleFilterModeFilterDefinition.setAllOptionsMatchingPatternGroupedByFirstCharacter();
    this.singleFilterMode$.next(true);
  }

  switchToMultiFilterMode() {
    this.singleFilterModeFilterDefinition = null;
    this.singleFilterMode$.next(false);
  }

  private applyDefaultSortToFilterOptions(filterOptions: FilterOption[]) {
    // default sort: alphabetical ascending
    filterOptions.sort((a, b) => {
      const aDescription = isTruthy(a.description) ? a.description.trim().toLocaleUpperCase() : '';
      const bDescription = isTruthy(b.description) ? b.description.trim().toLocaleUpperCase() : '';

      if (aDescription < bDescription) {
        return -1;
      }
      if (aDescription > bDescription) {
        return 1;
      }
      return 0;
    });
  }

  private applyPatternToFilterOptions(filterOptions: FilterOption[], pattern: string) {
    // escapes all regex special characters
    const regexPattern = pattern.replace(/[-[\]{}()*+?.,\\^$|#\s]/g, '\\$&');
    const regex = new RegExp(regexPattern, 'i');
    filterOptions.forEach(x => x.matchesPattern = isTruthy(x.description) && x.description.match(regex) !== null);
  }

  // an alternative view of the current filter state as a flat list of selected filter options.
  // only emits when we are in multi-filter mode.
  private setupSelectedFilterOptionsStream() {
    this.selectedFilterOptions$ = this.filterDefinitions$.pipe(
      combineLatest(this.singleFilterMode$),
      filter(stuff => !stuff[1]),
      map(stuff => {
        const filterDefs = stuff[0];
        const options: FilterOption[][] = filterDefs.map(filterDef => filterDef.filterOptions.filter(x => x.selected));
        for (let a = 0; a < options.length; a++) {
          if (options[a].find(x => x.id === 0)) {
            options[a].forEach(y => y.hasAllSelected = true);
          }
        }
        const flattened = [].concat.apply([], options);
        return flattened;
      })
    );
  }

  // the filter is just the set of selected filter option ids for each filter definition.
  // only emits in multi-filter mode.
  private setupFilterStream() {
    this.filter$ = this.filterDefinitions$.pipe(
      combineLatest(this.singleFilterMode$),
      filter(stuff => !stuff[1]),
      debounceTime(100),
      map(stuff => {
        const filterDefs = stuff[0];
        return new Filter(filterDefs.map(filterDef => new FilterDefinitionSelected(filterDef.id, filterDef.filterOptions.filter(option => option.selected).map(option => option.id))));
      }),
      distinctUntilChanged((a: Filter, b: Filter) => a.equals(b))
    );
  }
}
