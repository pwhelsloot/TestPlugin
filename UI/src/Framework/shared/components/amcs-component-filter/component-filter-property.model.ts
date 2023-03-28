import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';

export class ComponentFilterProperty implements ILookupItem {
  id: number;

  description: string;

  type: number;

  propertyKey: string;

  textOptionsRestricted = false;

  enumTypeItems: ILookupItem[];

  constructor(
    id: number,
    description: string,
    type: number,
    propertyKey: string,
    textOptionsRestricted = false,
    enumTypeItems?: ILookupItem[]
  ) {
    this.id = id;
    this.description = description;
    this.type = type;
    this.propertyKey = propertyKey;
    this.textOptionsRestricted = textOptionsRestricted;
    this.enumTypeItems = enumTypeItems;
  }
}
