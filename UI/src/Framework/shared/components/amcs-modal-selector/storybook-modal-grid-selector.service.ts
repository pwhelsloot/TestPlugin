import { Injectable } from '@angular/core';
import { CountCollectionModel } from '@core-module/models/api/count-collection.model';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { ModalGridSelectorServiceAdapter } from '@core-module/services/forms/modal-grid-selector.adapter';
import { Observable, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';

export interface SBModelSelectorItem {
  id: number;
  column1: string;
  column2: string;
  column3: string;
  description: string;
}
@Injectable()
export class StorybookeModalGridSelectorService extends ModalGridSelectorServiceAdapter {
  // Interface variable
  itemsObservable$: Observable<CountCollectionModel<ILookupItem>>;
  itemsSubject: Subject<CountCollectionModel<ILookupItem>> = new Subject<CountCollectionModel<ILookupItem>>();

  constructor() {
    super();
    this.setupItemStream();
  }
  private itemsRequest: Subject<number> = new Subject<number>();
  private page: number;

  // Interface method
  requestItems(page: number) {
    this.page = page;
    this.itemsRequest.next(page);
  }

  private getDefaultItems() {
    const results = new CountCollectionModel<SBModelSelectorItem>(
      [
        { id: 1, column1: 'Bag Collections', column2: 'Callout', column3: 'Per Job', description: '' },
        { id: 2, column1: 'Collection-Bin', column2: 'Charge', column3: 'Fixed', description: '' },
        { id: 3, column1: 'Collection-Bin', column2: 'Container Drop', column3: 'Per Job', description: '' },
        { id: 4, column1: 'Collection-Bin', column2: 'Container Exchange', column3: 'Per Job', description: '' },
        { id: 5, column1: 'Collection-Bin', column2: 'Container Remove', column3: 'Per Job', description: '' },
        { id: 6, column1: 'Inbound - Gate', column2: 'Secondary Processing', column3: 'Per Hour', description: '' },
        { id: 7, column1: 'Material Sales', column2: 'Charge', column3: 'Fixed', description: '' },
        { id: 8, column1: 'Route Weighing', column2: 'Processing', column3: 'Per Weight', description: '' },
        { id: 9, column1: 'Schedule Weighing', column2: 'Processing', column3: 'Per Volume', description: '' },
        { id: 10, column1: 'Scheduled - Riris', column2: 'Charge', column3: 'Per Hour', description: '' },
        { id: 11, column1: 'Scheduled - Riris', column2: 'Secondary Job', column3: 'Per Job', description: '' },
        { id: 12, column1: 'Scheduled - Skips', column2: 'Charge', column3: 'Fixed', description: '' },
        { id: 13, column1: 'Scheduled - Skips', column2: 'Charge', column3: 'Per Item', description: '' },
        { id: 14, column1: 'Scheduled Skip Bags', column2: 'Exchange', column3: 'Per Item', description: '' },
        { id: 15, column1: 'Transfer Out', column2: 'Processing', column3: 'Per Item', description: '' }
      ],
      15
    );
    return results;
  }

  private setupItemStream() {
    this.itemsObservable$ = this.itemsSubject.asObservable();
    this.itemsRequest.pipe(takeUntil(this.unsubscribe)).subscribe(() => {
      const results = this.getDefaultItems();
      this.itemsSubject.next({ results: results.results.slice(this.page * 10, this.page * 10 + 10), count: results.count });
    });
  }
}
