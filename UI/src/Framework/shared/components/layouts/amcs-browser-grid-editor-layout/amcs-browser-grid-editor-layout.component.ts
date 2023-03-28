import {
  Component,
  ElementRef,
  EventEmitter,
  forwardRef,
  Input,
  OnChanges,
  Output,
  Renderer2,
  SimpleChanges,
  TemplateRef,
  ViewContainerRef
} from '@angular/core';
import { ControlContainer, FormControl } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { IColumnLinkClickedEvent } from '@shared-module/components/amcs-grid-wrapper/column-link-clicked-event.interface';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { GridColumnType } from '@shared-module/components/amcs-grid/grid-column-type.enum';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';
import { BrowserGridEditorActionButtonEnum } from '@shared-module/components/layouts/amcs-browser-grid-editor-layout/browser-grid-editor-action-button.enum';
import { BrowserGridEditorOptions } from '@shared-module/components/layouts/amcs-browser-grid-editor-layout/browser-grid-editor-options.model';
import { ColumnPositionChangedEvent } from '@shared-module/components/layouts/amcs-browser-grid-layout/column-position-changed.event';
import { MenuItemClickedEvent } from '@shared-module/components/layouts/amcs-browser-grid-layout/menu-item-clicked-event';
import { OpenActionMenuEvent } from '@shared-module/components/layouts/amcs-browser-grid-layout/open-action-menu-event';
import { SortChangedEvent } from '@shared-module/components/layouts/amcs-browser-grid-layout/sort-changed.event';
import { ValueChangedEvent } from '@shared-module/components/layouts/amcs-browser-grid-layout/value-changed.event';
import { AmcsFormDirective, createFormGroupDirective } from '@shared-module/components/layouts/amcs-form/amcs-form.directive';
import { ISelectorItem } from '@shared-module/models/iselector-item.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Observable } from 'rxjs';
import { filter, take } from 'rxjs/operators';
import { BrowserDisplayMode } from './browser-display-mode.enum';

/**
 * A layout component which displays a browser tile, grid and form editor.
 */
@Component({
  selector: 'app-amcs-browser-grid-editor-layout',
  templateUrl: './amcs-browser-grid-editor-layout.component.html',
  styleUrls: ['./amcs-browser-grid-editor-layout.component.scss'],
  // RDM - Help for this from here https://stackoverflow.com/questions/53561484/access-formgroupdirective-of-container-component-inside-an-embeddedview
  // We want to have our 'formControlName' controls in a different component from the [formGroup] tag, only possible via providing a controlContainer.
  providers: [
    AmcsModalService,
    { provide: AmcsFormDirective, useExisting: forwardRef(() => AmcsBrowserGridEditorLayoutComponent) },
    {
      provide: ControlContainer,
      useFactory: createFormGroupDirective,
      deps: [AmcsFormDirective],
    },
  ],
})
export class AmcsBrowserGridEditorLayoutComponent extends AmcsFormDirective implements OnChanges {
  /**
   * Controls the amount of top padding for loading animation to be vertically centered
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  verticalLoadingPadding: number;

  /**
   * Options that will be used for the Browser, Grid and Editor
   *
   * @type {BrowserGridEditorOptions}
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() options: BrowserGridEditorOptions;

  /**
   * Used by Editor to indicate in loading state or not
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() editorLoading = false;

  /**
   * Used by Grid to indicate in loading state or not
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() gridLoading = false;

  /**
   * Used by Grid to indicate if row details are loading
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() gridDetailsLoading = false;

  /**
   * Template used by the Grid that will define what the details of each row will look like on Desktopn
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() rowDetailTemplate: TemplateRef<any>; // or change template inputs to @ViewChild() as pass into options.. but more work for dev..

  /**
   * Template used by the Grid that will define what the details of each row will look like on Mobile devices
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() mobileRowDetailTemplate: TemplateRef<any>;

  /**
   * Template that defines what a popover looks like
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() popoverTemplate: TemplateRef<any>;

  /**
   * Template that indicates what the menu in the browser tile header will look like
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() menuTemplate: TemplateRef<any>;

  /**
   * Template that indicates what the action in the browser tile header will look like
   *
   * @type {TemplateRef<any>}
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Input() headerActionTemplate: TemplateRef<any>;

  // OUTPUTS FROM BROWSER //

  /**
   * Emitted when the Add button is clicked in the browser tile
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onAdd = new EventEmitter();

  /**
   * Emitted when the DeExpand button is clicked in the browser tile
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onDeExpand = new EventEmitter();

  /**
   * Emitted when the Close button is clicked in the browser tile
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onClose = new EventEmitter();

  // OUTPUTS FROM GRID //

  /**
   * Emits the selected grid row when the Delete icon is clicked
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onDelete = new EventEmitter<any>();

  /**
 * Emits the selected grid row when the Delete icon is clicked
 *
 * @memberof AmcsBrowserGridEditorLayoutComponent
 */
  @Output() onUndelete = new EventEmitter<any>();

