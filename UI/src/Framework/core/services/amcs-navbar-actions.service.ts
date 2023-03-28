import { AmcsNavBarActionItem } from '@shared-module/models/amcs-nav-bar-action-item.model';
import { Subject } from 'rxjs';
import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { Router } from '@angular/router';

@Injectable()
export class AmcsNavBarActionsService extends BaseService {
  maxFavouritesAllowed = 3;
  actionItems: AmcsNavBarActionItem[] = null;
  selectedKeys$ = new Subject<string[]>();
  isOpen = false;

  constructor(private router: Router) {
    super();
  }

  private selectedKeyStack = [];

  configure(actionItems: AmcsNavBarActionItem[]) {
    this.actionItems = actionItems;
    this.actionItems.forEach(x => {
      if (x.isFavourite) {
        this.selectedKeyStack.push(x.key);
      }
    });
  }

  navigate(actionItem: AmcsNavBarActionItem) {
    this.hideMoreActions();

    this.router.navigate(actionItem.url);
  }

  toggleFavourite(actionItem: AmcsNavBarActionItem) {
    actionItem.isFavourite = !actionItem.isFavourite;

    if (actionItem.isFavourite) {
      if (this.selectedKeyStack.length === this.maxFavouritesAllowed) {
        const lastFavouriteKey = this.selectedKeyStack.pop();
        this.actionItems.filter(item => item.key === lastFavouriteKey).map(item => item.isFavourite = false);
      }

      this.selectedKeyStack.push(actionItem.key);
      this.selectedKeys$.next(this.selectedKeyStack);
    } else {
      this.selectedKeyStack = this.selectedKeyStack.filter(key => key !== actionItem.key);
      this.selectedKeys$.next(this.selectedKeyStack);
    }
  }

  toggleMoreActions() {
    this.isOpen = !this.isOpen;
  }

  hideMoreActions() {
    this.isOpen = false;
  }
}
