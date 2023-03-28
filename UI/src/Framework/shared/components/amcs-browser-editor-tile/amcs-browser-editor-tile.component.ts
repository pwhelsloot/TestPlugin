import {
  ChangeDetectionStrategy,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  Renderer2,
  SimpleChanges
} from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { BrowserDisplayMode } from '../layouts/amcs-browser-grid-editor-layout/browser-display-mode.enum';
import { BrowserEditorOptions } from '../layouts/amcs-browser-grid-layout/browser-editor-options.model';

@Component({
  selector: 'app-amcs-browser-editor-tile',
  templateUrl: './amcs-browser-editor-tile.component.html',
  styleUrls: ['./amcs-browser-editor-tile.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AmcsBrowserEditorTileComponent extends AutomationLocatorDirective implements OnInit, OnChanges {

  @Input() options: BrowserEditorOptions;
  @Input() loading = false;
  @Output() onDeExpand = new EventEmitter();
  @Output() onClose = new EventEmitter();
  @Output() onAdd = new EventEmitter();
  @Output() onEdit = new EventEmitter();
  @Output() onDelete = new EventEmitter();

  hasButtons = false;
  BrowserDisplayMode = BrowserDisplayMode;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }

  ngOnInit() {
    this.hasButtons = this.options.enableAdd || this.options.enableEdit || this.options.enableDelete || this.options.menuTemplate != null;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['options']) {
      this.getButtonClasses();
    }
  }

  getButtonClasses() {
    this.options?.buttons?.forEach((button) => {
      button.cssClass = 'btn noMargin';
      if (button.icon) {
        button.cssClass = `${button.cssClass} button.icon`;
      }
      if (button.link) {
        button.cssClass = `${button.cssClass} button.link`;
      }
      if (button.extraClasses) {
        button.cssClass = `${button.cssClass} ${button.extraClasses}`;
      }
    });
  }

}
