import { Injectable } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { BaseService } from '@coreservices/base.service';
import { BehaviorSubject } from 'rxjs';
import { takeUntil, withLatestFrom } from 'rxjs/operators';

@Injectable({ providedIn: 'root' })
export class RouteContextService extends BaseService {

  constructor(private router: Router) {
    super();
  }

  private contextValues: any[] = new Array<any>();
  private retainContext = new BehaviorSubject<boolean>(false);

  initialise() {
    this.router.events.pipe(
      withLatestFrom(this.retainContext),
      takeUntil(this.unsubscribe)
    ).subscribe(data => {
      const navEvent = data[0];
      const retainContext: boolean = data[1];
      if (navEvent instanceof NavigationEnd) {
        if (!retainContext) {
          this.contextValues = [];
        } else {
          this.retainContext.next(false);
        }
      }

    });
  }

  getValue(key: any) {
    return this.contextValues[key];
  }

  setValue(key: string, value: any) {
    this.contextValues[key] = value;
    this.retainContext.next(true);
  }
}
