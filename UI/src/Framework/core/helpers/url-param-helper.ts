import { getIntValue } from './number-helper';

export function parseEntityId(id: string): number {
  const numericValue = getIntValue(id);

  // entity id cannot be 0 - if the value is greater than zero
  // it's a valid id, otherwise we return null
  return numericValue > 0 ? numericValue : null;
}
