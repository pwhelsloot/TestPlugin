import { MediaMatcher } from '@angular/cdk/layout';
import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, Output, Renderer2, SimpleChanges } from '@angular/core';
import { MediaSizes } from '@core-module/models/media-sizes.constants';
import { AmcsNavBarActionsService } from '@core-module/services/amcs-navbar-actions.service';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { AmcsNavBarActionItem } from '@shared-module/models/amcs-nav-bar-action-item.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-amcs-navbar-actions',
  templateUrl: './amcs-navbar-actions.component.html',
  styleUrls: ['./amcs-navbar-actions.scss'],
  providers: [AmcsNavBarActionsService]
})
export class AmcsNavBarActionsComponent extends AutomationLocatorDirective implements OnChanges, OnDestroy {

  @Input() actionItems: AmcsNavBarActionItem[];

  @Output() favouriteActions = new EventEmitter<string[]>();

  @Input() handleNavigationInternally = true;

  @Output() onActionSelected = new EventEmitter<AmcsNavBarActionItem>();

  mobileQuery: MediaQueryList;

  moreActionsText: string = null;
  addToFavouritesText: string = null;

  constructor(
    public service: AmcsNavBarActionsService,
    private sharedTranslationsService: SharedTranslationsService,
    public media: MediaMatcher,
    protected elRef: ElementRef, protected renderer: Renderer2
  ) {
    super(elRef, renderer);
    this.translationSubscription = this.sharedTranslationsService.translations.subscribe(translations => {
      this.moreActionsText = translations['navbarActions.moreActions'];
      this.addToFavouritesText = translations['navbarActions.addToFavourites'];
    });
  }

  private selectedKeysSubscription: Subscription;
  private translationSubscription: Subscription;

  ngOnChanges(simpleChanges: SimpleChanges) {
    if (simpleChanges['actionItems'] && simpleChanges['actionItems'].currentValue && simpleChanges['actionItems'].firstChange) {
      this.service.configure(this.actionItems);
      this.selectedKeysSubscription = this.service.selectedKeys$.subscribe(selectedKeys => {
        this.favouriteActions.emit(selectedKeys);
      });

      this.mobileQuery = this.media.matchMedia('(max-width: ' + MediaSizes.thousandOneHundredForty.toString() + 'px)');
    }
  }

  navigate(actionItem: AmcsNavBarActionItem) {
    if (!this.handleNavigationInternally) {
      this.service.hideMoreActions();
      this.onActionSelected.emit(actionItem);
    } else {
      this.service.navigate(actionItem);
    }
  }

  ngOnDestroy() {
    if (this.selectedKeysSubscription) {
      this.selectedKeysSubscription.unsubscribe();
    }
    this.translationSubscription.unsubscribe();
  }
}
