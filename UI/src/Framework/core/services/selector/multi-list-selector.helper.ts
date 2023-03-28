import { MultiListSelectorAdapter } from '@core-module/services/selector/multi-list-selector.adapter';
import { MultiTileSelector } from '@core-module/models/selector/multi-tile-selector.model';
import { IMultiTileSelectorItem } from '@core-module/models/selector/multi-tile-selector-item.interface';
import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { InnerTileServiceUI } from '../ui/inner-tile.service.ui';
import { Subscription } from 'rxjs';

export class MultiListSelectorHelper {

    loading = true;
    tileIds: string[];

    constructor(public adapter: MultiListSelectorAdapter, private tileUiService: InnerTileServiceUI) {
        this.initalisedSubscription = this.adapter.initalised.subscribe((initalised: boolean) => {
            this.loading = !initalised;
            if (initalised) {
                this.setTileArrays();
            }
        });
    }

    private initalisedSubscription: Subscription;

    handleMoveLeft(result: boolean, idsToMove: number[], fromTileId: number, toTileId: number) {
        if (result) {
            this.doMove(idsToMove, fromTileId, toTileId);
            this.setAllTilesMoveButtonsDisabled();
        }
        this.setAllTilesMoveNotMoving();
    }

    handleMoveRight(result: boolean, idsToMove: number[], fromTileId: number, toTileId: number) {
        if (result) {
            this.doMove(idsToMove, fromTileId, toTileId);
            this.setAllTilesMoveButtonsDisabled();
        }
        this.setAllTilesMoveNotMoving();
    }

    handleDragDropMove(result: boolean, event: CdkDragDrop<IMultiTileSelectorItem[]>) {
        if (result) {
            this.setAllTilesMoveButtonsDisabled();
            const toTile = this.getTile(Number(event.container.id));
            toTile.items = toTile.disabledItems.concat(toTile.enabledItems);
            this.adapter.afterSuccessfulMove(toTile, event.container.data);
            this.tileUiService.innerTilesChanged.next();
        } else {
            // RDM - We've already moved it so the animation looks correct, if the move fails we need to change it back!
            if (event.previousContainer === event.container) {
                moveItemInArray(event.container.data, event.currentIndex, event.previousIndex);
            } else {
                transferArrayItem(event.container.data,
                    event.previousContainer.data,
                    event.currentIndex,
                    event.previousIndex);
            }
        }
        this.setAllTilesMoveNotMoving();
    }

    itemSelected(selectedTile: MultiTileSelector, selectedItem: IMultiTileSelectorItem) {
        this.setTilesMoveButtonsEnabled(selectedTile);
        // If can't multi-select then deselect others in same tile
        if (!selectedTile.canMultiSelect) {
            selectedTile.enabledItems.filter(x => x.id !== selectedItem.id).forEach(element => {
                element.isSelected = false;
            });
        }
        // Deselect others in other tiles
        this.adapter.tiles.filter(x => x.id !== selectedTile.id).forEach(tile => {
            tile.enabledItems.forEach(element => {
                element.isSelected = false;
            });
        });
    }

    itemUnSelected(selectedTile: MultiTileSelector) {
        // If not more items are selected this disable move buttons
        if (!selectedTile.enabledItems.some(x => x.isSelected)) {
            this.setAllTilesMoveButtonsDisabled();
        }
    }

    getTile(tileId: number): MultiTileSelector {
        return this.adapter.tiles.find(x => x.id === tileId);
    }

    destroy() {
        this.initalisedSubscription.unsubscribe();
    }

    private setTileArrays() {
        this.adapter.tiles.forEach(element => {
            element.enabledItems = element.items.filter(x => x.canMove);
            element.disabledItems = element.items.filter(x => !x.canMove);
        });
        this.tileIds = this.adapter.tiles.map(x => x.id.toString());
        this.loading = false;
    }

    private doMove(idsToMove: number[], fromTileId: number, toTileId: number) {
        // Grab items from tile
        const fromTile: MultiTileSelector = this.getTile(fromTileId);
        const itemsToMove: IMultiTileSelectorItem[] = fromTile.enabledItems.filter(x => idsToMove.some(y => y === x.id));
        // Remove items from tile
        fromTile.enabledItems = fromTile.enabledItems.filter(x => !idsToMove.some(y => y === x.id));
        // Add items at top of to tile
        const toTile: MultiTileSelector = this.getTile(toTileId);
        itemsToMove.forEach(element => {
            element.isSelected = false;
        });
        toTile.enabledItems.unshift(...itemsToMove);
        toTile.items = toTile.disabledItems.concat(toTile.enabledItems);
        this.adapter.afterSuccessfulMove(toTile, itemsToMove);
        this.tileUiService.innerTilesChanged.next();
    }

    // Figures out which move buttons on screen should be in enabled
    private setTilesMoveButtonsEnabled(selectedTile: MultiTileSelector) {
        for (let index = 0; index < this.adapter.tiles.length; index++) {
            const element = this.adapter.tiles[index];
            element.moveRightEnabled = false;
            element.moveLeftEnabled = false;
            if (element === selectedTile) {
                // Is first
                if (index === 0) {
                    element.moveLeftEnabled = false;
                    element.moveRightEnabled = true;
                    // Is last
                } else if (index === (this.adapter.tiles.length - 1)) {
                    this.adapter.tiles[index - 1].moveLeftEnabled = true;
                    this.adapter.tiles[index - 1].moveRightEnabled = false;
                    // In middle somewhere
                } else {
                    this.adapter.tiles[index - 1].moveLeftEnabled = true;
                    element.moveRightEnabled = true;
                }
            }
        }
    }

    private setAllTilesMoveButtonsDisabled() {
        this.adapter.tiles.forEach(element => {
            element.moveRightEnabled = false;
            element.moveLeftEnabled = false;
        });
    }

    private setAllTilesMoveNotMoving() {
        this.adapter.tiles.forEach(tile => {
            tile.enabledItems.forEach(item => {
                item.isMoving = false;
            });
        });
    }
}
