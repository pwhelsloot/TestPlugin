import { alias } from '../config/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class MaterialSalesSlotCopyPreferences {

    @alias('outlets')
    outletIds: number[];
}
