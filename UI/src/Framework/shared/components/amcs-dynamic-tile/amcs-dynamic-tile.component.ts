import { AfterViewInit, Component, ComponentFactoryResolver, ComponentRef, Input, OnDestroy, Type, ViewChild, ViewContainerRef } from '@angular/core';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';

let nextId = 1;

@Component({
  selector: 'app-amcs-dynamic-tile',
  templateUrl: './amcs-dynamic-tile.component.html',
  styleUrls: ['./amcs-dynamic-tile.component.scss']
})
export class AmcsDynamicTileComponent implements AfterViewInit, OnDestroy {

  @Input() id: string;
  @Input() component: Type<any>;
  @ViewChild('container', { read: ViewContainerRef }) container: ViewContainerRef;

  constructor(private instrumentationService: InstrumentationService,
    private componentFactoryResolver: ComponentFactoryResolver) {
  }

  private componentRef: ComponentRef<{}>;

  ngAfterViewInit() {
    this.componentRef = this.container.createComponent(this.componentFactoryResolver.resolveComponentFactory(this.component));
    this.componentRef.changeDetectorRef.detectChanges();
    nextId++;
    this.instrumentationService.trackComponentLifecycleEvent(`${LoggingVerbs.ComponentOnInit} ${this.id}`,
      { eventId: nextId.toString() }
    );
  }

  ngOnDestroy() {
    if (this.componentRef) {
      this.instrumentationService.trackComponentLifecycleEvent(`${LoggingVerbs.ComponentOnDestroy} ${this.id}`,
        { eventId: 'NA' }
      );
      this.componentRef.destroy();
      this.componentRef = null;
    }
  }
}
