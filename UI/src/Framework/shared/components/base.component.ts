import { Component, Inject, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { BaseComponentHelper } from './helpers/base-component-helper';

@Component({
  selector: 'app-base-component',
  template: '',
})
/**
 * @deprecated Marked for removal, please use the @aiComponent decorator instead.
 */
export class BaseComponent implements OnDestroy {
  constructor(
    public router: Router,
    public activatedRoute: ActivatedRoute,
    public instrumentationService: InstrumentationService,
    @Inject(String) public name: string
  ) {
    this.helper = new BaseComponentHelper(router, activatedRoute, instrumentationService, name);
  }

  private helper: BaseComponentHelper;

  ngOnDestroy(): void {
    this.helper.destroy();
  }
}
