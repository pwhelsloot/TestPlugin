import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ActionColumnButton } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-button';
import { BrowserGridEditorActionButtonEnum } from '@shared-module/components/layouts/amcs-browser-grid-editor-layout/browser-grid-editor-action-button.enum';
import { GridOptions } from '@shared-module/components/layouts/amcs-browser-grid-layout/grid-options-model';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';
import { BrowserEditorOptions } from '../amcs-browser-grid-layout/browser-editor-options.model';
import { BrowserDisplayMode } from './browser-display-mode.enum';

export class BrowserGridEditorOptions {
  editorMinHeight = 90;
  editLinkColumnKey: string; // the key of the column to use as an edit hyperlink

  gridOptions = new GridOptions();
  browserOptions = new BrowserEditorOptions();
  editorOptions = new FormOptions();

  constructor() {
    this.gridOptions.showFilter = false;
    this.gridOptions.actionColumnWidth = 58;
    this.gridOptions.displayActionColumnsButtonsInline = true;
    // Ids 998 and 999 will always be reserved for edit/delete in this layout (trying to avoid any clashing if uses override the default actions)
    this.gridOptions.actionColumnButtons = new Array<ActionColumnButton>(
      {
        icon: 'fas fa-pencil-alt',
        isDisabled: false,
        id: BrowserGridEditorActionButtonEnum.Edit,
      },
      {
        icon: 'fas fa-trash-alt',
        isDisabled: false,
        id: BrowserGridEditorActionButtonEnum.Delete,
      }
    );
    this.browserOptions.removePadding = true;
    this.browserOptions.enableAdd = true;
    this.browserOptions.enableClose = true;
  }

  private _displayMode = BrowserDisplayMode.Standard;
  get displayMode(): BrowserDisplayMode {
    return this._displayMode;
  }
  set displayMode(value: BrowserDisplayMode) {
    this._displayMode = value;
    if (isTruthy(this.browserOptions)) {
      this.browserOptions.displayMode = value;
    }
  }
}
