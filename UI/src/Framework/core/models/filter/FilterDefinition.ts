import { FilterOption } from '@coremodels/filter/FilterOption';
import { FilterDisplayMode } from './filter-display-mode.enum';

export class FilterDefinition {
  id: number;
  description: string;

  // whether this filter definition is expanded or not
  expanded: boolean;

  // the pattern by which the filter options have been filtered.
  pattern: string;

  numberOfOptionsToDisplay: number;

  filterOptions: FilterOption[];

  startChars: string[];

  groupedCharOptions: FilterOption[];

  filterDisplayMode: FilterDisplayMode;

  keepOriginalSorting: boolean;
  skipAllOptionOnExpandView: boolean;
  allowSelectAll: boolean;

  constructor(id: number, description: string, filterOptions: FilterOption[], pattern?: string, expanded?: boolean,
    filterDisplayMode?: FilterDisplayMode, skipAllOptionOnExpandView?: boolean, allowSelectAll?: boolean, keepOriginalSorting?: boolean) {
    this.id = id;
    this.description = description;
    this.filterOptions = filterOptions || [];
    this.expanded = expanded || false;
    this.pattern = pattern || '';
    this.filterDisplayMode = filterDisplayMode || FilterDisplayMode.CheckBox;
    this.skipAllOptionOnExpandView = skipAllOptionOnExpandView || false;
    this.allowSelectAll = allowSelectAll || false;
    this.keepOriginalSorting = keepOriginalSorting || false;
  }

  equals(filterDef: FilterDefinition) {
    return this.id === filterDef.id &&
      this.description === filterDef.description &&
      this.expanded === filterDef.expanded &&
      this.pattern === filterDef.pattern &&
      this.filterDisplayMode === filterDef.filterDisplayMode &&
      this.numberOfOptionsToDisplay === filterDef.numberOfOptionsToDisplay &&
      this.skipAllOptionOnExpandView === filterDef.skipAllOptionOnExpandView &&
      this.allowSelectAll === filterDef.allowSelectAll &&
      this.keepOriginalSorting === filterDef.keepOriginalSorting &&
      this.filterOptionsEqual(filterDef.filterOptions);
  }

  clone(): FilterDefinition {
    const clone = new FilterDefinition(this.id, this.description, this.filterOptions.map(opt => opt.clone()),
      this.pattern, this.expanded, this.filterDisplayMode, this.skipAllOptionOnExpandView, this.allowSelectAll, this.keepOriginalSorting);
    clone.numberOfOptionsToDisplay = this.numberOfOptionsToDisplay;
    return clone;
  }

  filterOptionsEqual(filterOptions: FilterOption[]) {
    if (!(this.filterOptions && filterOptions)) {
      return false;
    }

    if (this.filterOptions.length !== filterOptions.length) {
      return false;
    }

    for (const key in this.filterOptions) {
      if (!this.filterOptions[key].equals(filterOptions[key])) {
        return false;
      }
    }
    return true;
  }

  getNumberOfOptionsNotDisplayed() {
    const matchCount = this.filterOptions.filter(x => x.matchesPattern).length;

    if (matchCount === this.numberOfOptionsToDisplay + 1) {
      return 0;
    }
    return Math.max(matchCount, this.numberOfOptionsToDisplay) - this.numberOfOptionsToDisplay;
  }

  getOptionsToDisplay(): FilterOption[] {
    const matches = this.filterOptions.filter(x => x.matchesPattern);
    if (matches.length === this.numberOfOptionsToDisplay + 1) {
      return matches.slice(0, this.numberOfOptionsToDisplay + 1);
    }
    return matches.slice(0, this.numberOfOptionsToDisplay);
  }

  getAllOptionsMatchingPattern(): FilterOption[] {
    return this.filterOptions.filter(x => x.matchesPattern);
  }

  setAllOptionsMatchingPatternGroupedByFirstCharacter() {
    const reducer = (accumulator: FilterOption[], option: FilterOption) => {
      const startChar = option.description.trim().charAt(0).toUpperCase();
      option.displayInitialCharacter = !accumulator.find(x => x.description.trim().charAt(0).toUpperCase() === startChar);
      accumulator.push(option);
      return accumulator;
    };
    this.groupedCharOptions = this.filterOptions.filter(x => x.matchesPattern).reduce(reducer, []);
    if (this.skipAllOptionOnExpandView) {
      this.groupedCharOptions = this.groupedCharOptions.filter(x => x.id !== 0);
    }
    this.groupedCharOptions.forEach(element => {
      element.initialCharacter = element.description.charAt(0).toUpperCase();
    });
  }

  setDistinctFilterOptionStartCharacters() {
    const reducer = (accumulator: string[], character: string) => {
      if (!accumulator.find(x => x === character)) {
        accumulator.push(character);
      }
      return accumulator;
    };
    if (this.skipAllOptionOnExpandView) {
      this.startChars = this.filterOptions.filter(x => x.matchesPattern && x.id !== 0).map(x => x.description.trim().charAt(0).toUpperCase()).reduce(reducer, []);
    } else {
      this.startChars = this.filterOptions.filter(x => x.matchesPattern).map(x => x.description.trim().charAt(0).toUpperCase()).reduce(reducer, []);
    }
  }
}
