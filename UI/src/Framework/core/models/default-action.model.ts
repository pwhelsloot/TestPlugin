import { alias } from '@core-module/config/api-dto-alias.function';
import { nameof } from '@core-module/helpers/name-of.function';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { ComponentFilterPropertyValueType } from '@shared-module/components/amcs-component-filter/component-filter-property-value-type.enum';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from './api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class DefaultAction extends ApiBaseModel implements ILookupItem {

    @amcsJsonMember('DefaultActionId')
    @alias('DefaultActionId')
    id: number;

    @amcsJsonMember('ServiceId')
    @alias('ServiceId')
    serviceId: number;

    @amcsJsonMember('ServiceDescription')
    @alias('ServiceDescription')
    serviceDescription: string;

    @amcsJsonMember('MaterialClassId')
    @alias('MaterialClassId')
    materialClassId: number;

    @amcsJsonMember('MaterialDescription')
    @alias('MaterialDescription')
    materialDescription: string;

    @amcsJsonMember('ActionId')
    @alias('ActionId')
    actionId: number;

    @amcsJsonMember('ActionDescription')
    @alias('ActionDescription')
    actionDescription: string;

    @amcsJsonMember('PricingBasisId')
    @alias('PricingBasisId')
    pricingBasisId: number;

    @amcsJsonMember('PriceBasisDescription')
    @alias('PriceBasisDescription')
    priceBasisDescription: string;

    @amcsJsonMember('VATid')
    @alias('VATid')
    vatId: number;

    @amcsJsonMember('VATRate')
    @alias('VATRate')
    vatRate: number;

    @amcsJsonMember('TaxTemplateCollectionId')
    @alias('TaxTemplateCollectionId')
    taxTemplateCollectionId: number;

    @amcsJsonMember('TaxTemplateDescription')
    @alias('TaxTemplateDescription')
    taxTemplateDescription: string;

    @amcsJsonMember('UnitOfMeasurementId')
    @alias('UnitOfMeasurementId')
    unitOfMeasurementId: number;

    @amcsJsonMember('UnitOfMeasurement')
    @alias('UnitOfMeasurement')
    unitOfMeasurement: string;

    @alias('Description')
    @amcsJsonMember('Description')
    description: string;

    static getGridColumns(translations: string[]): GridColumnConfig[] {
        return [
            new GridColumnConfig(translations['defaultActionSelector.columns.service'], 'serviceDescription', [17.6]),
            new GridColumnConfig(translations['defaultActionSelector.columns.action'], 'actionDescription', [17.6]),
            new GridColumnConfig(translations['defaultActionSelector.columns.material'], 'materialDescription', [17.6]),
            new GridColumnConfig(translations['defaultActionSelector.columns.pricingBasis'], 'priceBasisDescription', [17.6]),
            new GridColumnConfig(translations['defaultActionSelector.columns.unitOfMeasure'], 'unitOfMeasurement', [17.6])
        ];
    }

    static getFilterProperties(translations: string[]): ComponentFilterProperty[] {
        const filterProperties: ComponentFilterProperty[] = [];
        filterProperties.push(new ComponentFilterProperty(1, translations['defaultActionSelector.columns.service'], ComponentFilterPropertyValueType.text, nameof<DefaultAction>('serviceDescription'), true));
        filterProperties.push(new ComponentFilterProperty(2, translations['defaultActionSelector.columns.action'], ComponentFilterPropertyValueType.text, nameof<DefaultAction>('actionDescription'), true));
        filterProperties.push(new ComponentFilterProperty(3, translations['defaultActionSelector.columns.material'], ComponentFilterPropertyValueType.text, nameof<DefaultAction>('materialDescription'), true));
        return filterProperties;
      }


  getTextValueFromPropertyKey(key: string): string {
    switch (key) {
      case nameof<DefaultAction>('serviceDescription'):
        return this.serviceDescription;
      case nameof<DefaultAction>('actionDescription'):
        return this.actionDescription;
      case nameof<DefaultAction>('materialDescription'):
        return this.materialDescription;
      default:
        return null;
    }
  }
}
