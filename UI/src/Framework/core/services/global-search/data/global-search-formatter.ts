import { AddressFieldDisplayConfiguration } from '@core-module/models/address/address-field-display-configuration.model';
import { CustomerStateEnum } from '@core-module/models/customer-state.enum';
import { AddressFieldLabelOptionsEnum } from '@coremodels/address/address-field-label-options.enum';
import { isTruthy } from '../../../helpers/is-truthy.function';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
export class GlobalSearchFormatter {

    static setDisplayServices(object) {
        const serviceStrings: string[] = [];
        if (object.orders != null) {
            object.orders.forEach(element => {
                if (serviceStrings.find(x => x === element.serviceName) == null) {
                    serviceStrings.push(element.serviceName);
                }
            });
        }
        object.displayServices = serviceStrings.join(', ');
    }

    static setDisplayOrder(object) {
        const orderStrings: string[] = [];
        this.ifOrderNotNullPush(orderStrings, object.orderNo);
        this.ifOrderNotNullPush(orderStrings, object.serviceName);
        this.ifOrderNotNullPush(orderStrings, object.materialProfile);
        this.ifOrderNotNullPush(orderStrings, object.containerType);
        object.displayOrder = orderStrings.join(', ');
    }

    static setDisplayAddress(object) {
        const addressStrings: string[] = [];
        this.ifAddressNotNullPush(addressStrings, object.houseNumber);
        this.ifAddressNotNullPush(addressStrings, object.address1);
        this.ifAddressNotNullPush(addressStrings, object.address2);
        this.ifAddressNotNullPush(addressStrings, object.address3);
        this.ifAddressNotNullPush(addressStrings, object.address4);
        this.ifAddressNotNullPush(addressStrings, object.postcode);
        this.ifAddressNotNullPush(addressStrings, object.address5);
        object.displayAddress = addressStrings.join(', ');
    }

    static getFormattedFullAddress(displayFormat: AddressFieldDisplayConfiguration[], object) {
        const addressStrings: string[] = [];

        const geonorgeLabelsAsStringArray = [
            AddressFieldLabelOptionsEnum[AddressFieldLabelOptionsEnum.KommuneNr],
            AddressFieldLabelOptionsEnum[AddressFieldLabelOptionsEnum.Bnr],
            AddressFieldLabelOptionsEnum[AddressFieldLabelOptionsEnum.Fnr],
            AddressFieldLabelOptionsEnum[AddressFieldLabelOptionsEnum.Gnr],
            AddressFieldLabelOptionsEnum[AddressFieldLabelOptionsEnum.Snr]
        ];

        displayFormat?.filter(x => x.isVisible).sort((a, b) => a.order - b.order).forEach(displayConfig => {
            const propName: string = this.getCamelCase(displayConfig.name);
            let addressPart = this.mapGeonorgePrefixIfExists(object, propName, geonorgeLabelsAsStringArray, displayConfig);
            this.ifAddressNotNullPush(addressStrings, addressPart);
        });
        object.displayAddress = addressStrings.join(', ');
    }

    static setCustomerStatus(object) {
        if (isTruthy(object) && isTruthy(object.customerStateId)) {
            if (object.customerStateId === CustomerStateEnum.Active) {
                object.stateClass = 'state-active';
            } else if (object.customerStateId === CustomerStateEnum.Suspended
                || object.customerStateId === CustomerStateEnum.Closed
                || object.customerStateId === CustomerStateEnum.Liquidated) {
                object.stateClass = 'state-closed';
            } else {
                object.stateClass = 'state-unknown';
            }
        }
    }

    private static ifOrderNotNullPush(orderStrings: string[], orderPart: string) {
        if (orderPart != null && orderPart !== '') {
            orderStrings.push(orderPart);
        }
    }

    private static ifAddressNotNullPush(addressStrings: string[], addressPart: string) {
        if (addressPart != null && addressPart !== '') {
            addressStrings.push(addressPart);
        }
    }

    private static getCamelCase(text: string) {
        return text.charAt(0).toLowerCase() + text.substr(1);
    }

    private static mapGeonorgePrefixIfExists(object, propName: string, geonorgeLabelsAsStringArray: any[], displayConfig: AddressFieldDisplayConfiguration) {
        let addressPart = object[propName];
        if (addressPart != null && addressPart.length > 0) {
            if (geonorgeLabelsAsStringArray.includes(displayConfig.text) && displayConfig.text !== 'KommuneNr') {
                addressPart = `${displayConfig.text}: ${addressPart}`;
            } else if (displayConfig.text === 'KommuneNr') {
                addressPart = 'Knr: ' + addressPart;
            }
        }
        return addressPart;
    }
}
