import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-amcs-loading-view',
  templateUrl: './amcs-loading-view.component.html',
  styleUrls: ['./amcs-loading-view.component.scss']
})
export class AmcsLoadingViewComponent {
  @Input('loading') loading = false;
  @Input('color') color = '#D4D4D4';
  @Input('size') size = 40;
  @Input('containerSize') containerSize = 90;
  @Input('verticallyCenter') verticallyCenter = false;
}
