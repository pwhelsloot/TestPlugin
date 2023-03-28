import { isDevMode } from '@angular/core';
import { AiLoggingService } from '@core-module/services/logging/ai-logging.service';
import { filter, take } from 'rxjs/operators';
import { IAiComponent } from './ai-component.interface';

/**
 * Use to automatically track Apdex information for components.
 * @param viewName A user friendly name of the view/component
 * @param firstRenderTimeout Optional firstRendered timeout
 * @param destroyTimeout Optional destroyed timeout
 * @returns
 */
export function aiComponent<T extends IAiComponent>(viewName: string, firstRenderTimeout?: number, destroyTimeout?: number) {
  return function(component: new (...args: any[]) => T) {
    setUpInit(component, viewName);
    setUpFirstRender(component, viewName, firstRenderTimeout);
    setUpDestroy(component, viewName, destroyTimeout);
  };
}

/**
 * Before the ngOnInit has been called on the component we log viewInit event. This starts API call logging hence must be done early.
 * @param component The component constructor
 * @param viewName The user friendly name of the component
 */
function setUpInit(component, viewName: string): string {
  const onInit = component.prototype.ngOnInit;
  let activeUrl: string;
  component.prototype.ngOnInit = function(this: IAiComponent) {
    AiLoggingService.aiLoggingServiceReference.viewInit(viewName);
    setUpReady(this, viewName);
    if (onInit && typeof onInit === 'function') {
      onInit.apply(this, arguments);
    }
  };
  return activeUrl;
}

/**
 * Once ngAfterViewInit has been called on the component we log viewFirstRender event after an optional extra timeout.
 * @param component The component constructor
 * @param viewName The user friendly name of the component
 * @param timeout Optional timeout
 */
function setUpFirstRender(component, viewName: string, timeout?: number): void {
  const onAfterViewInit = component.prototype.ngAfterViewInit;
  component.prototype.ngAfterViewInit = function() {
    if (onAfterViewInit && typeof onAfterViewInit === 'function') {
      onAfterViewInit.apply(this, arguments);
    }
    if (timeout) {
      setTimeout(() => {
        AiLoggingService.aiLoggingServiceReference.viewFirstRender(viewName);
      }, timeout);
    } else {
      AiLoggingService.aiLoggingServiceReference.viewFirstRender(viewName);
    }
  };
}

/**
 * Once ngOnDestroy has been called on the component we log viewDestroyed event after an optional extra timeout.
 * @param component The component constructor
 * @param viewName The user friendly name of the component
 * @param timeout Optional timeout
 */
function setUpDestroy(component, viewName: string, timeout?: number): void {
  const onDestroy = component.prototype.ngOnDestroy;
  component.prototype.ngOnDestroy = function() {
    if (onDestroy && typeof onDestroy === 'function') {
      onDestroy.apply(this, arguments);
    }
    if ((this as IAiComponent)?.viewReady) {
      this.viewReady.complete();
    }
    if (timeout) {
      setTimeout(() => {
        AiLoggingService.aiLoggingServiceReference.viewDestroyed(viewName);
      }, timeout);
    } else {
      AiLoggingService.aiLoggingServiceReference.viewDestroyed(viewName);
    }
  };
}

/**
 * Subscribes to viewReady subject and logs viewReady event once fired.
 * @param component The instance of the component (not the constructor, the component is now created)
 * @param viewName The user friendly name of the component
 * @returns
 */
function setUpReady(component: IAiComponent, viewName: string): void {
  if (isDevMode() && !component.viewReady) {
    throw new Error(`viewReady subject missing for ${viewName}. This must be created/assigned in component class definition or constructor, i.e BEFORE ngOnInit.`);
  }
  // If this has made it into production then don't attempt to track viewReady
  if (!component.viewReady) {
    return;
  }
  component.viewReady
    .pipe(
      // Even with Subject<boolean> people can still do .next() without a value. In this case we assume view is ready
      filter((x) => x === undefined || x),
      take(1)
    )
    .subscribe(() => {
      AiLoggingService.aiLoggingServiceReference.viewReady(viewName);
    });
}
