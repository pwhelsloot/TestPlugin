import { alias } from '@coreconfig/api-dto-alias.function';

export class DeveloperButtonsConfiguration {

    @alias('ShowCommunicationsDropdown')
    showCommunicationsDropdown: boolean;

    @alias('ShowServiceLocationsAdd')
    showServiceLocationsAdd: boolean;

    @alias('ShowServiceLocationsDelete')
    showServiceLocationsDelete: boolean;

    @alias('ShowContractsAdd')
    showContractsAdd: boolean;

    @alias('ShowContractsExpander')
    showContractsExpander: boolean;

    @alias('ShowHeaderERPAbout')
    showHeaderERPAbout: boolean;

    @alias('ShowHeaderDevTools')
    showHeaderDevtools: boolean;

    @alias('ShowFinanceEdit')
    showFinanceEdit: boolean;

    @alias('ShowContactDelete')
    showContactDelete: boolean;

    @alias('ShowMissedCollectionLogEvent')
    showMissedCollectionLogEvent: boolean;

    @alias('ShowMissedCollectionCreateCallout')
    showMissedCollectionCreateCallout: boolean;

    @alias('ShowFrequentDashboardActions')
    showFrequentDashboardActions: boolean;

    @alias('ShowSiteOrderActionMenu')
    showSiteOrderActionMenu: boolean;

    @alias('ShowSiteOrderAdd')
    showSiteOrderAdd: boolean;

    @alias('ShowLoadReceiving')
    showLoadReceiving: boolean;

    @alias('ShowAddUnplannedLoads')
    showAddUnplannedLoads: boolean;
}
