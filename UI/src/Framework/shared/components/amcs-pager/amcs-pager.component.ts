import { AfterViewInit, Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild } from '@angular/core';
import { MatPaginator, PageEvent } from '@angular/material/paginator';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subject, Subscription } from 'rxjs';

@Component({
  selector: 'app-amcs-pager',
  templateUrl: './amcs-pager.component.html',
  styleUrls: ['./amcs-pager.component.scss']
})
export class AmcsPagerComponent extends AutomationLocatorDirective implements OnInit, AfterViewInit, OnDestroy {

  @Input('totalItemCount') totalItemCount: number;
  @Input('pageSize') pageSize: number;
  @Input('pageIndex') pageIndex: number;
  @Input('pageResetRequest') pageResetRequest: Subject<void>;
  @Input() loading = false;
  @Input('hidePageSize') hidePageSize = true;
  @Input('pageSizeOptions') pageSizeOptions: number[] = [];

  @Output('pageChange') pageChange = new EventEmitter<number>();
  @Output('pageSizeChange') pageSizeChange = new EventEmitter<PageEvent>();

  @ViewChild(MatPaginator) paginator: MatPaginator;

  private pageResetSubscription: Subscription;

  ngOnInit() {
    if (!isTruthy(this.pageIndex)) {
      this.pageIndex = 0;
    }
  }

  ngAfterViewInit() {
    if (isTruthy(this.pageResetRequest)) {
      this.pageResetSubscription = this.pageResetRequest.subscribe(() => {
        if (isTruthy(this.paginator)) {
          if (this.paginator.pageIndex === 0) {
            this.pageChange.next(0);
          }
          this.paginator.firstPage();
        }
      });
    }
  }

  ngOnDestroy() {
    if (isTruthy(this.pageResetSubscription)) {
      this.pageResetSubscription.unsubscribe();
    }
  }

  pageChanged(page: PageEvent) {
    this.pageChange.emit(page.pageIndex);
    this.pageSizeChange.emit(page);
  }
}
