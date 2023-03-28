import { Directive, ElementRef, OnDestroy, OnInit } from '@angular/core';
import { LoggingVerbs } from '@coreservices/logging/loggingVerbs.model';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';

let nextId = 1;

// #docregion spy-directive
// Spy on any element to which it is applied.
// Usage: <div mySpy>...</div>
@Directive({ selector: '[appInstrumentation]' })
export class InstrumentationDirective implements OnInit, OnDestroy {

    constructor(private instrumentationService: InstrumentationService, private elementRef: ElementRef) {

    }
    ngOnInit() {
        nextId++;
        this.instrumentationService.trackComponentLifecycleEvent(`${LoggingVerbs.ComponentOnInit} ${this.elementRef.nativeElement.localName}`,
            { eventId: nextId.toString() }
        );
    }

    ngOnDestroy() {
        this.instrumentationService.trackComponentLifecycleEvent(`${LoggingVerbs.ComponentOnDestroy} ${this.elementRef.nativeElement.localName}`,
            { eventId: 'NA' }
        );
    }
}
