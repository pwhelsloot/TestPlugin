import { FilterDefinitionSelected } from '@coremodels/filter/FilterDefinitionSelected';

export class Filter {
  filterDefinitions: FilterDefinitionSelected[];

  constructor(filterDefinitions: FilterDefinitionSelected[]) {
    this.filterDefinitions = filterDefinitions || [];
  }

  equals(filter: Filter) {
    if (!(this.filterDefinitions && filter.filterDefinitions)) {
      return false;
    }

    if (this.filterDefinitions.length !== filter.filterDefinitions.length) {
      return false;
    }

    for (const key in this.filterDefinitions) {
      if (!this.filterDefinitions[key].equals(filter.filterDefinitions[key])) {
        return false;
      }
    }

    return true;
  }
}
