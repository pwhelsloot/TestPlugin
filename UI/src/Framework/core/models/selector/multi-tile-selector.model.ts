import { IMultiTileSelectorItem } from './multi-tile-selector-item.interface';

export class MultiTileSelector {
    id: number;

    title: string;

    items: IMultiTileSelectorItem[];

    canMultiSelect = true;

    moveLeftEnabled = false;

    moveRightEnabled = false;

    enabledItems: IMultiTileSelectorItem[];

    disabledItems: IMultiTileSelectorItem[];
}
