import { DecimalPlacesEnum } from '@shared-module/models/decimal-places.enum';
import { GridColumnType } from './grid-column-type.enum';

export class GridTotalsHeaderColumnConfig {
    columnType: GridColumnType;
    currencyCode?: string;
    numberOfDecimals?: DecimalPlacesEnum;
    unitOfMeasurement?: string;
    mappedToColumnKey?: string;
    includeInTopTotal?: boolean;
}
