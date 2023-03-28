import { AfterViewInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2, TemplateRef, ViewChild } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ColumnCustomiserService } from '@core-module/services/column-customiser/column-customiser.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ITableItem } from '@shared-module/models/itable-item.model';
import { IColumnLinkClickedEvent } from '../amcs-grid-wrapper/column-link-clicked-event.interface';
import { GridColumnAlignment } from '../amcs-grid/grid-column-alignment.enum';
import { GridColumnType } from '../amcs-grid/grid-column-type.enum';

@Component({
  selector: 'app-amcs-grid-column-customiser',
  templateUrl: './amcs-grid-column-customiser.component.html',
  styleUrls: ['./amcs-grid-column-customiser.component.scss'],
  providers: [ColumnCustomiserService.providers]
})
export class AmcsGridColumnCustomiserComponent extends AutomationLocatorDirective implements OnInit, AfterViewInit {

  @Input() accordianTitle: string;
  @Input() columnListMaxHeight = 500;
  @ViewChild('defaultAccordianTemplate') defaultAccordianTemplate: TemplateRef<any>;
  // Optional, will override the default
  @Input() customAccordianTemplate: TemplateRef<any> = null;
  @Input() isSecondaryUi = false;
  @Input() disableSelection = false;

  @Output() linkClickedExtended = new EventEmitter<IColumnLinkClickedEvent>();

  ColumnType = GridColumnType;
  GridColumnAlignment = GridColumnAlignment;
  showPopover = false;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public service: ColumnCustomiserService) {
    super(elRef, renderer);
  }

  togglePopover() {
    this.showPopover = !this.showPopover;
    if (this.showPopover) {
      this.service.buildTiles();
    }
  }

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    this.service.initialise(isTruthy(this.customAccordianTemplate) ? this.customAccordianTemplate : this.defaultAccordianTemplate);
  }

  hidePopover() {
    this.showPopover = false;
    this.service.orderColumns();
  }

  onLinkClicked(key: string, item: ITableItem) {
    this.linkClickedExtended.emit({ key, item });
  }
}
