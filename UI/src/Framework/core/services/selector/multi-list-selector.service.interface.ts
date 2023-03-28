import { Observable, ReplaySubject } from 'rxjs';
import { MultiTileSelector } from '@core-module/models/selector/multi-tile-selector.model';
import { IMultiTileSelectorItem } from '@core-module/models/selector/multi-tile-selector-item.interface';

export interface IMultiListSelectorService {

    tiles: MultiTileSelector[];

    initalised: ReplaySubject<boolean>;

    moveLeftText: string;

    moveRightText: string;
    move(ids: number[], toTileId: number): Observable<boolean>;

    afterSuccessfulMove(toTile: MultiTileSelector, items: IMultiTileSelectorItem[]);
}
