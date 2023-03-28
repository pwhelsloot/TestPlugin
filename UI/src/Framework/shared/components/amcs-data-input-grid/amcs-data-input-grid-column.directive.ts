import { Directive, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';
import { AmcsDataInputGridColumn } from '@shared-module/components/amcs-data-input-grid/amcs-data-input-grid-column';

@Directive({
    selector: '[appAmcsDataInputGridColumn]'
})
// I'm used on the form template and means we can easily add/change classes of the cells in one place rather than each template.
export class AmcsDataInputGridColumnDirective implements OnInit {

    @Input('appAmcsDataInputGridColumn') appAmcsDataInputGridColumn: AmcsDataInputGridColumn;

    constructor(private el: ElementRef, private renderer: Renderer2) {
    }

    ngOnInit() {
        this.renderer.setStyle(this.el.nativeElement, 'flex-basis', this.appAmcsDataInputGridColumn.sizePercentage);
        this.renderer.addClass(this.el.nativeElement, 'amcs-flex-item');
        this.renderer.addClass(this.el.nativeElement, 'amcs-grid-cell');
    }
}
