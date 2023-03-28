import { Component, OnDestroy, ElementRef, HostListener } from '@angular/core';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';
import { AmcsInnerTileHelper } from '../helpers/amcs-inner-tile-helper';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-amcs-inner-container-group',
  templateUrl: './amcs-inner-container-group.component.html',
  styleUrls: ['./amcs-inner-container-group.component.scss']
})
export class AmcsInnerContainerGroupComponent implements OnDestroy {

  constructor(el: ElementRef, private uiService: InnerTileServiceUI) {
    this.tileHelper = new AmcsInnerTileHelper(el, this.uiService.getNextGroupNumber());
    this.tileChangedSubscription = this.uiService.innerTilesChanged.subscribe(() => {
      this.tileHelper.setPadding();
    });
  }

  private tileHelper: AmcsInnerTileHelper;
  private tileChangedSubscription: Subscription;

  ngOnDestroy() {
    this.tileChangedSubscription.unsubscribe();
  }

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.tileHelper.setPadding();
  }
}
