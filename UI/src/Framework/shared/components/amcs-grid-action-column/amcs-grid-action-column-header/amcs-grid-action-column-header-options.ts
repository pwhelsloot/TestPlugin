import { EventEmitter } from '@angular/core';
import { GridActionColumnHeaderIconEnum } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header-icon.enum';
import { GridColumnAlignment } from '@shared-module/components/amcs-grid/grid-column-alignment.enum';

export class GridActionColumnHeaderOptions {
    icon = GridActionColumnHeaderIconEnum.Add;
    align = GridColumnAlignment.center;
    onIconClicked = new EventEmitter<GridActionColumnHeaderIconEnum>();
}
