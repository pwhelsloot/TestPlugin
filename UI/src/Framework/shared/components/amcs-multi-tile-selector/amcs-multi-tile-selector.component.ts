import { CdkDragDrop, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Component, ElementRef, Input, OnDestroy, OnInit, Renderer2, TemplateRef } from '@angular/core';
import { IMultiTileSelectorItem } from '@core-module/models/selector/multi-tile-selector-item.interface';
import { MultiTileSelector } from '@core-module/models/selector/multi-tile-selector.model';
import { MultiListSelectorAdapter } from '@core-module/services/selector/multi-list-selector.adapter';
import { MultiListSelectorHelper } from '@core-module/services/selector/multi-list-selector.helper';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { take } from 'rxjs/operators';

@Component({
  selector: 'app-amcs-multi-tile-selector',
  templateUrl: './amcs-multi-tile-selector.component.html',
  styleUrls: ['./amcs-multi-tile-selector.component.scss']
})
export class AmcsMultiTileSelectorComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {

  @Input() itemTextTemplate: TemplateRef<any>;
  @Input() isDynamicHeight = false;
  @Input() minHeight = 300;
  @Input() maxHeight = 500;
  @Input() disableOrdering = false;
  helper: MultiListSelectorHelper = null;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public adapter: MultiListSelectorAdapter, private tileUiService: InnerTileServiceUI) {
    super(elRef, renderer);
  }

  ngOnInit() {
    this.helper = new MultiListSelectorHelper(this.adapter, this.tileUiService);
  }

  moveLeft(fromTileId: number, toTileId: number) {
    const fromTile: MultiTileSelector = this.helper.getTile(fromTileId);
    const idsToMove: number[] = fromTile.enabledItems.filter(x => x.isSelected).map(x => x.id);
    fromTile.enabledItems.filter(x => x.isSelected).forEach(element => {
      element.isMoving = true;
    });
    this.adapter.move(idsToMove, toTileId).pipe(take(1)).subscribe((result: boolean) => {
      this.helper.handleMoveLeft(result, idsToMove, fromTileId, toTileId);
    });
  }

  moveRight(fromTileId: number, toTileId: number) {
    const fromTile: MultiTileSelector = this.helper.getTile(fromTileId);
    const idsToMove: number[] = fromTile.enabledItems.filter(x => x.isSelected).map(x => x.id);
    fromTile.enabledItems.filter(x => x.isSelected).forEach(element => {
      element.isMoving = true;
    });
    this.adapter.move(idsToMove, toTileId).pipe(take(1)).subscribe((result: boolean) => {
      this.helper.handleMoveRight(result, idsToMove, fromTileId, toTileId);
    });
  }

  itemSelected(tile: MultiTileSelector, item: IMultiTileSelectorItem) {
    if (item.canMove) {
      item.isSelected = !item.isSelected;
      if (item.isSelected) {
        this.helper.itemSelected(tile, item);
      } else {
        this.helper.itemUnSelected(tile);
      }
    }
  }

  // Ensure all items are unselected, drag doesn't support multi-select so don't want user being confused
  dragStarted() {
    this.adapter.tiles.forEach(tile => {
      tile.enabledItems.forEach(element => {
        element.isSelected = false;
      });
    });
  }

  dragFinished(tile: MultiTileSelector, event: CdkDragDrop<IMultiTileSelectorItem[]>) {
    const draggedItem: IMultiTileSelectorItem = event.previousContainer.data[event.previousIndex];
    draggedItem.isMoving = true;
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data,
        event.container.data,
        event.previousIndex,
        event.currentIndex);
    }
    this.adapter.move([draggedItem.id], Number(event.container.id)).pipe(take(1)).subscribe((result: boolean) => {
      this.helper.handleDragDropMove(result, event);
    });
  }

  ngOnDestroy() {
    this.helper.destroy();
  }
}
