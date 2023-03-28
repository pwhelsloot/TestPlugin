import { Directive, ElementRef, Input, OnChanges, OnInit } from '@angular/core';

@Directive({
    // tslint:disable:directive-selector
    // eslint-disable-next-line
    selector: '[takeFocus]'
})
export class TakeFocusDirective implements OnInit, OnChanges {

    @Input('takeFocus') takeFocus: boolean;

    constructor(
        private el: ElementRef) {
    }

    ngOnInit() {
        this.setFocus();
    }

    ngOnChanges() {
        this.setFocus();
    }

    private setFocus(): void {
        if (this.takeFocus) {
            setTimeout(() => {
                this.el.nativeElement.focus();
            }, 150);
        }
    }
}
