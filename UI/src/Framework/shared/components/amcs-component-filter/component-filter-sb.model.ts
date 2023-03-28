import { nameof } from '@core-module/helpers/name-of.function';
import { ComponentFilterPropertyValueType } from '@shared-module/components/amcs-component-filter/component-filter-property-value-type.enum';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { GridColumnType } from '@shared-module/components/amcs-grid/grid-column-type.enum';
import { IFilterableItem } from '@shared-module/models/ifilterable-item.interface';

export class ComponentFilterSB implements IFilterableItem {

  service: string;
  material: string;
  quantity: number;
  price: number;
  startDate: Date;
  endDate: Date;

  constructor(service: string, material: string, quantity: number, price: number, startDate: Date, endDate: Date) {
    this.service = service;
    this.material = material;
    this.quantity = quantity;
    this.price = price;
    this.startDate = startDate;
    this.endDate = endDate;
  }

  static getGridColumns(): GridColumnConfig[] {
    return [
        new GridColumnConfig('Service', nameof<ComponentFilterSB>('service'), [15]),
        new GridColumnConfig('Material', nameof<ComponentFilterSB>('material'), [15]),
        new GridColumnConfig('Quantity', nameof<ComponentFilterSB>('quantity'), [15]),
        new GridColumnConfig('Price', nameof<ComponentFilterSB>('price'), [15]),
        new GridColumnConfig('Start Date', nameof<ComponentFilterSB>('startDate'), [15]).withType(GridColumnType.shortDate),
        new GridColumnConfig('End Date', nameof<ComponentFilterSB>('endDate'), [15]).withType(GridColumnType.shortDate)
    ];
  }

  static getFilterProperties(): ComponentFilterProperty[] {
    return [
      new ComponentFilterProperty(1, 'Service', ComponentFilterPropertyValueType.text, nameof<ComponentFilterSB>('service')),
      new ComponentFilterProperty(2, 'Material', ComponentFilterPropertyValueType.text, nameof<ComponentFilterSB>('material')),
      new ComponentFilterProperty(3, 'Quantity', ComponentFilterPropertyValueType.number, nameof<ComponentFilterSB>('quantity')),
      new ComponentFilterProperty(4, 'Price', ComponentFilterPropertyValueType.number, nameof<ComponentFilterSB>('price')),
      new ComponentFilterProperty(5, 'Start Date', ComponentFilterPropertyValueType.date, nameof<ComponentFilterSB>('startDate')),
      new ComponentFilterProperty(6, 'End Date', ComponentFilterPropertyValueType.date, nameof<ComponentFilterSB>('endDate'))
    ];
  }

  getTextValueFromPropertyKey(key: string): string {
    switch (key) {
      case 'service':
        return this.service;

      case 'material':
        return this.material;

      default:
          return null;
    }
  }

  getNumberValueFromPropertyKey(key: string): number {
    switch (key) {
      case 'quantity':
        return this.quantity;

      case 'price':
        return this.price;

      default:
          return null;
    }
  }

  getDateValueFromPropertyKey(key: string): Date {
    switch (key) {
      case 'startDate':
        return this.startDate;

      case 'endDate':
        return this.endDate;

      default:
          return null;
    }
  }

  getBooleanValueFromPropertyKey(key: string): boolean {
    return false;
  }
}
