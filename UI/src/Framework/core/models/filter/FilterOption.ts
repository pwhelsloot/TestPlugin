export class FilterOption {
  id: number;
  filterDefinitionId: number;
  description: string;
  selected: boolean;
  matchesPattern: boolean;
  displayInitialCharacter: boolean;
  initialCharacter: string;
  hasAllSelected: boolean;

  constructor(id: number, description: string, selected?: boolean, filterDefinitionId?: number, displayInitialCharacter?: boolean, initialCharacter?: string) {
    this.id = id;
    this.description = description;
    this.selected = selected || false;
    this.filterDefinitionId = filterDefinitionId || 0;
    this.matchesPattern = false;
    this.displayInitialCharacter = displayInitialCharacter || false;
    this.initialCharacter = initialCharacter || null;
  }

  equals(filterOption: FilterOption) {
    return this.id === filterOption.id &&
      this.filterDefinitionId === filterOption.filterDefinitionId &&
      this.description === filterOption.description &&
      this.selected === filterOption.selected &&
      this.matchesPattern === filterOption.matchesPattern &&
      this.displayInitialCharacter === filterOption.displayInitialCharacter;
  }

  clone(): FilterOption {
    const clone = new FilterOption(this.id, this.description, this.selected, this.filterDefinitionId, this.displayInitialCharacter, this.initialCharacter);
    clone.matchesPattern = this.matchesPattern;
    return clone;
  }
}
