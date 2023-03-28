import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI + ScaleUI + IMMUI
 */
@amcsJsonObject()
export class ContainerTypeLookup extends ApiBaseModel implements ILookupItem {

  @amcsJsonMember('ContainerTypeId')
  id: number;

  @amcsJsonMember('Description')
  description: string;
}
