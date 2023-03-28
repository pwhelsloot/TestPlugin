export class AddressFormatter {

  static getFullGenericAddress(houseNumber: string, address1: string, address2: string, address3: string, address4: string, address5: string, postcode: string,
    address6?: string, address7?: string, address8?: string, address9?: string) {
    const addressStrings: string[] = [];
    this.ifAddressNotNullPush(addressStrings, houseNumber);
    this.ifAddressNotNullPush(addressStrings, address1);
    this.ifAddressNotNullPush(addressStrings, address2);
    this.ifAddressNotNullPush(addressStrings, address3);
    this.ifAddressNotNullPush(addressStrings, address4);
    this.ifAddressNotNullPush(addressStrings, postcode);
    this.ifAddressNotNullPush(addressStrings, address5);
    this.ifAddressNotNullPush(addressStrings, address6);
    this.ifAddressNotNullPush(addressStrings, address7);
    this.ifAddressNotNullPush(addressStrings, address8);
    this.ifAddressNotNullPush(addressStrings, address9);
    return addressStrings.join(', ');
  }

  private static ifAddressNotNullPush(addressStrings: string[], addressPart: string) {
    if (addressPart != null && addressPart.length > 0) {
      addressStrings.push(addressPart);
    }
  }
}
