import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from '@core-module/models/api/api-base.model';

@amcsJsonObject()
export class PlatformConfiguration extends ApiBaseModel {

    // Only configs shared between scale/tdm/csr here!

    @amcsJsonMember('DisableClassicLogin')
    disableClassicLogin: boolean;

    @amcsJsonMember('InventoryManagementEnabled')
    inventoryManagementEnabled: boolean;
}
