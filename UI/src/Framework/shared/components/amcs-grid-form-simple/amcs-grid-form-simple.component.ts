import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, Renderer2, SimpleChanges, TemplateRef, ViewChild } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ResizeableColumnHelper } from '@core-module/helpers/resizeable-column.helper';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { AmcsGridFormSimpleService } from '@core-module/services/forms/amcs-grid-form-simple.service';
import { AmcsGridFormValidationService } from '@core-module/services/forms/amcs-grid-form-validation.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { BehaviorSubject, Observable, Subject, Subscription } from 'rxjs';
import { take } from 'rxjs/operators';
import { AmcsDropdownIconEnum } from '../amcs-dropdown/amcs-dropdown-icon-enum.model';
import { GridActionColumnHeaderOptions } from '../amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header-options';
import { AmcsGridFormColumn } from '../amcs-grid-form/amcs-grid-form-column';
import { AmcsGridFormConfig } from '../amcs-grid-form/amcs-grid-form-config.model';
import { GridColumnAlignment } from '../amcs-grid/grid-column-alignment.enum';
import { GridColumnType } from '../amcs-grid/grid-column-type.enum';
import { GridTotalsHeaderConfig } from '../amcs-grid/grid-totals-header-config.model';
import { IGridFormSimpleRowForm } from './amcs-grid-form-simple-row-form.interface';
import { AmcsGridFormSimpleRow } from './amcs-grid-form-simple-row.model';

@Component({
  selector: 'app-amcs-grid-form-simple',
  templateUrl: './amcs-grid-form-simple.component.html',
  styleUrls: ['./amcs-grid-form-simple.component.scss'],
  providers: [AmcsGridFormSimpleService]
})
export class AmcsGridFormSimpleComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {

  loading: boolean;
  disabled: boolean;
  styles = { display: 'flex', width: '100%' };
  AmcsDropdownIconEnum = AmcsDropdownIconEnum;
  displayMode = FormControlDisplay.Grid;
  ColumnType = GridColumnType;

  @Input() loading$: BehaviorSubject<boolean>;
  @Input() rows: AmcsGridFormSimpleRow[];
  @Input() focusOnErrorSubject: Subject<void>;
  @Input() formTemplate: TemplateRef<any>;
  @Input() columns: AmcsGridFormColumn[];
  @Input() config: AmcsGridFormConfig;
  @Input() createForm: (rowId: number) => AmcsFormGroup;
  @Input() clearForm: (form: IGridFormSimpleRowForm) => void;
  @Input() settingsMenu: TemplateRef<any>;
  @Input() enableClear = false;
  @Input() enableDelete = false;
  @Input() deleteRow: (row: AmcsGridFormSimpleRow) => void;
  @Input() allowColumnResize = true;
  @Input() allowColumnRebind = false;
  @Input() applyVirtualScroll = false;
  @Input() pageChanged: Subject<void> = new Subject<void>();
  @Input() gridHeight = 'auto';
  @Input() hideHeader = false;
  @Input() hideBackgroundColor = false;
  @Input() noDataTemplate: TemplateRef<any>;
  @Input() virtualScrollItemSize = 10;
  @Input() virtualScrollHeight = 300;
  @Input() actionColumnHeaderOptions: GridActionColumnHeaderOptions;
  @Input() stickyHeader = false;
  @Input() totalsHeaderConfig: GridTotalsHeaderConfig;
  @Input() headerSummaryEntity: any;

  @Output() onManualFocusError = new EventEmitter<any>();
  @Output('onRowsSelected') onRowsSelected = new EventEmitter<number>();
  @Output('onGridBlur') onGridBlur = new EventEmitter<boolean>();
  @ViewChild(CdkVirtualScrollViewport) viewPort: CdkVirtualScrollViewport;

  GridColumnAlignment = GridColumnAlignment;
  resizeHelper: ResizeableColumnHelper = null;
  scrollSize = 0;

  topColumnTotals: { key: string; total: number }[] = [];

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public service: AmcsGridFormSimpleService, private validationService: AmcsGridFormValidationService) {
    super(elRef, renderer);
  }

  private loadingSubscription: Subscription;
  private focusOnErrorSubscription: Subscription;

  ngOnInit() {
    this.loadingSubscription = this.loading$.subscribe((loading: boolean) => {
      this.loading = loading;
      if (!this.loading) {
        // Need timeout here to ensure we get inputs
        setTimeout(() => {
          this.service.rows = this.rows;
          if (this.applyVirtualScroll && this.rows.length > 10) {
            this.scrollSize = 1.2;
          }
          this.service.createForm = this.createForm;
          this.service.clearForm = this.clearForm;
          this.service.deleteRow = this.deleteRow;
          this.service.config = this.config;
          this.service.onManualFocusError = this.onManualFocusError;
          this.service.setColumnSizes(this.columns, this.enableClear, this.enableDelete, isTruthy(this.actionColumnHeaderOptions), this.scrollSize);
          this.resizeHelper = new ResizeableColumnHelper(this.columns);
          this.service.setDisabledState();
          this.validationService.validForms$ = Observable.create((function(observer) {
            observer.next(this.service.getForms());
          }).bind(this));
          this.validationService.readySubject.next();
          this.configureHeaderRow();
          this.setupFormEvents();
        }, 0);
      }
    });

    this.pageChanged.pipe(take(1)).subscribe(() => {
      if (this.viewPort) {
        this.viewPort.scrollToIndex(0, 'smooth');
        this.viewPort.checkViewportSize();
      }
    });

    if (isTruthy(this.focusOnErrorSubject)) {
      this.focusOnErrorSubscription = this.focusOnErrorSubject.subscribe(() => {
        // triggers 'appFocusOnError'
        this.onManualFocusError.next();
      });
    }
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['rows'] && this.viewPort) {
      this.viewPort.scrollToIndex(this.rows.length - 1);
    }
    if (changes['columns'] && this.columns && this.allowColumnRebind) {
      this.service.setColumnSizes(this.columns, this.enableClear, this.enableDelete, isTruthy(this.actionColumnHeaderOptions), this.scrollSize);
    }
  }

  ngOnDestroy() {
    this.loadingSubscription.unsubscribe();
    if (isTruthy(this.resizeHelper)) {
      this.resizeHelper.destroy();
    }
    if (isTruthy(this.focusOnErrorSubscription)) {
      this.focusOnErrorSubscription.unsubscribe();
    }
  }

  onRowSelected(index: number) {
    this.onRowsSelected.next(index);
  }

  onClickOutside() {
    this.onGridBlur.next(true);
  }

  private setupFormEvents() {
    if(isTruthy(this.rows)) {
      this.rows.forEach(row => {
        row.form.htmlFormGroup.valueChanges.subscribe(() => {
          this.configureHeaderRow();
        });
      });
    }
  }

  private configureHeaderRow() {
    if (isTruthy(this.totalsHeaderConfig)) {
      this.columns
        .filter((column) => column.totalsHeaderConfig.includeInTopTotal)
        .forEach((column) => {
          const columnKey = isTruthy(column.totalsHeaderConfig.mappedToColumnKey)
            ? column.totalsHeaderConfig.mappedToColumnKey : column.key;

          if (isTruthy(this.headerSummaryEntity)) {
            this.topColumnTotals[column.key] = this.headerSummaryEntity[columnKey];
          } else if (this.rows.length > 0) {
            this.topColumnTotals[column.key] = this.rows.map((row) => row.form[columnKey].value)
              .reduce((sum, current) => sum + current);
          }
        });
    }
  }

}
