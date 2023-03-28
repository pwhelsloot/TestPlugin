import { Component, Input } from '@angular/core';
import { InnerTileServiceUI } from '@core-module/services/ui/inner-tile.service.ui';

@Component({
    selector: 'app-amcs-custom-inner-tile',
    templateUrl: './amcs-custom-inner-tile.component.html',
    styleUrls: ['./amcs-custom-inner-tile.component.scss']
})
export class AmcsCustomInnerTileComponent {

    @Input('isForm') isForm = false;
    @Input('loading') loading = false;
    @Input('isChild') isChild = false;
    @Input('bodyMinHeight') bodyMinHeight: number;
    @Input() isDynamicHeight = false;
    @Input() noPadding = false;

    constructor(public uiService: InnerTileServiceUI) { }

    tilesChanged() {
        setTimeout(() => {
            this.uiService.innerTilesChanged.next();
        }, 0);
    }
}
