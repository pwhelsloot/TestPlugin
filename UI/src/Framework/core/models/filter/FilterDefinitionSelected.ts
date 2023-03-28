export class FilterDefinitionSelected {

  filterDefinitionId: number;

  filterOptions: number[];

  constructor(filterDefId: number, filterOptions: number[]) {
    this.filterDefinitionId = filterDefId;
    this.filterOptions = filterOptions || [];
  }

  equals(filterDef: FilterDefinitionSelected) {
    return this.filterDefinitionId === filterDef.filterDefinitionId &&
      this.filterOptionsEqual(filterDef.filterOptions);
  }

  filterOptionsEqual(filterOptions: number[]) {
    if (!(this.filterOptions && filterOptions)) {
      return false;
    }

    if (this.filterOptions.length !== filterOptions.length) {
      return false;
    }

    for (const key in this.filterOptions) {
      if (this.filterOptions[key] !== filterOptions[key]) {
        return false;
      }
    }
    return true;
  }
}
