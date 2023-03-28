import { MediaMatcher } from '@angular/cdk/layout';
import { Component, ElementRef, EventEmitter, Input, OnDestroy, OnInit, Output, Renderer2, TemplateRef, ViewChild } from '@angular/core';
import { MatSidenav } from '@angular/material/sidenav';
import { NavigationEnd, Router } from '@angular/router';
import { MediaSizes } from '@core-module/models/media-sizes.constants';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';
import { filter } from 'rxjs/operators';
import { AmcsNavBarMenuItem } from './amcs-navbar-item.model';

@Component({
  selector: 'app-amcs-navbar',
  templateUrl: './amcs-navbar.component.html',
  styleUrls: ['./amcs-navbar.component.scss']
})
export class AmcsNavbarComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {

  @Input() titleTemplate: TemplateRef<any> = null;
  @Input() afterTitleTemplate: TemplateRef<any> = null;
  @Input() menuItems: AmcsNavBarMenuItem[] = [];
  @Input() titleMenuTemplate: TemplateRef<any> = null;
  @Input() showHamburgerMenu = true;
  @Input() showReturn = false;
  @ViewChild('sidebar') sidebar: MatSidenav;
  @Output() titleClicked = new EventEmitter();
  @Output() onReturn = new EventEmitter();
  @Input() customTop = 50;
  @Input() menuFixedGap = 100;

  isMobile: boolean;
  isSidebarOpen = false;
  headerMenuItems: AmcsNavBarMenuItem[] = [];

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    private media: MediaMatcher, private router: Router) {
    super(elRef, renderer);
  }

  private routeSubscription: Subscription;

  private mobileQuery: MediaQueryList;
  private _mobileQueryListener: () => void;
  private isToggling = false;

  ngOnInit() {
    this.routeSubscription = this.router.events
      .pipe(filter((event: any) => event instanceof NavigationEnd))
      .subscribe((event: any) => {
        this.menuItems.forEach(item => {
          item.isSelected = false;
        });
        this.headerMenuItems.forEach(item => {
          item.isSelected = false;
        });
        this.menuItems.forEach(item => {
          item.isSelected = (item.url === event.url);
        });
        this.headerMenuItems.forEach(item => {
          item.isSelected = (item.url === event.url);
        });
      });

    this.menuItems.forEach(item => {
      item.isSelected = (item.url === this.router.url);
    });

    this.headerMenuItems = this.menuItems.filter(x => x.showOnHeader);

    this.mobileQuery = this.media.matchMedia('(max-width: ' + MediaSizes.small.toString() + 'px)');
    this._mobileQueryListener = () => {
      this.isMobile = this.mobileQuery.matches;
    };
    this.mobileQuery.addListener(this._mobileQueryListener);
    this.isMobile = this.mobileQuery.matches;
  }

  closeSidebar() {
    this.sidebar.close();
    this.isSidebarOpen = false;
  }

  toggleSidebar() {
    this.isToggling = true;
    this.sidebar.toggle();
    this.isSidebarOpen = this.sidebar.opened;
  }

  return() {
    this.onReturn.emit();
  }

  ngOnDestroy() {
    this.routeSubscription.unsubscribe();
    this.mobileQuery.removeListener(this._mobileQueryListener);
  }

  navigate(item: AmcsNavBarMenuItem) {
    this.menuItems.forEach(element => {
      element.isSelected = false;
    });
    item.isSelected = true;
    if (this.checkContainsQueryString(item.url)) {
      this.router.navigateByUrl(item.url);
    } else {
      this.router.navigate([item.url]);
    }
    this.closeSidebar();
  }

  onClickOutside() {
    if (!this.isToggling && this.sidebar.opened) {
      this.closeSidebar();
    }

    if (this.isToggling) {
      this.isToggling = false;
    }
  }

  private checkContainsQueryString(input: string) {
    return input.indexOf('?') !== -1;
  }
}
