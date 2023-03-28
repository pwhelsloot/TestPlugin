import { GridColumnAdvancedSizeEnum } from '@core-module/models/column-customiser/grid-column-advanced-size.enum';
import { ComponentFilterPropertyValueType } from '@shared-module/components/amcs-component-filter/component-filter-property-value-type.enum';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { IMultiTileSelectorItem } from '../selector/multi-tile-selector-item.interface';
import { GridColumnAdvancedConfigSave } from './grid-column-advanced-config-save.model';
import { GridColumnAdvancedTypeEnum } from './grid-column-advanced-type.enum';

export class GridColumnAdvancedConfig extends GridColumnConfig implements IMultiTileSelectorItem {

    advancedType: GridColumnAdvancedTypeEnum = GridColumnAdvancedTypeEnum.Grid;
    size = GridColumnAdvancedSizeEnum.Single;

    canMove = true;

    // IMultiTileSelectorItem
    id: number;
    isSelected: boolean;
    cssClass: string;
    isMoving: boolean;
    filter: ComponentFilterProperty;

    constructor(title: string, key: string, filterType: ComponentFilterPropertyValueType, position?: number) {
        super(title, key, [0, 0], position);
        this.filter = new ComponentFilterProperty(null, title, filterType, key);
    }

    setAdvancedType(advancedType: GridColumnAdvancedTypeEnum): GridColumnAdvancedConfig {
        this.advancedType = advancedType;
        return this;
    }

    setSize(size: GridColumnAdvancedSizeEnum): GridColumnAdvancedConfig {
        this.size = size;
        return this;
    }

    setAsUnmoveable(): GridColumnAdvancedConfig {
        this.canMove = false;
        return this;
    }

    toSaveModel(): GridColumnAdvancedConfigSave {
        const saveModel = new GridColumnAdvancedConfigSave();
        saveModel.id = this.id;
        saveModel.advancedType = this.advancedType;
        saveModel.key = this.key;
        saveModel.position = this.position;
        return saveModel;
    }
}
