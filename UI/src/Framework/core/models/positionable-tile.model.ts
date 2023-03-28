import { Type } from '@angular/core';
import { GridsterItem } from 'angular-gridster2';

export class PositionableTile implements GridsterItem {
    id: string;
    class?: string;
    component?: Type<any>;
    cols: number;
    rows: number;
    y: number;
    x: number;
}
