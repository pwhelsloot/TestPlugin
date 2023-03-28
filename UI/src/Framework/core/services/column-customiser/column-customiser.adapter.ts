
import { Injectable, TemplateRef } from '@angular/core';
import { GridColumnAdvancedConfig } from '@core-module/models/column-customiser/grid-column-advanced-config.model';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { Observable, Subject } from 'rxjs';
import { IColumnCustomiserAdapter } from './column-customiser.adapter.interface';

@Injectable()
export abstract class ColumnCustomiserAdapter implements IColumnCustomiserAdapter {

    // totalPercentageOffset will need to be > 0 if you use action columns ect then add the width % here
    totalPercentageOffset = 0;
    // numberOfColumnsBeforeScroll really sizeOfColumnsBeforeScroll as we can now choose column sizes single/double/triple/quadtriple
    numberOfColumnsBeforeScroll = 10;
    disabledColumnClass = 'col-lg-6 col-md-6 col-sm-6 col-xs-6';
    enabledColumnClass = 'col-lg-2 col-md-2 col-sm-2 col-xs-2';
    truncateText = false;
    requestRefreshColumnSubject: Subject<void> = new Subject();
    requestRefreshColumn$: Observable<void> = this.requestRefreshColumnSubject.asObservable();

    // These should be implemented by the adapter extender
    abstract gridKey: string;

    // These will be set by the column customiser component.
    accordianTemplate: TemplateRef<any> = null;
    gridColumns: GridColumnConfig[] = null;
    gridFilterColumns: ComponentFilterProperty[] = null;
    excludeFilterColumns: string[] = [];

    // These should be implemented by the adapter extender
    abstract getDefaultColumns$(): Observable<GridColumnAdvancedConfig[]>;
}
