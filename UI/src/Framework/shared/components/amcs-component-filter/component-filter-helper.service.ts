import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { BaseService } from '@core-module/services/base.service';
import { IFilterableItem } from '@shared-module/models/ifilterable-item.interface';
import { ComponentFilterPropertyValueType } from './component-filter-property-value-type.enum';
import { ComponentFilter } from './component-filter.model';

export class ComponentFilterHelper extends BaseService {
  static isEqual(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.text: {
        const lowerCaseTextFilterValue = this.stringEmptyIfNull(filter.filterTextValue).toLowerCase();
        return data.filter(
          (x) => this.stringEmptyIfNull(x.getTextValueFromPropertyKey(filter.propertyKey)).toLowerCase() === lowerCaseTextFilterValue
        );
      }
      case ComponentFilterPropertyValueType.number: {
        return data.filter((x) => x.getNumberValueFromPropertyKey(filter.propertyKey) === filter.filterNumberValue);
      }
      case ComponentFilterPropertyValueType.date: {
        return data.filter((x) => this.checkDateValueForEqual(x, filter.propertyKey) === filter.filterDateValue.getTime());
      }
      case ComponentFilterPropertyValueType.boolean: {
        return data.filter((x) => this.checkBooleanValueForEqual(x, filter.propertyKey) === (filter.filterBooleanValue === 1));
      }
      case ComponentFilterPropertyValueType.enum: {
        return data.filter((x) => x.getEnumValueFromPropertyKey(filter.propertyKey) === filter.filterEnumValue);
      }
      default:
        return data;
    }
  }

  static isNotEqual(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.text: {
        const lowerCaseTextFilterValue = this.stringEmptyIfNull(filter.filterTextValue).toLowerCase();
        return data.filter(
          (x) => this.stringEmptyIfNull(x.getTextValueFromPropertyKey(filter.propertyKey)).toLowerCase() !== lowerCaseTextFilterValue
        );
      }
      case ComponentFilterPropertyValueType.number: {
        return data.filter((x) => x.getNumberValueFromPropertyKey(filter.propertyKey) !== filter.filterNumberValue);
      }
      case ComponentFilterPropertyValueType.date: {
        return data.filter((x) => this.checkDateValueForEqual(x, filter.propertyKey) !== filter.filterDateValue.getTime());
      }
      case ComponentFilterPropertyValueType.boolean: {
        return data.filter((x) => this.checkBooleanValueForEqual(x, filter.propertyKey) !== (filter.filterBooleanValue === 1));
      }
      case ComponentFilterPropertyValueType.enum: {
        return data.filter((x) => x.getEnumValueFromPropertyKey(filter.propertyKey) !== filter.filterEnumValue);
      }
      default:
        return data;
    }
  }

  static startsWith(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.text: {
        const lowerCaseTextFilterValue = this.stringEmptyIfNull(filter.filterTextValue).toLowerCase();
        return data.filter((x) =>
          this.stringEmptyIfNull(x.getTextValueFromPropertyKey(filter.propertyKey)).toLowerCase().startsWith(lowerCaseTextFilterValue)
        );
      }
      default:
        return data;
    }
  }

  static endsWith(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.text: {
        const lowerCaseTextFilterValue = this.stringEmptyIfNull(filter.filterTextValue).toLowerCase();
        return data.filter((x) =>
          this.stringEmptyIfNull(x.getTextValueFromPropertyKey(filter.propertyKey)).toLowerCase().endsWith(lowerCaseTextFilterValue)
        );
      }
      default:
        return data;
    }
  }

  static contains(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.text: {
        const lowerCaseTextFilterValue = this.stringEmptyIfNull(filter.filterTextValue).toLowerCase();
        return data.filter((x) =>
          this.stringEmptyIfNull(x.getTextValueFromPropertyKey(filter.propertyKey)).toLowerCase().includes(lowerCaseTextFilterValue)
        );
      }
      default:
        return data;
    }
  }

  static isEmpty(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.text: {
        return data.filter((x) => this.stringEmptyIfNull(x.getTextValueFromPropertyKey(filter.propertyKey)) === '');
      }
      default:
        return data;
    }
  }

  static isNotEmpty(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.text: {
        return data.filter((x) => this.stringEmptyIfNull(x.getTextValueFromPropertyKey(filter.propertyKey)) !== '');
      }
      default:
        return data;
    }
  }

  static isGreaterThan(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.number: {
        return data.filter((x) => x.getNumberValueFromPropertyKey(filter.propertyKey) > filter.filterNumberValue);
      }
      case ComponentFilterPropertyValueType.date: {
        return data.filter((x) => this.checkDateValue(x, filter.propertyKey) >= filter.filterDateValue);
      }
      default:
        return data;
    }
  }

  static isGreaterThanOrEqual(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.number: {
        return data.filter((x) => x.getNumberValueFromPropertyKey(filter.propertyKey) >= filter.filterNumberValue);
      }
      case ComponentFilterPropertyValueType.date: {
        return data.filter((x) => this.checkDateValue(x, filter.propertyKey) >= filter.filterDateValue);
      }
      default:
        return data;
    }
  }

  static isLessThan(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.number: {
        return data.filter((x) => x.getNumberValueFromPropertyKey(filter.propertyKey) < filter.filterNumberValue);
      }
      case ComponentFilterPropertyValueType.date: {
        return data.filter((x) => this.checkDateValue(x, filter.propertyKey) < filter.filterDateValue);
      }
      default:
        return data;
    }
  }

  static isLessThanOrEqual(data: IFilterableItem[], filter: ComponentFilter): IFilterableItem[] {
    switch (filter.propertyValueType) {
      case ComponentFilterPropertyValueType.number: {
        return data.filter((x) => x.getNumberValueFromPropertyKey(filter.propertyKey) <= filter.filterNumberValue);
      }
      case ComponentFilterPropertyValueType.date: {
        return data.filter((x) => this.checkDateValue(x, filter.propertyKey) <= filter.filterDateValue);
      }
      default:
        return data;
    }
  }

  private static stringEmptyIfNull(inputString: string): string {
    return inputString || '';
  }

  private static checkDateValue(x: IFilterableItem, propertyKey: string): Date {
    const dateValue = x.getDateValueFromPropertyKey(propertyKey);
    return isTruthy(dateValue) ? AmcsDate.removeTime(dateValue) : null;
  }

  private static checkDateValueForEqual(x: IFilterableItem, propertyKey: string): number {
    const dateValue = x.getDateValueFromPropertyKey(propertyKey);
    return isTruthy(dateValue) ? AmcsDate.removeTime(dateValue).getTime() : null;
  }

  private static checkBooleanValueForEqual(x: IFilterableItem, propertyKey: string): boolean {
    const booleanValue = x.getBooleanValueFromPropertyKey(propertyKey);
    return isTruthy(booleanValue) ? booleanValue : false;
  }
}
