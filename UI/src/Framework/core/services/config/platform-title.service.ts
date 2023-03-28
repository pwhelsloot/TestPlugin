import { Injectable } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { BaseService } from '../base.service';
import { CoreAppMessagingAdapter } from './core-app-messaging.service';

@Injectable({ providedIn: 'root' })
export class PlatformTitleService extends BaseService {
  constructor(private coreAppMessagingService: CoreAppMessagingAdapter, private titleService: Title) {
    super();
  }

  setTitle(title: string) {
    this.coreAppMessagingService.sendAppChangeTitleRequest(title);
    this.titleService.setTitle(title);
  }

  getTitle() {
    this.titleService.getTitle();
  }
}
