import { TemplateRef } from '@angular/core';
import { ResizeableColumn } from '@core-module/models/grid/resizeable-column.model';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { GridCellColorEnum } from '@shared-module/models/grid-cell-color-enum';
import { Subject } from 'rxjs';
import { isTruthy } from '../../../core/helpers/is-truthy.function';
import { DecimalPlacesEnum } from '../../models/decimal-places.enum';
import { GridColumnAlignment } from './grid-column-alignment.enum';
import { GridColumnType } from './grid-column-type.enum';
import { GridColumnViewport } from './grid-column-viewport';
import { GridTotalsHeaderColumnConfig } from './grid-totals-header-column-config';
import { GridViewport } from './grid-viewport.enum';

export class GridColumnConfig extends ResizeableColumn {
    title: string;                                              // the column header text                                            // used to retrieve the data for this column from a row object
    type: GridColumnType = GridColumnType.text;                 // determines how the cell is rendered
    viewports: GridColumnViewport[];                            // for now, the grid supports only 2 viewports - one for sm and xs and one for md, lg and xl
    align: GridColumnAlignment = GridColumnAlignment.start;
    sortedBy = false;
    sortedByDesc = false;
    sortedByAnotherPropertyKey = null;
    sortDisabled = false;

    selectItems: ILookupItem[];
    superScriptText: string;
    superScriptTextPropertyName: string;
    minInputValue = null;
    maxInputValue = null;
    maxLength = null;
    isBold = false;

    position: number;
    currencyCode: string;
    noOfDecimalPlaces: string;

    enumerations: { key: number; value: string }[];

    cellValueKey: string;
    cellValues: number[] = [];
    baseValue = 0;

    applyConditionalColor = false;
    cellColors: GridCellColorEnum[] = [];
    defaultColor = GridCellColorEnum.none;
    lessThanColor = GridCellColorEnum.red;
    greaterThanColor = GridCellColorEnum.green;

    template: TemplateRef<any>;

    tooltipManualResizeSubject = new Subject();

    includeInTotal = false;
    includeInTopTotal = false;
    topTotalType: GridColumnType;
    topTotalUnitOfMeasurement: string;
    topTotalColumnMappedToColumnKey = null;

    constructor(title: string, key: string, size: number[], position?: number) {
        super();
        this.title = title;
        this.key = key;
        this.viewports = [];

        // for now, support 1 or 2 items in the sizes array, where the first item is the mobile width and the second is the desktop.
        // deprecated though - should be done with the withViewport method instead.
        if (size.length === 1) {
            this.viewports.push(new GridColumnViewport(GridViewport.desktop, true, size[0]));
            this.viewports.push(new GridColumnViewport(GridViewport.mobile, true, size[0]));
        } else if (size.length === 2) {
            this.viewports.push(new GridColumnViewport(GridViewport.mobile, true, size[0]));
            this.viewports.push(new GridColumnViewport(GridViewport.desktop, true, size[1]));
        }

        this.position = !position ? -1 : position;             // if a position is not supplied, the grid will assign the next available
    }

    withType(type: GridColumnType) {
        this.type = type;
        // RDM As standard bools should be centered
        if (this.type === GridColumnType.boolean) {
            this.withAlignment(GridColumnAlignment.center);
        }
        return this;
    }

    withSuperScriptText(superScriptText: string) {
        this.superScriptText = superScriptText;
        return this;
    }

    withSuperScriptTextFromRow(propertyName: string) {
        this.superScriptTextPropertyName = propertyName;
        return this;
    }

    withEnumeration(keys: { key: number; value: string }[]) {
        this.enumerations = keys;
        return this;
    }

    withCurrencyCode(currencyCode: string, noOfDecimals?: DecimalPlacesEnum) {
        this.currencyCode = currencyCode;
        if (isTruthy(noOfDecimals)) {
            this.noOfDecimalPlaces = '.' + noOfDecimals + '-' + noOfDecimals;
        } else {
            this.noOfDecimalPlaces = '.' + DecimalPlacesEnum.two + '-' + DecimalPlacesEnum.two;
        }
        return this;
    }

