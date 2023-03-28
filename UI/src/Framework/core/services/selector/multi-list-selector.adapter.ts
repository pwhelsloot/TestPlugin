import { IMultiListSelectorService } from './multi-list-selector.service.interface';
import { MultiTileSelector } from '@core-module/models/selector/multi-tile-selector.model';
import { Observable, ReplaySubject } from 'rxjs';
import { Injectable } from '@angular/core';
import { IMultiTileSelectorItem } from '@core-module/models/selector/multi-tile-selector-item.interface';

@Injectable()
export abstract class MultiListSelectorAdapter implements IMultiListSelectorService {

    abstract initalised: ReplaySubject<boolean>;

    abstract tiles: MultiTileSelector[];

    abstract moveLeftText: string;

    abstract moveRightText: string;

    abstract move(ids: number[], toTileId: number): Observable<boolean>;

    abstract afterSuccessfulMove(toTile: MultiTileSelector, items: IMultiTileSelectorItem[]);
}
