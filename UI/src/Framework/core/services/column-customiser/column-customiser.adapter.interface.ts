import { GridColumnAdvancedConfig } from '@core-module/models/column-customiser/grid-column-advanced-config.model';
import { Observable } from 'rxjs';

export interface IColumnCustomiserAdapter {

    gridKey: string;
    getDefaultColumns$(): Observable<GridColumnAdvancedConfig[]>;

}
