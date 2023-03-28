import { Component, OnInit, OnDestroy } from '@angular/core';
import { HeaderService } from '@core-module/services/header/header.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-template-page-layout',
  templateUrl: './page-layout.component.html',
  styleUrls: ['./page-layout.component.scss'],
})
export class TemplatePageLayoutComponent implements OnInit, OnDestroy {
  isSearchClicked: boolean;

  constructor(private headerService: HeaderService) {
    // Bit of a hack but timeout here is because of padding being added too quickly, need to wait till 150ms animation is done.
    this.isSearchedClickedSub = this.headerService.isSearchedClicked.subscribe((value: boolean) => {
      if (!value) {
        setTimeout(() => {
          this.isSearchClicked = value;
        }, 155);
      } else {
        this.isSearchClicked = value;
      }
    });
  }

  private isSearchedClickedSub: Subscription;

  ngOnInit() {}

  ngOnDestroy() {
    this.isSearchedClickedSub.unsubscribe();
  }
}
