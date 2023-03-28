import { Directive, ElementRef, Input, OnInit, Renderer2, OnDestroy } from '@angular/core';
import { AmcsGridFormColumn } from '@shared-module/components/amcs-grid-form/amcs-grid-form-column';
import { Subscription } from 'rxjs';

@Directive({
    selector: '[appAmcsGridFormColumn]'
})
// I'm used on the form template and means we can easily add/change classes of the cells in one place rather than each template.
export class AmcsGridFormColumnDirective implements OnInit, OnDestroy {

    @Input('appAmcsGridFormColumn') appAmcsGridFormColumn: AmcsGridFormColumn;

    constructor(private el: ElementRef, private renderer: Renderer2) {
    }

    private sizeChangedSubscription: Subscription;

    ngOnInit() {
        this.renderer.setStyle(this.el.nativeElement, 'flex-basis', this.appAmcsGridFormColumn.widthPercentage.toString() + '%');
        this.sizeChangedSubscription = this.appAmcsGridFormColumn.sizeChanged.subscribe(() => {
            this.renderer.setStyle(this.el.nativeElement, 'flex-basis', this.appAmcsGridFormColumn.widthPercentage.toString() + '%');
        });
        this.renderer.addClass(this.el.nativeElement, 'amcs-flex-item');
        this.renderer.addClass(this.el.nativeElement, 'amcs-grid-cell');
    }

    ngOnDestroy() {
        this.sizeChangedSubscription.unsubscribe();
    }
}
