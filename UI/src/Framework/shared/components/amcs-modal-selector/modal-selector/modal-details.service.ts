import { Injectable } from '@angular/core';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { BaseService } from '@core-module/services/base.service';
import { ModalGridSelectorServiceAdapter } from '@core-module/services/forms/modal-grid-selector.adapter';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { BehaviorSubject, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

@Injectable()
export class ModalDetailsService extends BaseService {
  columns: GridColumnConfig[];
  items: CountCollectionModel<any>;
  modalLoading: boolean;
  isAnItemSelected = false;
  isMultiselect = false;
  autoClose = false;

  private loading: BehaviorSubject<boolean>;
  private externalClose: Subject<any>;
  private adapter: ModalGridSelectorServiceAdapter;
  private filters: IFilter[];
  private selectedIds: number[] = [];
  private isPagingAndMultiselect = false;
  private allPagesSelectedIds: number[] = [];
  private allPagesItems: ILookupItem[] = [];

  initialise(extraData: any, loading: BehaviorSubject<boolean>, externalClose: Subject<any>) {
    [this.selectedIds, this.columns, this.adapter, this.filters, this.isMultiselect, this.autoClose] = extraData;
    this.loading = loading;
    this.allPagesSelectedIds = this.selectedIds;
    this.externalClose = externalClose;
    this.modalLoading = true;
    this.adapter.itemsObservable$.pipe(takeUntil(this.unsubscribe)).subscribe((data: CountCollectionModel<ILookupItem>) => {
      this.isPagingAndMultiselect = data.results.length !== data.count && this.isMultiselect;
      this.items = data;
      this.loading.next(false);
      this.modalLoading = false;
      this.updateAllPagesItems();
      if (this.selectedIds.length > 0) {
        this.items.results
          .filter((result) => {
            this.isAnItemSelected = true;
            return this.selectedIds.find((id) => id === result.id);
          })
          .map((x) => (x.isSelected = true));
      }
    });
    this.adapter.requestItems(0, this.filters);
  }

  onPageChange(page: number) {
    this.adapter.requestItems(page, this.filters);
  }

  rowSelected(rowIds: number[]) {
    if (this.isPagingAndMultiselect) {
      const resultsSet = new Set([...rowIds, ...this.getIdsFromOtherPages()]);
      this.selectedIds = [...resultsSet];
      this.allPagesSelectedIds = this.selectedIds;
    } else {
      this.selectedIds = rowIds;
    }
    this.isAnItemSelected = this.selectedIds.length !== 0;
    if (this.autoClose) {
      this.closeModal();
    }
  }

  cancel() {
    this.externalClose.next([]);
  }

  closeModal() {
    this.externalClose.next(this.allPagesItems.filter((row) => this.selectedIds.find((selectedId) => row.id === selectedId)));
  }

  // Keeps track of all items over all pages and ensure there is no duplicates
  private updateAllPagesItems() {
    this.allPagesItems = [
      ...this.allPagesItems,
      ...this.items.results.filter((row) => !this.allPagesItems.some((existingRow) => existingRow.id === row.id))
    ];
  }

  private getIdsFromOtherPages(): number[] {
    const rowIdsOnPage: number[] = this.items.results.map((row) => row.id);
    // Return all original ids who are not in the current page
    return this.allPagesSelectedIds.filter((originalId) => !rowIdsOnPage.some((rowIdOnPage) => rowIdOnPage === originalId));
  }
}
