import { Component, Input } from '@angular/core';

/**
 * @deprecated use the app-dashboard-tile instead
 */
@Component({
    selector: 'app-amcs-report-tile',
    templateUrl: './amcs-report-tile.component.html',
    styleUrls: ['./amcs-report-tile.component.scss']
})
export class AmcsReportTileComponent {

    @Input('loading') loading = false;
    @Input() portletCustomClass = '';
}
