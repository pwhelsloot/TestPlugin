import { GridSortDirection } from '../../amcs-grid/grid-sort-direction.enum';

/**
 * Used by the sort changed event in the amcs-grid
 *
 * @export
 * @class SortChangedEvent
 */
export class SortChangedEvent {
  key: string;
  direction: GridSortDirection;
}
