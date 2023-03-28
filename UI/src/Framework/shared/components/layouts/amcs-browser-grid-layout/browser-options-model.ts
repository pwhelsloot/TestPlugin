import { TemplateRef } from '@angular/core';
import { BrowserButton } from './browser-button-model';

export class BrowserOptions {
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
  buttons: BrowserButton[];
  menuTemplate: TemplateRef<any>;
  headerActionTemplate: TemplateRef<any>;
  compName: string;
}
