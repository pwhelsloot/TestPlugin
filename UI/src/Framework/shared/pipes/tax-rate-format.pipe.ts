import { DecimalPipe, getCurrencySymbol } from '@angular/common';
import { Pipe, PipeTransform } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { TaxRateType } from '@core-module/models/tax-rate-type.enum';

@Pipe({
    name: 'taxRateFormatter'
})
export class TaxRateFormatPipe implements PipeTransform {

    constructor(private decimalPipe: DecimalPipe) { }

    transform(value: number, item: any, currencyCode: string): string {
        let formattedRate: string = value.toString();

        if (isTruthy(item)) {
            switch (item.taxRateTypeId) {
                case TaxRateType.Percentage:
                    formattedRate = this.decimalPipe.transform(value * 100, '1.0-3') + '%';
                    break;

                case TaxRateType.Weight:
                    formattedRate = this.decimalPipe.transform(value, '1.2-2') + ' /' + item.unitOfMeasurement;
                    break;

                case TaxRateType.Flat:
                    formattedRate = this.decimalPipe.transform(value, '1.2-2');
                    break;

                default:
                    formattedRate = this.decimalPipe.transform(value, '1.2-2');
                    break;
            }
        }

        if (isTruthy(currencyCode) && item.taxRateTypeId !== TaxRateType.Percentage) {
            formattedRate = getCurrencySymbol(currencyCode, 'narrow') + formattedRate;
        }

        return formattedRate;
    }
}
