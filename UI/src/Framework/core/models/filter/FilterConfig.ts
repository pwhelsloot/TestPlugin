import { FilterDefinition } from '@coremodels/filter/FilterDefinition';
import { FilterLayoutMode } from './filter-layout-mode.enum';

export class FilterConfig {
  filterDefinitions: FilterDefinition[] = [];

  numberOfOptionsToDisplay: number;

  filterLayoutMode: FilterLayoutMode;

  constructor(filterDefinitions: FilterDefinition[], numberOfOptionsToDisplay?: number, filterLayoutMode?: FilterLayoutMode) {
    this.filterDefinitions = filterDefinitions;
    this.numberOfOptionsToDisplay = numberOfOptionsToDisplay || 4;
    this.filterLayoutMode = filterLayoutMode || FilterLayoutMode.ThreeColumn;
  }
}
