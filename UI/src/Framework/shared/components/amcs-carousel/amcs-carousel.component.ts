import { ChangeDetectionStrategy, Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges, TemplateRef } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-carousel',
  templateUrl: './amcs-carousel.component.html',
  styleUrls: ['./amcs-carousel.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AmcsCarouselComponent extends AutomationLocatorDirective implements OnInit, OnChanges {

  @Input('items') items: any;
  @Input('itemsPerSlide') itemsPerSlide: number;
  @Input('slideTemplate') slideTemplate: TemplateRef<any>;
  @Output('slideChanged') slideChanged: EventEmitter<any> = new EventEmitter<any>();
  slides = [];

  ngOnInit() {
    this.getNumberOfSlides(this.items, this.itemsPerSlide);
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['items'] && this.items) {
      this.getNumberOfSlides(this.items, this.itemsPerSlide);
    }
  }

  getNumberOfSlides(items: any, count: number) {
    this.slides = [];
    for (let i = 0, len = items.length; i < len; i += count) {
      this.slides.push(items.slice(i, i + count));
    }
  }

  onSlideChange(index: number) {
    this.slideChanged.next(this.slides[index ?? 0]);
  }
}
