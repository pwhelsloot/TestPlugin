import { alias } from '@core-module/config/api-dto-alias.function';
import { amcsApiUrl, amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

/**
 * @deprecated Move to PlatformUI + ScaleUI (or new module) https://dev.azure.com/amcsgroup/Platform/_workitems/edit/188268
 */
@amcsJsonObject()
@amcsApiUrl('AddressFieldDisplayConfigurations')
export class AddressFieldDisplayConfiguration extends ApiBaseModel {

    @amcsJsonMember('AddressFieldDisplayConfigurationId')
    @alias('AddressFieldDisplayConfigurationId')
    addressFieldDisplayConfigurationId: number;

    @amcsJsonMember('Name')
    @alias('Name')
    name: string;

    @amcsJsonMember('Text')
    @alias('Text')
    text: string;

    @amcsJsonMember('IsVisible')
    @alias('IsVisible')
    isVisible: boolean;

    @amcsJsonMember('IsMandatory')
    @alias('IsMandatory')
    isMandatory: boolean;

    @amcsJsonMember('IsVisibleInShortFormat')
    @alias('IsVisibleInShortFormat')
    isVisibleInShortFormat: boolean;

    @amcsJsonMember('Order')
    @alias('Order')
    order: number;

    index: number;

    isError: boolean;

    labelKey: number;

    isStaticField: boolean;
}

