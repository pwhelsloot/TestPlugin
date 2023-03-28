import { alias } from '@coreconfig/api-dto-alias.function';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '../api/api-base.model';
import { ILookupItem } from './lookup-item.interface';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DocumentDeliveryTypeLookup extends ApiBaseModel implements ILookupItem {

    @amcsJsonMember('DocumentDeliveryTypeId')
    @alias('DocumentDeliveryTypeId')
    id: number;

    @amcsJsonMember('Description')
    @alias('Description')
    description: string;

    @amcsJsonMember('GUID')
    @alias('GUID')
    guid: string;
}
