import { environment } from '@environments/environment';
export class CoreAppRoutes {
  static notFound = 'not-found';

  static index = environment.applicationURLPrefix + '/index';

  // TODO The below are referenced in framework and so must live here for now till the code
  // referencing it is also removed from framework

  /**
  * @todo The below are referenced in framework and so must live here for now till the code
  * referencing it is also removed from framework
  */
  static customerModule = environment.applicationURLPrefix + '/customer';
  static dashboard = 'dashboard';
  static communications = 'communications';
  static contact = 'contact';

  static priceChangeModule = environment.applicationURLPrefix + '/price/priceChange';
  static priceChangeDashboard = 'dashboard';

  static userModule = environment.applicationURLPrefix + '/user';
  static customerServiceDashboard = 'customerServiceDashboard';
  static userCommunications = 'communications';
  static communication = 'communication';

  static homeModule = environment.applicationURLPrefix + '/home';
  static devHome = 'devHome';
  static myProfile = 'myProfile';
  static report = 'report';
  static search = 'search';
  static searchcontainer = 'container';
  static searchprepaycard = 'prepayCard';
  static searchfederalid = 'searchFederalId';
  static searchinvoicenumber = 'searchInvoiceNumber';
  static searchsalesordernumber = 'searchOutboundOrderNumber';
  static searchpurchaseordernumber = 'searchPurchaseOrderNumber';
  static searchpurchaseorder = 'searchPurchaseOrder';
  static searchtradingname = 'searchTradingName';
  static searchwastedeclarationnumber = 'searchWasteDeclarationNumber';
  static searchreport = 'searchReport';
  static searchweighingticket = 'searchWeighingTicket';
  static searchbarcode = 'searchBarcode';
  static searcharaccountcode = 'searchArAccountCode';
  static searchmatrikkelnummer = 'searchMatrikkelnummer';
}
