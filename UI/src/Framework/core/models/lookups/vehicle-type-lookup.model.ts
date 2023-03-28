import { alias } from '@coreconfig/api-dto-alias.function';

/**
 * @deprecated Move to PlatformUI
 */
export class VehicleTypeLookup {

  @alias('VehicleTypeId')
  id: number;

  @alias('Description')
  description: string;
}
