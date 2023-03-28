import { animate, style, transition, trigger } from '@angular/animations';
import { Component, ElementRef, Input, OnChanges, OnDestroy, OnInit, Output, Renderer2, SimpleChanges, TemplateRef } from '@angular/core';
import { MediaChange, MediaObserver } from '@angular/flex-layout';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ResizeableColumnHelper } from '@core-module/helpers/resizeable-column.helper';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ISelectorItem } from '@shared-module/models/iselector-item.model';
import { Subject, Subscription } from 'rxjs';
import { GridColumnAlignment } from '../amcs-grid/grid-column-alignment.enum';
import { GridColumnConfig } from '../amcs-grid/grid-column-config';
import { GridColumnType } from '../amcs-grid/grid-column-type.enum';
import { GridViewport } from '../amcs-grid/grid-viewport.enum';

/**
 * @deprecated Use the app-amcs-grid with 'showSelectedCheckbox' set to True
 */
@Component({
    selector: 'app-amcs-selector-grid',
    templateUrl: './amcs-selector-grid.component.html',
    styleUrls: ['./amcs-selector-grid.component.scss'],
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
export class AmcsSelectorGridComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {
    @Input('data') data: ISelectorItem[];
    @Input('columns') columns: GridColumnConfig[];
    @Input('disableSelection') disableSelection = false;
    @Input('popoverTemplate') popoverTemplate: TemplateRef<any>;
    @Input('popoverConfig') popoverConfig = { allowOnUnselectedRows: false };
    @Input('singleSelectionEnforced') singleSelectionEnforced = false;
    @Input('actionColumnTemplate') actionColumnTemplate: TemplateRef<any>;
    @Input('rowDetailTemplate') rowDetailTemplate: TemplateRef<any>;
    @Input('mobileRowDetailTemplate') mobileRowDetailTemplate: TemplateRef<any>;
    @Input('detailsLoading') detailsLoading = false;
    @Input('noDataTemplate') noDataTemplate: TemplateRef<any>;
    @Input('allowColumnRefresh') allowColumnRefresh = false;
    @Input('selectAll') selectAll = false;
    @Input('alignHeaderWithCell') alignHeaderWithCell = false;
    @Input() allowHorizontalScroll = false;
    @Input() gridHeight = 'auto';
    @Input() loading = false;
    @Input() noMargin = false;

    @Output('rowsSelected') rowsSelected = new Subject<ISelectorItem[]>();
    @Output('linkClicked') linkClicked = new Subject<ISelectorItem>();
    @Output('popoverClicked') popoverClicked = new Subject<ISelectorItem>();
    @Output('rowExpanded') rowExpanded = new Subject<any>();

    // enums
    Viewport = GridViewport;
    ColumnType = GridColumnType;
    GridColumnAlignment = GridColumnAlignment;
    expandedRowId: number;

    currentViewport: GridViewport = GridViewport.desktop;
    displayMode = FormControlDisplay.Grid;
    visibleColumns: GridColumnConfig[] = [];
    selectedRows: ISelectorItem[] = [];
    // Deary me this isn't nice but no other way to ensure multi-popovers don't appear together!
    isPopoverOpenArray: boolean[] = [];
    resizeHelper: ResizeableColumnHelper = null;
    isSelectAll = false;

    constructor(protected elRef: ElementRef, protected renderer: Renderer2,
        public tileUiService: InnerTileServiceUI,
        private media: MediaObserver) {
        super(elRef, renderer);
    }

    private mediaChangeSubscription: Subscription;
    private columnPositionsInitialised: boolean;

    ngOnInit() {
        if (this.media.isActive('xs') || this.media.isActive('sm')) {
            this.currentViewport = GridViewport.mobile;
        } else {
            this.currentViewport = GridViewport.desktop;
        }

        if (isTruthy(this.data)) {
            const selectedLength = this.data.filter((x) => x.isSelected).length;
            if (this.data.length === selectedLength && this.data.length > 0) {
                this.isSelectAll = true;
            } else {
                this.isSelectAll = false;
            }
        }

        this.changeViewport(this.currentViewport);

        this.mediaChangeSubscription = this.media.media$.subscribe((change: MediaChange) => {
            // for now, the grid supports only 2 different viewports - desktop and mobile.
            // here we map the flex-layout viewports to these two.
            let viewport: GridViewport = null;

            switch (change.mqAlias) {
                case 'xs':
                    viewport = GridViewport.mobile;
                    break;
                case 'sm':
                    viewport = GridViewport.mobile;
                    break;
                case 'md':
                    viewport = GridViewport.desktop;
                    break;
                case 'lg':
                    viewport = GridViewport.desktop;
                    break;
                case 'xl':
                    viewport = GridViewport.desktop;
                    break;
            }

            if (viewport !== this.currentViewport) {
                this.currentViewport = viewport;
                this.changeViewport(this.currentViewport);
            }
        });
    }

    ngOnChanges(changes: SimpleChanges) {
        if (changes['columns'] && this.columns && (!this.columnPositionsInitialised || this.allowColumnRefresh)) {
            this.initialiseColumnPositions();
            this.columnPositionsInitialised = true;
            this.changeViewport(this.currentViewport);
        }

        if (changes['data'] && this.data) {
            this.expandedRowId = null; // Reset the expanded row so that this.expand always works
            this.selectedRows = this.data.filter((x) => x.isSelected);
            this.isPopoverOpenArray = [];
            this.data.forEach((element) => {
                this.isPopoverOpenArray[element.id] = false;
            });
        }

        if (this.data && this.data.length === this.data.filter(x => x.isSelected).length && this.data.length > 0) {
            this.isSelectAll = true;
        } else {
            this.isSelectAll = false;
        }
    }

    ngOnDestroy() {
        if (this.mediaChangeSubscription) {
            this.mediaChangeSubscription.unsubscribe();
        }
    }

    selectRowWithId(id: number) {
        if (isTruthy(id)) {
            this.selectedRows = this.data.filter((x) => x.id === id);
            this.data
                .filter((x) => x.id === id)
                .forEach((x) => {
                    x.isSelected = true;
                });
            this.data
                .filter((x) => x.id !== id)
                .forEach((x) => {
                    x.isSelected = false;
                });
        }
    }

    onActionClicked(event) {
        event.stopPropagation();
    }

    onLinkClicked(event, row: ISelectorItem) {
        event.stopPropagation();
        this.linkClicked.next(row);
    }

    onExpandRowClicked(rowId: number) {
        this.expand(rowId);
        event.stopPropagation();
    }

    onPopoverClicked(event, row: ISelectorItem) {
        this.data
            .filter((x) => x.id !== row.id)
            .forEach((element) => {
                this.isPopoverOpenArray[element.id] = false;
            });
        setTimeout(() => {
            this.isPopoverOpenArray[row.id] = !this.isPopoverOpenArray[row.id];
        }, 0);
        event.stopPropagation();
        this.popoverClicked.next(row);
    }

    onRowClicked(row: ISelectorItem) {
        setTimeout(() => {
            if (!this.disableSelection && !row.forceDisabled) {
                row.isSelected = !row.isSelected;

                if (this.singleSelectionEnforced && row.isSelected) {
                    this.selectedRows = this.data.filter((x) => x.id === row.id);
                    this.data
                        .filter((x) => x.id !== row.id)
                        .forEach((x) => {
                            x.isSelected = false;
                        });
                }

                this.rowSelectionChanged();
            }
        }, 0);
    }

    onSelectAllClicked(selectAll: boolean) {
        this.data.forEach((i) => (i.isSelected = i.forceDisabled ? i.isSelected : selectAll));
        this.rowSelectionChanged();
    }

    numericInputChanged(row: ISelectorItem, column: GridColumnConfig, input: number) {
        this.inputChanged(row, column, input, false);
    }

    integerInputChanged(row: ISelectorItem, column: GridColumnConfig, input: number) {
        this.inputChanged(row, column, input, true);
    }

    textInputChanged(row: ISelectorItem, column: GridColumnConfig, input: string) {
        row[column.key] = input;
    }

    stopEvent(event) {
        event.stopPropagation();
    }

    private expand(rowId: number) {
        this.expandedRowId = this.expandedRowId === rowId ? null : rowId;
        this.rowExpanded.next(this.expandedRowId);
    }

    private inputChanged(row: ISelectorItem, column: GridColumnConfig, text: number, isIntegerInput: boolean) {
        (row as any).inputValueOutOfBounds = false;

        if (text !== null && text !== undefined && text !== row[column.key]) {
            const numericValue = +text;

            if (!isNaN(numericValue)) {
                if (column.minInputValue !== null && numericValue < column.minInputValue) {
                    // set a flag so can indicate to the user it's not valid
                    (row as any).inputValueOutOfBounds = true;
                }

                if (column.maxInputValue !== null && numericValue > column.maxInputValue) {
                    // set a flag so can indicate to the user it's not valid
                    (row as any).inputValueOutOfBounds = true;
                }

                row[column.key] = numericValue;
                this.rowsSelected.next(this.selectedRows);
            }
        } else if (text === null) {
            row[column.key] = null;
            this.rowsSelected.next(this.selectedRows);
        }
    }

    private rowSelectionChanged() {
        this.selectedRows = [];
        this.data.forEach((element) => {
            if (element.isSelected) {
                this.selectedRows.push(element);
            }
        });
        if (this.data.length === this.selectedRows.length) {
            this.isSelectAll = true;
        } else {
            this.isSelectAll = false;
        }
        this.rowsSelected.next(this.selectedRows);
    }

    private initialiseColumnPositions() {
        if (this.columns) {
            const withPosition = this.columns.filter((x) => x.position !== -1);
            const withoutPosition = this.columns.filter((x) => x.position === -1);
            withPosition.sort((a, b) => a.position - b.position);
            withPosition.push(...withoutPosition);
            let position = 1;
            withPosition.forEach((x) => {
                x.position = position;
                position++;
            });

            this.columns = withPosition.sort((a, b) => a.position - b.position);
        }
    }

    private changeViewport(viewport: GridViewport) {
        if (this.columns) {
            this.visibleColumns = this.columns.filter((col) => col.viewports.find((x) => x.viewport === viewport && x.visible));
            this.visibleColumns.forEach((col) => {
                const columnViewport = col.viewports.find((x) => x.viewport === viewport);
                col.widthPercentage = columnViewport.widthPercentage;
            });
            this.resizeHelper = new ResizeableColumnHelper(this.visibleColumns);
        }
    }
}
