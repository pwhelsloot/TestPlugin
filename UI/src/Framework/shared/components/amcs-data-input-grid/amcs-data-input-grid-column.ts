import { TemplateRef } from '@angular/core';
import { AmcsDataInputGridColumnType } from './amcs-data-input-grid-column-type.enum';

export class AmcsDataInputGridColumn {
    sizePercentage: string;
    relativeSizePercentage: string;
    template: TemplateRef<any>;

    // All AmcsDataInputGridColumn sizes should add to 100
    constructor(public name: string, public title: string, public size: number, public rightAlign: boolean = false, public columnType: AmcsDataInputGridColumnType = AmcsDataInputGridColumnType.text) {
    }

    withTemplate(template: TemplateRef<any>): AmcsDataInputGridColumn {
        this.template = template;
        return this;
    }
}
