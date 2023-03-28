import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';

export class ComponentFilterType implements ILookupItem {

  id: number;

  description: string;

  constructor(id: number, description: string) {
    this.id = id;
    this.description = description;
  }

}
