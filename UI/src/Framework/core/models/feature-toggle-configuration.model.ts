import { alias } from '@coreconfig/api-dto-alias.function';

export class FeatureToggleConfiguration {

    @alias('ShowFeatureCustomerService')
    showFeatureCustomerService: boolean;

    @alias('ShowFeatureRouting')
    showFeatureRouting: boolean;

    @alias('ShowFeatureScale')
    showFeatureScale: boolean;

    @alias('ShowFeatureMaterials')
    showFeatureMaterials: boolean;

    @alias('ShowFeatureEquipmentInventory')
    showFeatureEquipmentInventory: boolean;

    @alias('ShowFeatureAccounting')
    showFeatureAccounting: boolean;

    @alias('ShowFeaturePricesAndProducts')
    showFeaturePricesAndProducts: boolean;

    @alias('ShowFeatureReportsAnalytics')
    showFeatureReportsAnalytics: boolean;

    @alias('ShowFeatureSettings')
    showFeatureSettings: boolean;

    @alias('ShowDesignLibrary')
    showDesignLibrary: boolean;

    @alias('ShowCustomerSiteHealthAndSafetyForms')
    showCustomerSiteHealthAndSafetyForms: boolean;

    @alias('ShowCustomerPaidToDateBrowser')
    showCustomerPaidToDateBrowser: boolean;

    @alias('ShowFeatureExtensibility')
    showFeatureExtensibility: boolean;
}
