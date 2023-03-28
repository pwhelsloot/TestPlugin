import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ResizeableColumn } from '@core-module/models/grid/resizeable-column.model';
import { DecimalPlacesEnum } from '@shared-module/models/decimal-places.enum';
import { Subject } from 'rxjs';
import { GridColumnAlignment } from '../amcs-grid/grid-column-alignment.enum';
import { GridColumnType } from '../amcs-grid/grid-column-type.enum';
import { GridTotalsHeaderColumnConfig } from '../amcs-grid/grid-totals-header-column-config';

export class AmcsGridFormColumn extends ResizeableColumn {
    relativeWidthMultiplier: number;
    relativeWidthPercentage: number;
    currencyCode: string;
    align: GridColumnAlignment = GridColumnAlignment.start;
    type: GridColumnType = GridColumnType.text;
    noOfDecimalPlaces: string;

    totalsHeaderConfig = new GridTotalsHeaderColumnConfig();

    sizeChanged = new Subject();

    // All AmcsGridFormColumn sizes should add to 100
    constructor(public key: string, public widthPercentage: number) {
        super();
    }

    afterResize() {
        this.relativeWidthPercentage = this.relativeWidthMultiplier * this.widthPercentage;
        this.sizeChanged.next();
    }

    withCurrencyCode(currencyCode: string) {
        this.currencyCode = currencyCode;
        return this;
    }

    withType(type: GridColumnType) {
      this.type = type;
      // RDM As standard bools should be centered
      if (this.type === GridColumnType.boolean) {
          this.withAlignment(GridColumnAlignment.center);
      }
      return this;
    }

    withAlignment(align: GridColumnAlignment) {
      this.align = align;
      return this;
    }

    withIncludeInTopTotal(config: GridTotalsHeaderColumnConfig) {
      this.totalsHeaderConfig = config;
      this.totalsHeaderConfig.includeInTopTotal = true;

      if (isTruthy(config.numberOfDecimals)) {
        this.noOfDecimalPlaces = '.' + config.numberOfDecimals + '-' + config.numberOfDecimals;
      } else {
          this.noOfDecimalPlaces = '.' + DecimalPlacesEnum.two + '-' + DecimalPlacesEnum.two;
      }

      if (isTruthy(config.currencyCode) && config.columnType === GridColumnType.currency) {
          this.withCurrencyCode(config.currencyCode);
      }

      return this;
  }
}
