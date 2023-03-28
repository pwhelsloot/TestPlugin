import { GridSortDirection } from './grid-sort-direction.enum';

export class GridSortDefinition {
  key: string;
  direction: GridSortDirection;

  constructor(key: string, direction: GridSortDirection) {
    this.key = key;
    this.direction = direction;
  }
}
