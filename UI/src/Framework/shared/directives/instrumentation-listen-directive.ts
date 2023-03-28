import { Directive, ElementRef, HostListener } from '@angular/core';
import { InstrumentationService } from '@coreservices/logging/instrumentationService.service';

// #docregion spy-directive
// Spy on any element to which it is applied.
// Usage: <div mySpy>...</div>
@Directive({ selector: '[appInstrumentationListen]' })
export class InstrumentationListenDirective {

    constructor(private instrumentationService: InstrumentationService, private elementRef: ElementRef) {

    }

    @HostListener('click', ['$event', '$event.target'])
    logClick(event: any, targetElement: HTMLElement): void {

        let uiEventName = 'UI Event - ';
        let instrumentationKey = '';
        if (event.target.attributes['data-instrumentationKey'] == null) {
            uiEventName = uiEventName + this.elementRef.nativeElement.localName;
            instrumentationKey = 'NA';
        } else {
            uiEventName = uiEventName + event.target.attributes['data-instrumentationKey'].value;
            instrumentationKey = event.target.attributes['data-instrumentationKey'].value;
        }

        this.instrumentationService.trackUIInteractionEvent(`${uiEventName}`,
            {
                instrumentationKey,
                eventType: event.type,
                nodeName: event.target.nodeName,
                innerText: event.target['innerText'],
                componentName: this.elementRef.nativeElement.localName
            }
        );
    }
}
