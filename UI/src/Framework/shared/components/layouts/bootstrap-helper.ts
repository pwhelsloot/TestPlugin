import { environment } from '@environments/environment';

export function getDesktopColumns(size: number) {
  return `${getLGColumn(size)} ${getMDColumn(size)}`;
}

export function getMobileColumns(size: number) {
  return `${getSMColumn(size)}`;
}

export function getLGColumn(size: number) {
  return `col-lg-${size}`;
}

export function getMDColumn(size: number) {
  return `col-md-${size}`;
}

export function getSMColumn(size: number) {
  return `col-sm-${size}`;
}

export function getXSColumn(size: number) {
  return `col-xs-${size}`;
}

/**
 * Validate size is a valid bootstrap column size
 *
 * @param {number} size
 * @param {string} property
 */
export function verifyIsValidBSColumnSize(size: number, property: string) {
  if (!environment.production && (!Number.isInteger(size) || !(size > 0 && size <= 12))) {
    throw new Error(`column size ${size.toPrecision(2)} is not a valid bootstrap column size ${property}`);
  }
}

export function verifyIsWithinBSBoundaries(size: number) {
  if (!environment.production && (!Number.isInteger(size) || !(size >= 0 && size <= 12))) {
    throw new Error(`column size ${size.toPrecision(2)} does not meet boundaries(1-12) of bootstrap grid system`);
  }
}
