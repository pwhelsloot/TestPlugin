import {Directive, ElementRef, Output, EventEmitter, HostListener} from '@angular/core';

@Directive({
    // tslint:disable:directive-selector
    // eslint-disable-next-line
    selector: '[clickOutside]'
})
export class ClickOutsideDirective {

    @Output()
    clickOutside = new EventEmitter<MouseEvent>();

    constructor(private _elementRef: ElementRef) {
    }

    @HostListener('document:click', ['$event', '$event.target'])
    @HostListener('document:touchmove', ['$event', '$event.target'])
    onClick(event: MouseEvent, targetElement: HTMLElement): void {
        if (!targetElement) {
            return;
        }
        const clickedInside = this._elementRef.nativeElement.contains(targetElement);
        if (!clickedInside) {
            this.clickOutside.emit(event);
        }
    }
}
