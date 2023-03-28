import { TemplateRef } from '@angular/core';
import { BrowserDisplayMode } from '../amcs-browser-grid-editor-layout/browser-display-mode.enum';
import { BrowserButton } from './browser-button-model';

export class BrowserEditorOptions {
  title: string;
  saveButtonText: string;
  buttonText: string;
  enableAdd = false;
  enableEdit = false;
  enableDelete = false;
  enableDeExpand = false;
  enableClose = false;
  enableFormActions = false;
  isChild = false;
  removePadding = false;
  isNewMode = false;
  isEditMode = false;
  buttons: BrowserButton[];
  menuTemplate: TemplateRef<any>;
  headerActionTemplate: TemplateRef<any>;
  compName: string;
  displayMode = BrowserDisplayMode.Standard;
}
