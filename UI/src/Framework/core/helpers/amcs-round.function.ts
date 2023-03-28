export function roundValue(value: number, numberOfDecimals: number, doubleRounding = false) {
  const additionalRounding = doubleRounding ? 1 : 0;
  for (let i = additionalRounding; i >= 0; i--) {
    value = value < 0 ?
    (Math.round(Math.abs(value) * Math.pow(10, numberOfDecimals + i)) / Math.pow(10, numberOfDecimals + i)) * -1
    : Math.round(value * Math.pow(10, numberOfDecimals + i)) / Math.pow(10, numberOfDecimals + i);
  }
  return value;
}
