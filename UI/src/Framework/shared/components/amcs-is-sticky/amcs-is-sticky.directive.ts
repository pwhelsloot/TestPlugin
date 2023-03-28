import { AfterViewInit, Directive, ElementRef, Input, OnDestroy, Renderer2 } from '@angular/core';
import { ResizeObserver } from '@juggle/resize-observer';
import { fromEvent, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';

/**
 * @todo Move to directive folder
 */
@Directive({
  selector: '[appIsSticky]',
  exportAs: 'isSticky'
})
export class AmcsIsStickyDirective implements AfterViewInit, OnDestroy {

  @Input() stickyOffset = 0;
  @Input() stickyPosition: 'top' | 'bottom' = 'bottom';

  get isElementSticky(): boolean {
    return this._isElementSticky;
  }

  constructor(private element: ElementRef, private renderer: Renderer2) { }

  private _isElementSticky = false;

  private windowResizeSubscription: Subscription;
  private scrollSubscription: Subscription;

  // See https://developer.mozilla.org/en-US/docs/Web/API/ResizeObserver for more information about ResizeObserver
  private resizeObserver: ResizeObserver;

  private parentElement: HTMLElement;
  private currentElement: HTMLElement;

  private offsetPadding = 0;
  private initialPaddingBottom = 0;

  ngAfterViewInit() {
    this.parentElement = this.element.nativeElement.parentElement as HTMLElement;
    this.currentElement = this.element.nativeElement as HTMLElement;
    this.initialPaddingBottom = parseFloat(window.getComputedStyle(this.parentElement).paddingBottom);

    // If there's no scrollbar, then there's no point in making anything sticky
    if (!this._isElementSticky && document.body.scrollHeight + this.offsetPadding > window.innerHeight) {
      this.setInitialProperties();
      this.setupSubscriptions();
      this._isElementSticky = true;
    }

    this.resizeObserver = new ResizeObserver(entries => {
      entries.forEach(entry => {
        const temp = entry.contentRect.bottom - this.currentElement.clientHeight;
        if (!this._isElementSticky && temp > window.innerHeight) {
          this.setInitialProperties();
          this.setupSubscriptions();
          this._isElementSticky = true;
        } else if (this._isElementSticky && window.innerHeight > temp) {
          this.disableSticky();
          this._isElementSticky = false;
        }
      });
    });

    this.resizeObserver.observe(this.parentElement);
  }

  ngOnDestroy() {
    if (this.windowResizeSubscription) {
      this.windowResizeSubscription.unsubscribe();
    }

    if (this.scrollSubscription) {
      this.scrollSubscription.unsubscribe();
    }

    this.resizeObserver.disconnect();
  }

  private setInitialProperties() {
    const newPadding = this.initialPaddingBottom + this.currentElement.offsetHeight;
    this.renderer.setStyle(this.parentElement, 'padding-bottom', `${newPadding}px`);

    this.renderer.setStyle(this.currentElement, this.stickyPosition, `${this.stickyOffset}px`);
    this.renderer.setStyle(this.currentElement, 'position', 'fixed');
    this.renderer.setStyle(this.currentElement, 'min-width', `${this.parentElement.offsetWidth}px`);
  }

  private setupSubscriptions() {
    this.windowResizeSubscription = fromEvent(window, 'resize').pipe(debounceTime(50)).subscribe(() => {
      this.renderer.setStyle(this.currentElement, 'min-width', `${this.parentElement.offsetWidth}px`);
    });

    this.scrollSubscription = fromEvent(window, 'scroll').pipe(debounceTime(10)).subscribe(() => {
      const currentPosition = window.getComputedStyle(this.currentElement).position;
      const windowHeight = window.innerHeight;
      const parentBottom = this.parentElement.getBoundingClientRect().bottom;

      const newPosition = parentBottom < windowHeight - this.stickyOffset ? 'unset' : 'fixed';
      if (newPosition !== currentPosition) {
        if (newPosition === 'fixed') {
          const newPadding = this.initialPaddingBottom + this.currentElement.offsetHeight;
          this.renderer.setStyle(this.parentElement, 'padding-bottom', `${newPadding}px`);
        } else {
          this.renderer.setStyle(this.parentElement, 'padding-bottom', `${this.initialPaddingBottom}px`);
        }
        this.renderer.setStyle(this.currentElement, 'position', newPosition);
      }
    });
  }

  private disableSticky() {
    this.windowResizeSubscription.unsubscribe();
    this.scrollSubscription.unsubscribe();
    this.renderer.setStyle(this.parentElement, 'padding-bottom', `${this.initialPaddingBottom}px`);
    this.renderer.removeStyle(this.currentElement, 'min-width');
    this.renderer.setStyle(this.currentElement, 'position', 'unset');
  }
}
