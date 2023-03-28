import { Component, ElementRef, Input, Renderer2 } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

@Component({
  selector: 'app-amcs-expand-collapse',
  templateUrl: './amcs-expand-collapse.component.html',
  styleUrls: ['./amcs-expand-collapse.component.scss']
})
export class AmcsExpandCollapseComponent extends AutomationLocatorDirective {
  @Input('isExpanded') isExpanded: boolean;
  @Input() isSecondaryStyle = false;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }
}