  /**
   * Emits the selected grid row when the Edit icon is clicked
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onEdit = new EventEmitter<any>();

  /**
   * Emitted when a row is expanded
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onRowExpanded = new EventEmitter<number>();

  /**
  * Emitted when rows are selected
  *
  * @memberof AmcsBrowserGridEditorLayoutComponent
  */
  @Output() onRowsSelected = new EventEmitter<number[]>();

  /**
   * Emitted when a link on a row has been clicked
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onLinkClicked = new EventEmitter<number>();

  /**
   * Emitted when a link on a row has been clicked, gives more information about which column was clicked
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onLinkClickedExtended = new EventEmitter<IColumnLinkClickedEvent>();

  /**
   * Emitted when the value of a amcs-select in a column was changed
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onValueChanged = new EventEmitter<ValueChangedEvent>();

  /**
   * Emitted when the sorting direction of a column was changed
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onSortChanged = new EventEmitter<SortChangedEvent>();

  /**
   * Emitted when the filter was changed
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onFilterChanged = new EventEmitter<string>();

  /**
   * Emitted when a column has been re-ordered
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onColumnPositionsChanged = new EventEmitter<ColumnPositionChangedEvent[]>();

  /**
   * Emitted when the page was changed
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onSelectedPageChanged = new EventEmitter<number>();

  /**
   * Emitted when the action column menu is opened
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onOpenActions = new EventEmitter<OpenActionMenuEvent>();

  /**
   * Emitted when a popover is clicked
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onPopoverClicked = new EventEmitter<ISelectorItem>();

  /**
   * Emitted when a menu item in a row is clicked
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onMenuItemClicked = new EventEmitter<MenuItemClickedEvent>();

  /**
   * Emitted when a menu item in a row is clicked
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() showDeletedRecords = new EventEmitter<boolean>();

  // OUTPUTS FROM EDITOR //

  /**
   * Emitted when a form is valid, not pristine and save is clicked
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onSave = new EventEmitter();

  /**
   * Emitted when a editor is canceled
   *
   * @memberof AmcsBrowserGridEditorLayoutComponent
   */
  @Output() onCancel = new EventEmitter();

  BrowserDisplayMode = BrowserDisplayMode;

  constructor(
    private readonly translationsService: SharedTranslationsService,
    private readonly modalService: AmcsModalService,
    elRef: ElementRef,
    renderer: Renderer2,
    viewContainer: ViewContainerRef
  ) {
    super(elRef, renderer, viewContainer);
  }

  private isAddEnabled = false;

  ngOnChanges(changes: SimpleChanges): void {
    super.ngOnChanges(changes);
    if (changes['form']) {
      this.setUpEditMode();
      if (this.isAddEnabled) {
        this.options.browserOptions = { ...this.options.browserOptions, enableAdd: !this.form };
      }
    }
    if (changes['options']) {
      this.validateOptions();
      this.calculateVerticalLoadingPadding();
      this.setUpEditLinkColumn();
      this.options.gridOptions.rowDetailTemplate = this.rowDetailTemplate; // yes this looks weird but makes for easier syntax in layout
      this.options.gridOptions.mobileRowDetailTemplate = this.mobileRowDetailTemplate;
      this.options.gridOptions.popoverTemplate = this.popoverTemplate;
      this.options.browserOptions.menuTemplate = this.menuTemplate;
      this.options.browserOptions.headerActionTemplate = this.headerActionTemplate;
      this.isAddEnabled = this.options.browserOptions.enableAdd;
    }
  }

  /**
   * Handles edit/delete and any custom buttons added
   */
  handleMenuItemClicked(data: MenuItemClickedEvent) {
    if (data.action.id === BrowserGridEditorActionButtonEnum.Edit) {
      this.onEdit.emit(data.row);
    } else if (data.action.id === BrowserGridEditorActionButtonEnum.Undelete) {
      this.onUndelete.emit(data.row);
    } else if (data.action.id === BrowserGridEditorActionButtonEnum.Delete) {
      this.handleDelete(data.row);
    } else {
      this.onMenuItemClicked.emit(data);
    }
  }

  /**
   * Fires onAdd after checking for unsaved changes
   */
  handleAdd() {
    if (this.form && !this.form.htmlFormGroup.pristine) {
      this.showUnsavedChangesModel().subscribe(() => {
        this.onAdd.emit();
      });
    } else {
      this.onAdd.emit();
    }
  }

  /**
   * Fires onClose after checking for unsaved changes
   */
  handleClose() {
    if (this.form && !this.form.htmlFormGroup.pristine) {
      this.showUnsavedChangesModel().subscribe(() => {
        this.onClose.emit();
      });
    } else {
      this.onClose.emit();
    }
  }

  /**
   * Fires onDeExpand after checking for unsaved changes
   */
  handleDexpand() {
    if (this.form && !this.form.htmlFormGroup.pristine) {
      this.showUnsavedChangesModel().subscribe(() => {
        this.onDeExpand.emit();
      });
    } else {
      this.onDeExpand.emit();
    }
  }

  /**
   * Fires onCancel after checking for unsaved changes
   */
  handleCancel(): void {
    if (this.form && !this.form.htmlFormGroup.pristine) {
      this.showUnsavedChangesModel().subscribe(() => {
        this.cancelAndUnselectRows();
      });
    } else {
      this.cancelAndUnselectRows();
    }
  }

