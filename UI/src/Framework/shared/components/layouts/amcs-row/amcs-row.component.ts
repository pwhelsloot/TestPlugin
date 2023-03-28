import { AfterContentInit, Component, ContentChildren, OnDestroy, QueryList } from '@angular/core';
import { Subscription } from 'rxjs';
import { isNullOrUndefined } from '../../../../core/helpers/is-truthy.function';
import { AmcsColumnComponent } from '../amcs-column/amcs-column.component';
import { verifyIsValidBSColumnSize, verifyIsWithinBSBoundaries } from '../bootstrap-helper';

@Component({
  selector: 'app-amcs-row',
  templateUrl: './amcs-row.component.html',
  styleUrls: ['./amcs-row.component.scss'],
  // tslint:disable: no-host-metadata-property
  // eslint-disable-next-line @angular-eslint/no-host-metadata-property
  host: { class: 'row' },
})
export class AmcsRowComponent implements AfterContentInit, OnDestroy {
  @ContentChildren(AmcsColumnComponent) columns: QueryList<AmcsColumnComponent>;

  private flexibleColumns = 0;
  private maxSize = 12;
  private remainingDesktopSize = this.maxSize;
  private subscription: Subscription;

  ngAfterContentInit() {
    this.setSizes();
    this.subscription = this.columns.changes.subscribe(() => {
      setTimeout(() => {
        this.setSizes();
      }, 0);
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  private setSizes() {
    if (this.columns.length > 0) {
      verifyIsWithinBSBoundaries(this.columns.length);
      if (this.columns.length === 1) {
        this.setFullSize();
      } else {
        this.setFlexibleSizes();
      }
    }
  }

  private setFlexibleSizes() {
    this.flexibleColumns = this.columns.filter((column) => isNullOrUndefined(column.size)).length;
    const totalManualSize = this.columns.map((column) => (column.size ? column.size : 0)).reduce(this.countSize(), 0);
    verifyIsWithinBSBoundaries(totalManualSize);
    this.remainingDesktopSize -= totalManualSize;
    const desktopSize = this.calculateColumnWidth(this.remainingDesktopSize);
    verifyIsValidBSColumnSize(desktopSize, 'desktop size');
    this.columns.forEach((column) => {
      column.setColumnSizes(desktopSize);
    });
  }

  /**
   * Set first column to 100% width (col-12)
   *
   * @private
   * @memberof AmcsRowComponent
   */
  private setFullSize() {
    this.columns.first.setColumnSizes(12);
  }

  private countSize(): (previousValue: number, currentValue: number, currentIndex: number, array: number[]) => number {
    return (sum, current) => sum + current;
  }

  private calculateColumnWidth(remaininggSize: number) {
    return remaininggSize / this.flexibleColumns;
  }
}
