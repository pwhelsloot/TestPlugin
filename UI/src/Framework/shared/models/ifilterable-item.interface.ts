export interface IFilterableItem {
  getTextValueFromPropertyKey(key: string): string;
  getNumberValueFromPropertyKey(key: string): number;
  getDateValueFromPropertyKey(key: string): Date;
  getBooleanValueFromPropertyKey(key: string): boolean;
  getEnumValueFromPropertyKey?(key: string): number;
}
