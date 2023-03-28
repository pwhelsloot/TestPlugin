import { Component, AfterViewInit, OnDestroy, Input, OnChanges, SimpleChanges } from '@angular/core';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';

@Component({
  selector: 'app-amcs-inner-container',
  templateUrl: './amcs-inner-container.component.html',
  styleUrls: ['./amcs-inner-container.component.scss']
})
export class AmcsInnerContainerComponent implements AfterViewInit, OnDestroy, OnChanges {

  className: string;
  @Input() loading: boolean;

  constructor(public uiService: InnerTileServiceUI) {
    this.className = 'amcs-inner-container container-group-' + this.uiService.getCurrentGroupNumber().toString();
  }

  ngOnChanges(change: SimpleChanges) {
    // If loading has finished, screen is ready so fire notification
    if (change && change['loading'] && !change['loading'].currentValue) {
      this.uiService.innerTilesChanged.next();
    }
  }

  ngAfterViewInit() {
    this.uiService.innerTilesChanged.next();
  }

  ngOnDestroy() {
    this.uiService.innerTilesChanged.next();
  }
}
