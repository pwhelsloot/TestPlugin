import { Component, OnDestroy, NgZone } from '@angular/core';
import { MediaMatcher } from '@angular/cdk/layout';
import { MediaSizes } from '@core-module/models/media-sizes.constants';

@Component({
  selector: 'app-template-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss'],
  providers: [],
})
export class TemplateHeaderComponent implements OnDestroy {
  mobileQuery: MediaQueryList;

  constructor(media: MediaMatcher, private zone: NgZone) {
    this.mobileQuery = media.matchMedia('(max-width: ' + MediaSizes.small.toString() + 'px)');

    this._mobileQueryListener = () => {
      // This re-renders the page once per media change
      this.zone.run(() => {});
    };
    this.mobileQuery.addListener(this._mobileQueryListener);
  }

  private _mobileQueryListener: () => void;

  ngOnDestroy() {
    this.mobileQuery.removeListener(this._mobileQueryListener);
  }
}