    withDecimal(noOfDecimals?: DecimalPlacesEnum) {
        if (isTruthy(noOfDecimals)) {
            this.noOfDecimalPlaces = '.' + noOfDecimals + '-' + noOfDecimals;
        } else {
            this.noOfDecimalPlaces = '.' + DecimalPlacesEnum.two + '-' + DecimalPlacesEnum.two;
        }
        return this;
    }

    withAlignment(align: GridColumnAlignment) {
        this.align = align;
        return this;
    }

    withSort() {
        this.sortedBy = true;
        return this;
    }

    withSortViaColumn(key: string) {
        this.sortedByAnotherPropertyKey = key;
        return this;
    }

    withSortDescending() {
        this.sortedByDesc = true;
        return this;
    }

    withSortDisabled() {
      this.sortDisabled = true;
      return this;
    }

    withViewport(viewport: GridViewport, display: boolean, widthPercentage?: number) {
        this.viewports.push(new GridColumnViewport(viewport, display, widthPercentage));
        return this;
    }

    hideOnMobile() {
        const isMobileViewExist = this.viewports.find(x => x.viewport === GridViewport.mobile);
        if (isMobileViewExist) {
            isMobileViewExist.visible = false;
            isMobileViewExist.widthPercentage = null;
        } else {
            this.viewports.push(new GridColumnViewport(GridViewport.mobile, false));
        }
        return this;
    }

    withIncludeInTotal(includeInTotal: boolean) {
        this.includeInTotal = includeInTotal;
        return this;
    }

    withIncludeInTopTotal(config: GridTotalsHeaderColumnConfig) {
        this.includeInTopTotal = true;
        this.topTotalType = config.columnType;
        this.topTotalUnitOfMeasurement = config.unitOfMeasurement;
        this.topTotalColumnMappedToColumnKey = config.mappedToColumnKey;

        if (isTruthy(config.currencyCode) && config.columnType === GridColumnType.currency) {
            this.withCurrencyCode(config.currencyCode, config.numberOfDecimals);
        }

        return this;
    }

    withTemplate(template: TemplateRef<any>) {
        this.template = template;
        return this;
    }

    withSelectItems(items: ILookupItem[]) {
        this.selectItems = items;
        return this;
    }

    hideColumn() {
        this.viewports = [];
        return this;
    }

    afterResize() {
        this.tooltipManualResizeSubject.next();
    }

    withMinInputValue(minValue: number) {
        this.minInputValue = minValue;
        return this;
    }

    withMaxInputValue(maxValue: number) {
        this.maxInputValue = maxValue;
        return this;
    }

    withMaxInputLength(maxLength: number) {
        this.maxLength = maxLength;
        return this;
    }

    setBold(isBold: boolean) {
        this.isBold = isBold;
        return this;
    }

    setCellColorsBasedOnValue(baseValue: number, cellValueKey: string, cellValues: number[],
        lessThanColor: GridCellColorEnum = GridCellColorEnum.red, greaterThanColor: GridCellColorEnum = GridCellColorEnum.green,
        defaultColor: GridCellColorEnum = GridCellColorEnum.none) {
        this.applyConditionalColor = true;
        this.lessThanColor = lessThanColor;
        this.greaterThanColor = greaterThanColor;
        this.defaultColor = defaultColor;
        this.cellValueKey = cellValueKey;
        this.baseValue = baseValue;
        this.assignCellColorsBasedOnValue(cellValues);
    }

    assignCellColorsBasedOnValue(cellValues: number[]) {
        this.cellColors = [];
        this.applyConditionalColor = true;
        this.cellValues = cellValues;
        this.cellValues.forEach(x => {
            if (x > this.baseValue) {
                this.cellColors.push(this.greaterThanColor);
            } else if (x < this.baseValue) {
                this.cellColors.push(this.lessThanColor);
            } else {
                this.cellColors.push(this.defaultColor);
            }
        });
    }
}