  /**
   * * Fires onLinkClickedExtended or onEdit depending on the selected column matching the given editLinkColumnKey
   * @param event The IColumnLinkClickedEvent data
   */
  handleOnLinkClickedExtended(event: IColumnLinkClickedEvent): void {
    if (this.options.editLinkColumnKey && event.key === this.options.editLinkColumnKey) {
      this.onEdit.emit(event.item);
    } else {
      this.onLinkClickedExtended.emit(event);
    }
  }

  /**
   * Ensures the options passed are valid.
   */
  private validateOptions(): void {
    if (this.options.gridOptions.allowMultiSelect) {
      throw new Error('Grid Multi-Select not supported in AmcsBrowserGridEditorLayoutComponent');
    }
    if (this.options.browserOptions.enableEdit) {
      throw new Error('Browser Edit button not supported in AmcsBrowserGridEditorLayoutComponent. Use pencil icon on the grid to edit.');
    }
    if (this.options.browserOptions.enableDelete) {
      throw new Error(
        'Browser Delete button not supported in AmcsBrowserGridEditorLayoutComponent. Use trashbin icon on grid row to delete.'
      );
    }
    if (!this.options.browserOptions.removePadding) {
      throw new Error('Browser removePadding must be true to ensure consistent padding for grid and editor.');
    }
    if (this.options.gridOptions.actionColumnTemplate) {
      throw new Error(
        'Custom actionColumnTemplate not supported in AmcsBrowserGridEditorLayoutComponent. Use gridOption\'s actionColumnButtons to customise.'
      );
    }
    if (this.options.editLinkColumnKey && !this.options.gridOptions.columns.find((x) => x.key === this.options.editLinkColumnKey)) {
      throw new Error(`No column key matches given editLinkColumnKey: ${this.options.editLinkColumnKey}.`);
    }
  }

  /**
   * Shows confirmation modal before firing the onDelete output
   * @param row The row to delete
   */
  private handleDelete(row: any): void {
    const dialog = this.modalService.createModal({
      template: this.translationsService.getTranslation('browserGridEditorLayout.deleteMessage'),
      title: this.translationsService.getTranslation('browserGridEditorLayout.deleteTitle'),
      type: 'confirmation',
    });
    dialog
      .afterClosed()
      .pipe(
        take(1),
        filter((x) => x === true)
      )
      .subscribe((result: boolean) => {
        if (result) {
          this.onDelete.emit(row);
        }
      });
  }

  /**
   * Edit mode controls whether the 'New' text is displayed in the editor title
   */
  private setUpEditMode(): void {
    if (!this.form) {
      this.options.browserOptions = { ...this.options.browserOptions, isNewMode: false };
      this.options.browserOptions = { ...this.options.browserOptions, isEditMode: false };
      return;
    }
    const primaryKey: FormControl = this.form.getPrimaryKeyControl();
    this.options.browserOptions = { ...this.options.browserOptions, isNewMode: !isTruthy(primaryKey.value) };
    this.options.browserOptions = { ...this.options.browserOptions, isEditMode: isTruthy(primaryKey.value) };
  }

  /**
   * Calculates the amount of top padding needed to keep the loading animation vertically centered
   */
  private calculateVerticalLoadingPadding(): void {
    this.verticalLoadingPadding = Math.max(0, this.options.editorMinHeight / 2 - 70);
  }

  /**
   * Cancel hides the editor and unselects all items in grid
   */
  private cancelAndUnselectRows(): void {
    this.options.gridOptions.data?.forEach((element) => {
      element.isSelected = false;
    });
    this.options.gridOptions = { ...this.options.gridOptions };
    this.onCancel.emit();
  }

  /**
   * Creates an unsaved changes confirmation modal
   */
  private showUnsavedChangesModel(): Observable<boolean> {
    return this.modalService
      .createModal({
        template: this.translationsService.getTranslation('browserGridEditorLayout.unsavedChangedMessage'),
        title: this.translationsService.getTranslation('browserGridEditorLayout.unsavedChangedTitle'),
        type: 'confirmation',
      })
      .afterClosed()
      .pipe(
        take(1),
        filter((x) => x === true)
      );
  }

  /**
   * If editLinkColumnKey is supplied set that column as a link column and remove the edit action (link column will initiate edit)
   */
  private setUpEditLinkColumn(): void {
    if (!this.options.editLinkColumnKey) {
      return;
    }
    const linkColumn: GridColumnConfig = this.options.gridOptions.columns.find((x) => x.key === this.options.editLinkColumnKey);
    linkColumn.withType(GridColumnType.link);
    if (
      this.options.gridOptions.actionColumnButtons &&
      this.options.gridOptions.actionColumnButtons.find((x) => x.id === BrowserGridEditorActionButtonEnum.Edit)
    ) {
      this.options.gridOptions.actionColumnButtons = this.options.gridOptions.actionColumnButtons.filter(
        (x) => x.id !== BrowserGridEditorActionButtonEnum.Edit
      );
    }
  }
}
