import { Directive, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { environment } from '@environments/environment';

@Directive({ selector: '[appAutomationLocator]' })
export class AutomationLocatorDirective implements OnInit {
  @Input() compName: string;
  @Input() uniqueKey: string;
  @Input() formControlName: string;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2) {}


  ngOnInit() {
    // set data-testId base on env variable
    if (environment.isAutomationEnabled) {
      this.renderer.setAttribute(
        this.elRef.nativeElement,
        'data-testId',
        this.getDataTestId(this.compName, this.formControlName, this.uniqueKey)
      );
    }

    // remove unnecessary automation attributes
    this.renderer.removeAttribute(this.elRef.nativeElement, 'appautomationlocator');
    this.renderer.removeAttribute(this.elRef.nativeElement, 'ng-reflect-comp-name');
    this.renderer.removeAttribute(this.elRef.nativeElement, 'ng-reflect-form-control-name');
    this.renderer.removeAttribute(this.elRef.nativeElement, 'ng-reflect-dummy-form-control-name');
    this.renderer.removeAttribute(this.elRef.nativeElement, 'ng-reflect-unique-key');
  }

  getDataTestId(compName: string, formControlName: string, uniqueKey: string) {
    const array: string[] = [];
    this.ifNotNullPush(array, compName);
    this.ifNotNullPush(array, formControlName);
    this.ifNotNullPush(array, uniqueKey);
    return this.sanitisedTestId(array.join('_'));
  }

  private sanitisedTestId(testId: string): string {
    let sanitisedTestId: string = null;
    if (isTruthy(testId)) {
      // Remove any 'undefined'
      sanitisedTestId = testId.replace(new RegExp('undefined', 'g'), '');
      // Remove any extra '_' 's
      if (sanitisedTestId.includes('__')) {
        sanitisedTestId = sanitisedTestId.replace(new RegExp('__', 'g'), '_');
      }
      // Remove any start '_'
      if (sanitisedTestId.startsWith('_')) {
        sanitisedTestId = sanitisedTestId.substring(1);
      }
      // Remove any end '_'
      if (sanitisedTestId.endsWith('_')) {
        sanitisedTestId = sanitisedTestId.slice(0, -1);
      }
    }
    return sanitisedTestId;
  }

  private ifNotNullPush(array: string[], data: string) {
    if (data != null && data.length > 0) {
      array.push(data.replace('Component', '').replace(' ', ''));
    }
  }
}
