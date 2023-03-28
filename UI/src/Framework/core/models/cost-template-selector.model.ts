import { GridColumnConfig } from '@shared-module/components/amcs-grid/grid-column-config';
import { amcsJsonMember, amcsJsonObject, ApiBaseModel } from './api/api-base.model';

/**
 * @deprecated Move to PlatformUI
 */
@amcsJsonObject()
export class CostTemplateSelector extends ApiBaseModel {

  @amcsJsonMember('SupplierSiteCostTemplateId')
  id: number;

  @amcsJsonMember('DefaultActionId')
  defaultActionId: number;

  @amcsJsonMember('ServiceId')
  serviceId: number;

  @amcsJsonMember('Service')
  service: string;

  @amcsJsonMember('MaterialClassId')
  materialClassId: number;

  @amcsJsonMember('Material')
  material: string;

  @amcsJsonMember('ActionId')
  actionId: number;

  @amcsJsonMember('Action')
  action: string;

  @amcsJsonMember('PricingBasisId')
  pricingBasisId: number;

  @amcsJsonMember('PricingBasis')
  pricingBasis: string;

  @amcsJsonMember('VatId')
  vatId: number;

  @amcsJsonMember('VATRate')
  vatRate: number;

  @amcsJsonMember('TaxTemplateCollectionId')
  taxTemplateCollectionId: number;

  @amcsJsonMember('TaxTemplate')
  taxTemplateDescription: string;

  @amcsJsonMember('UnitOfMeasurementId')
  unitOfMeasurementId: number;

  @amcsJsonMember('UnitOfMeasurement')
  unitOfMeasurement: string;

  @amcsJsonMember('SupplierSite')
  supplierSite: string;

  @amcsJsonMember('SupplierSiteId')
  supplierSiteId: number;

  static getGridColumns(translations: string[]): GridColumnConfig[] {
    return [
      new GridColumnConfig(translations['costeTemplateSelector.columns.service'], 'service', [14.6]),
      new GridColumnConfig(translations['costeTemplateSelector.columns.action'], 'action', [14.6]),
      new GridColumnConfig(translations['costeTemplateSelector.columns.material'], 'material', [14.6]),
      new GridColumnConfig(translations['costeTemplateSelector.columns.pricingBasis'], 'pricingBasis', [14.6]),
      new GridColumnConfig(translations['costeTemplateSelector.columns.unitOfMeasure'], 'unitOfMeasurement', [14.6]),
      new GridColumnConfig(translations['costeTemplateSelector.columns.supplierSite'], 'supplierSite', [14.6])
    ];
  }
}
