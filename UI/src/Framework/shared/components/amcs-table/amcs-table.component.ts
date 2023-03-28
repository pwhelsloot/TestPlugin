// eslint-disable-next-line max-len
import { animate, style, transition, trigger } from '@angular/animations';
import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, QueryList, Renderer2, SimpleChanges, TemplateRef, ViewChildren } from '@angular/core';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { AmcsTableService } from '@shared-module/components/amcs-table/amcs-table.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ITableItem } from '@shared-module/models/itable-item.model';
import { fromEvent, Subscription } from 'rxjs';
import { debounceTime, distinctUntilChanged, map } from 'rxjs/operators';

/**
 * @deprecated Marked for Removal, use the app-amcs-grid instead
 */
@Component({
    selector: 'app-amcs-table',
    templateUrl: './amcs-table.component.html',
    styleUrls: ['./amcs-table.component.scss'],
    providers: [AmcsTableService],
    animations: [
        trigger('expandAndCollapse', [
            transition(':enter', [
                style({ height: 0, overflow: 'hidden' }),
                animate('200ms', style({ height: '*' }))
            ]),
            transition(':leave', [
                style({ height: '*', overflow: 'hidden' }),
                animate('200ms', style({ height: 0 }))
            ])])
    ]
})
export class AmcsTableComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {

    @Input() rows: ITableItem[];
    @Input() rowTemplate: TemplateRef<any>;
    @Input() detailRowTemplate: TemplateRef<any>;
    @Input() detailsLoading = false;
    @Input() tableLayoutFixed = false;
    @Input() showFilter = false;
    @Input() filterPlaceholder: string;
    @Input() selectedRow: ITableItem;
    @Input() removeTableContainer = false;
    @Output() selectedRowChanged = new EventEmitter<ITableItem>();
    @Output() selectedPageChanged = new EventEmitter<number>();
    @ViewChildren('rowsRef') rowsRef: QueryList<ElementRef>;
    @Input() disableSelection = false;
    @Input() includePaging = false;
    @Input() totalItemCount = false;
    @Input() pageSize = 50;
    @Input() rowStriping = false;
    @Input() rowBorder = false;
    @Input() fixedContentHeight = false;

    filterText: string;
    filteredRows: ITableItem[];

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        public tileUiService: InnerTileServiceUI,
        private service: AmcsTableService,
        private elementRef: ElementRef) {
        super(elRef, renderer);
    }

    private filterKeyupEventSubscription: Subscription;

    ngOnInit() {
        if (this.showFilter) {
            this.filterKeyupEventSubscription = fromEvent(this.elementRef.nativeElement, 'keyup')
                .pipe(
                    map(() => this.filterText),
                    debounceTime(100),
                    distinctUntilChanged())
                .subscribe(input => this.onFilter(input ? input.toLowerCase() : ''));
        }
    }

    ngOnChanges(change: SimpleChanges) {
        // Handles case where item is pre-selected as componet is loaded.
        if (change && change['selectedRow'] && this.selectedRow) {
            setTimeout(() => {
                this.selectRow(this.selectedRow);
            }, 0);
        }

        // Takes copy of rows whenever they are changed.
        if (change && change['rows']) {
            this.filteredRows = this.rows != null ? this.rows.slice() : [];
            if (this.filterText) {
                this.onFilter(this.filterText.toLowerCase());
            }
        }
    }

    ngOnDestroy() {
        if (this.filterKeyupEventSubscription) {
            this.filterKeyupEventSubscription.unsubscribe();
        }
    }

    selectRow(row: ITableItem) {
        if (!this.disableSelection) {
            if (!row.isSelected) {
                this.rows.forEach(x => x.isSelected = false);
            }
            row.isSelected = !row.isSelected;
            if (row.isSelected) {
                this.service.scrollIntoView(row.id, this.rowsRef);
            }
            this.selectedRowChanged.emit(row);
        }
    }

    pageChanged(page: number) {
        this.selectedPageChanged.emit(page);
        this.service.scrollToTop(this.elementRef);
    }

    private onFilter(text: string) {
        this.filteredRows = this.rows.filter(x => x.filter(text));
    }
}
