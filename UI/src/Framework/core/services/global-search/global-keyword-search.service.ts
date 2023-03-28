import { Injectable } from '@angular/core';
import { NavigationEnd, Router } from '@angular/router';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { ARAccountCodeSearchResult } from '@core-module/models/keyword-search/ar-account-code-search.result.model';
import { ContainerSearchResult } from '@core-module/models/keyword-search/container-search-result.model';
import { FederalIdSearchResult } from '@core-module/models/keyword-search/federal-id-search-result.model';
import { InvoiceNumberSearchResult } from '@core-module/models/keyword-search/invoice-number-search-result.model';
import { PrePayCardSearchResult } from '@core-module/models/keyword-search/pre-pay-card-search.result.model';
import { SalesOrderSearchResult } from '@core-module/models/keyword-search/sales-order-search.result.model';
import { TradingNameSearchResult } from '@core-module/models/keyword-search/trading-name-search-result.model';
import { GlobalSearchKeywordQuickSearch } from '@coremodels/global-search/global-search-keyword-quick-search.model';
import { SearchKeywordEnum } from '@shared-module/models/search-keyword.enum';
import { BehaviorSubject, Observable, ReplaySubject, Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged, filter, switchMap, take, takeUntil } from 'rxjs/operators';
import { BarcodeSearchResult } from '../../models/keyword-search/barcode-search.result.model';
import { PurchaseOrderNumberSearchResult } from '../../models/keyword-search/purchase-order-number-search.result.model';
import { PurchaseOrderSearchResult } from '../../models/keyword-search/purchase-order-search.result.model';
import { ReportSearchResult } from '../../models/keyword-search/report-search.result.model';
import { WasteDeclarationSearchResult } from '../../models/keyword-search/waste-declaration-search-result.model';
import { WeighingTicketSearchResult } from '../../models/keyword-search/weighing-ticket-search-result.model';
import { BaseService } from '../base.service';
import { HeaderRoutingSeviceAdapter } from '../header/header-routing.service.abstract';
import { GlobalKeywordSearchServiceData } from './data/global-keyword-search.service.data';

/**
 * @deprecated Move to PlatformUI https://dev.azure.com/amcsgroup/Platform/_workitems/edit/247298
 */
@Injectable({ providedIn: 'root' })
export class GlobalKeywordSearchService extends BaseService {

  quickSearchResults$: Observable<GlobalSearchKeywordQuickSearch[]>;
  containerSearchResult$: Observable<ContainerSearchResult>;
  prepayCardSearchResult$: Observable<PrePayCardSearchResult>;
  federalIdSearchResult$: Observable<FederalIdSearchResult>;
  salesOrderSearchResult$: Observable<SalesOrderSearchResult>;
  purchaseOrderSearchResult$: Observable<PurchaseOrderSearchResult>;
  tradingNameSearchResult$: Observable<TradingNameSearchResult>;
  reportSearchResult$: Observable<ReportSearchResult>;
  wasteDeclarationSearchResult$: Observable<WasteDeclarationSearchResult>;
  invoiceNumberSearchResult$: Observable<InvoiceNumberSearchResult>;
  purchaseOrderNumberSearchResult$: Observable<PurchaseOrderNumberSearchResult>;
  arAccountCodeSearchResult$: Observable<ARAccountCodeSearchResult>;
  weighingTicketSearchResult$: Observable<WeighingTicketSearchResult>;
  barcodeSearchResult$: Observable<BarcodeSearchResult>;

  loading = false;
  loadingResults = false;
  searchString: string = null;
  activeSector: string = null;
  selectedId: number;
  constructor(private dataService: GlobalKeywordSearchServiceData,
    private headerRoutingSevice: HeaderRoutingSeviceAdapter,
    private router: Router) {
    super();
    this.router.events
      .pipe(
        takeUntil(this.unsubscribe),
        filter(event => (event instanceof NavigationEnd))
      )
      .subscribe((event: NavigationEnd) => {
        // After navigation is complete, select menu item based on the url
        this.activeSector = this.headerRoutingSevice.getMenuItemKeyFromUrl(event.url);
      });
    this.setUpQuickSearchStream();
    this.setupContainerSearchResultStream();
    this.setupPrePayCardSearchResultStream();
    this.setupFederalIdSearchResultStream();
    this.setupSalesOrderSearchResultStream();
    this.setupPurchaseOrderNumberSearchResultStream();
    this.setupPurchaseOrderSearchResultStream();
    this.setupTradingNameSearchResultStream();
    this.setupReportSearchResultStream();
    this.setupWasteDeclarationSearchResultStream();
    this.setupInvoiceNumberSearchResultStream();
    this.setupWeighingTicketSearchResultStream();
    this.setupBarcodeSearchResultStream();
    this.setupARAccountCodeSearchResultStream();
  }

  private quickSearchResults: ReplaySubject<GlobalSearchKeywordQuickSearch[]> = new ReplaySubject<GlobalSearchKeywordQuickSearch[]>(1);
  private selectedQuickSearchResult = new BehaviorSubject<GlobalSearchKeywordQuickSearch>(null);
  private requestQuickSearchSubject: Subject<{ searchTerm: string; keyword: SearchKeywordEnum }> = new Subject<{ searchTerm: string; keyword: SearchKeywordEnum }>();

  private containerSearchResultSubject = new ReplaySubject<ContainerSearchResult>(1);
  private containerSearchResultRequestSubject = new Subject<number>();

  private prepayCardSearchResult = new ReplaySubject<PrePayCardSearchResult>(1);
  private prepayCardSearchResultRequestSubject = new Subject<number>();

  private federalIdSearchResult = new ReplaySubject<FederalIdSearchResult>(1);
  private federalIdSearchResultRequestSubject = new Subject<[number, number]>();

  private salesOrderSearchResult = new ReplaySubject<SalesOrderSearchResult>(1);
  private salesOrderSearchResultRequestSubject = new Subject<number>();

  private purchaseOrderSearchResult = new ReplaySubject<PurchaseOrderSearchResult>(1);
  private purchaseOrderSearchResultRequestSubject = new Subject<number>();

  private tradingNameSearchResult = new ReplaySubject<TradingNameSearchResult>(1);
  private tradingNameSearchResultRequestSubject = new Subject<number>();

  private reportSearchResult = new ReplaySubject<ReportSearchResult>(1);
  private reportSearchResultRequestSubject = new Subject<number>();

  private wasteDeclarationSearchResult = new ReplaySubject<WasteDeclarationSearchResult>(1);
  private wasteDeclarationSearchResultRequestSubject = new Subject<number>();

  private invoiceNumberSearchResult = new ReplaySubject<InvoiceNumberSearchResult>(1);
  private invoiceNumberSearchResultRequestSubject = new Subject<number>();

  private purchaseOrderNumberSearchResult = new ReplaySubject<PurchaseOrderNumberSearchResult>(1);
  private purchaseOrderNumberSearchResultRequestSubject = new Subject<number>();

  private arAccountCodeSearchResult = new ReplaySubject<ARAccountCodeSearchResult>(1);
  private arAccountCodeSearchResultRequestSubject = new Subject<number>();

  private weighingTicketSearchResult = new ReplaySubject<WeighingTicketSearchResult>(1);
  private weighingTicketSearchResultRequestSubject = new Subject<number>();

  private barcodeSearchResult = new ReplaySubject<BarcodeSearchResult>(1);
  private barcodeSearchResultRequestSubject = new Subject<[number, string]>();

  requestQuickSearch(searchTerm: string, keyword: SearchKeywordEnum) {
    this.selectedQuickSearchResult.pipe(take(1)).subscribe((selectedItem: GlobalSearchKeywordQuickSearch) => {
      // This code is bit of a hack to get around https://github.com/angular/angular/issues/12540 - problem with autocomplete control.
      // Once we select a autocomplete option we DON'T want to make an api call, this prevents that.
      if (!isTruthy(selectedItem) || searchTerm !== selectedItem.description) {
        this.requestQuickSearchSubject.next({ searchTerm, keyword });
      } else {
        // We must of selected an autocomplete option, no more quick search results
        this.clearQuickSearchResults();
      }
    });
  }

  quickSearchResultSelected(selectedItem: GlobalSearchKeywordQuickSearch) {
    this.selectedQuickSearchResult.next(selectedItem);
  }

  clearQuickSearchResults() {
    this.selectedQuickSearchResult.next(null);
  }

  private setUpQuickSearchStream() {
    this.quickSearchResults$ = this.quickSearchResults.asObservable();
    this.requestQuickSearchSubject
      .pipe(
        distinctUntilChanged(),
        takeUntil(this.unsubscribe),
        debounceTime(300),
        switchMap((data: { searchTerm: string; keyword: SearchKeywordEnum }) => {
          this.loading = true;
          this.searchString = data.searchTerm.trim();
          switch (data.keyword) {
            case SearchKeywordEnum.CON:
              return this.dataService.doContainerKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.INV:
              return this.dataService.doInvoiceNumberKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.PPC:
              return this.dataService.doPrePayCardKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.FED:
              return this.dataService.doFederalIdKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.SON:
              return this.dataService.doSalesOrderNoKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.PON:
              if (isTruthy(this.activeSector) && (this.activeSector === 'materialManagement')) {

                return this.dataService.doPurchaseOrderNoKeywordQuickSearch(this.searchString);
              } else {

                return this.dataService.doPurchaseOrderNumberNoKeywordQuickSearch(this.searchString);
              }

            case SearchKeywordEnum.TNA:
              return this.dataService.doTradingNameNoKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.RPT:
              return this.dataService.doReportKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.WDN:
              return this.dataService.doWasteDeclarationNoKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.STN:
              return this.dataService.doWeighingTicketNoKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.BCN:
              return this.dataService.doBarCodeNoKeywordQuickSearch(this.searchString);

            case SearchKeywordEnum.ACC:
              return this.dataService.doARAccountCodeQuickSearch(this.searchString);

            case SearchKeywordEnum.MAT:
              return this.dataService.doMatrikkelnummerKeywordQuickSearch(this.searchString);
          }
        })
      )
      .subscribe((result: GlobalSearchKeywordQuickSearch[]) => {
        this.quickSearchResults.next(result);
        this.loading = false;
      });

    this.selectedQuickSearchResult.pipe(takeUntil(this.unsubscribe)).subscribe((selectedItem: GlobalSearchKeywordQuickSearch) => {
      if (selectedItem === null) {
        this.clearAllKeywordResults();
      } else {
        this.requestSearch(selectedItem.id, selectedItem.type, selectedItem.customerSiteId, selectedItem.description);
      }
    });
  }

  private clearAllKeywordResults() {
    this.quickSearchResults.next([]);
    this.containerSearchResultSubject.next(null);
    this.prepayCardSearchResult.next(null);
    this.federalIdSearchResult.next(null);
    this.salesOrderSearchResult.next(null);
    this.purchaseOrderSearchResult.next(null);
    this.tradingNameSearchResult.next(null);
    this.wasteDeclarationSearchResult.next(null);
    this.invoiceNumberSearchResult.next(null);
    this.purchaseOrderNumberSearchResult.next(null);
    this.reportSearchResult.next(null);
    this.weighingTicketSearchResult.next(null);
    this.barcodeSearchResult.next(null);
    this.arAccountCodeSearchResult.next(null);
  }

  private requestSearch(id: number, keyword: SearchKeywordEnum, customerSiteId: number, description: string) {
    this.loadingResults = true;
    this.selectedId = null;
    switch (keyword) {
      case SearchKeywordEnum.CON: {
        this.containerSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.PPC: {
        this.prepayCardSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.FED: {
        this.federalIdSearchResultRequestSubject.next([id, customerSiteId]);
        break;
      }
      case SearchKeywordEnum.SON: {
        this.salesOrderSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.PON:
        if (isTruthy(this.activeSector) && (this.activeSector === 'materialManagement')) {
          this.purchaseOrderSearchResultRequestSubject.next(id);
          break;
        } else {
          this.purchaseOrderNumberSearchResultRequestSubject.next(id);
          break;
        }
      case SearchKeywordEnum.TNA: {
        this.tradingNameSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.RPT: {
        this.reportSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.WDN: {
        this.wasteDeclarationSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.INV: {
        this.invoiceNumberSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.STN: {
        this.weighingTicketSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.BCN: {
        this.barcodeSearchResultRequestSubject.next([id, description]);
        break;
      }
      case SearchKeywordEnum.ACC: {
        this.arAccountCodeSearchResultRequestSubject.next(id);
        break;
      }
      case SearchKeywordEnum.MAT: {
        this.selectedId = id;
        break;
      }
    }
  }

  private setupPrePayCardSearchResultStream() {
    this.prepayCardSearchResult$ = this.prepayCardSearchResult.asObservable();
    this.prepayCardSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((prepayCardId: number) => {
        return this.dataService.getPrePayCardSearchResult(prepayCardId);
      })
    ).subscribe((result: PrePayCardSearchResult) => {
      this.prepayCardSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupSalesOrderSearchResultStream() {
    this.salesOrderSearchResult$ = this.salesOrderSearchResult.asObservable();
    this.salesOrderSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((salesOrderId: number) => {
        return this.dataService.getSalesOrderSearchResult(salesOrderId);
      })
    ).subscribe((result: SalesOrderSearchResult) => {
      this.salesOrderSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupPurchaseOrderSearchResultStream() {
    this.purchaseOrderSearchResult$ = this.purchaseOrderSearchResult.asObservable();
    this.purchaseOrderSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((purchaseOrderId: number) => {
        return this.dataService.getPurchaseOrderSearchResult(purchaseOrderId);
      })
    ).subscribe((result: PurchaseOrderSearchResult) => {
      this.purchaseOrderSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupFederalIdSearchResultStream() {
    this.federalIdSearchResult$ = this.federalIdSearchResult.asObservable();
    this.federalIdSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((data: [number, number]) => {
        return this.dataService.getFederalIdSearchResult(data[0], data[1]);
      })
    ).subscribe((result: FederalIdSearchResult) => {
      this.federalIdSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupContainerSearchResultStream() {
    this.containerSearchResult$ = this.containerSearchResultSubject.asObservable();
    this.containerSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((containerId: number) => {
        return this.dataService.getContainerSearchResult(containerId);
      })
    ).subscribe(result => {
      this.containerSearchResultSubject.next(result);
      this.loadingResults = false;
    });
  }

  private setupTradingNameSearchResultStream() {
    this.tradingNameSearchResult$ = this.tradingNameSearchResult.asObservable();
    this.tradingNameSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((CustomerTradingNameId: number) => {
        return this.dataService.getTradingNameSearchResult(CustomerTradingNameId);
      })
    ).subscribe((result: TradingNameSearchResult) => {
      this.tradingNameSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupReportSearchResultStream() {
    this.reportSearchResult$ = this.reportSearchResult.asObservable();
    this.reportSearchResultRequestSubject
      .pipe(
        takeUntil(this.unsubscribe),
        switchMap((reportId: number) => {
          return this.dataService.getReportSearchResult(reportId);
        })
      ).subscribe((result: ReportSearchResult) => {
        this.reportSearchResult.next(result);
        this.loadingResults = false;
      });
  }

  private setupWasteDeclarationSearchResultStream() {
    this.wasteDeclarationSearchResult$ = this.wasteDeclarationSearchResult.asObservable();
    this.wasteDeclarationSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((WasteDeclarationTransactionId: number) => {
        return this.dataService.getWasteDeclarationSearchResult(WasteDeclarationTransactionId);
      })
    ).subscribe((result: WasteDeclarationSearchResult) => {
      this.wasteDeclarationSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupInvoiceNumberSearchResultStream() {
    this.invoiceNumberSearchResult$ = this.invoiceNumberSearchResult.asObservable();
    this.invoiceNumberSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((invoiceId: number) => {
        return this.dataService.getInvoiceNumberSearchResult(invoiceId);
      })
    ).subscribe((result: InvoiceNumberSearchResult) => {
      this.invoiceNumberSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupPurchaseOrderNumberSearchResultStream() {
    this.purchaseOrderNumberSearchResult$ = this.purchaseOrderNumberSearchResult.asObservable();
    this.purchaseOrderNumberSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((PONumberId: number) => {
        return this.dataService.getPurchaseOrderNumberSearchResult(PONumberId);
      })
    ).subscribe((result: PurchaseOrderNumberSearchResult) => {
      this.purchaseOrderNumberSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupWeighingTicketSearchResultStream() {
    this.weighingTicketSearchResult$ = this.weighingTicketSearchResult.asObservable();
    this.weighingTicketSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((weighingId: number) => {
        return this.dataService.getWeighingTicketSearchResult(weighingId);
      })
    ).subscribe((result: WeighingTicketSearchResult) => {
      this.weighingTicketSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupARAccountCodeSearchResultStream() {
    this.arAccountCodeSearchResult$ = this.arAccountCodeSearchResult.asObservable();
    this.arAccountCodeSearchResultRequestSubject.pipe(
      takeUntil(this.unsubscribe),
      switchMap((customerId: number) => {
        return this.dataService.getARAccountCodeSearchResult(customerId);
      })
    ).subscribe((result: ARAccountCodeSearchResult) => {
      this.arAccountCodeSearchResult.next(result);
      this.loadingResults = false;
    });
  }

  private setupBarcodeSearchResultStream() {
    this.barcodeSearchResult$ = this.barcodeSearchResult.asObservable();
    this.barcodeSearchResultRequestSubject
      .pipe(
        takeUntil(this.unsubscribe),
        switchMap((data: [number, string]) => {
          return this.dataService.getBarcodeSearchResult(data[0], data[1]);
        })
      ).subscribe((result: BarcodeSearchResult) => {
        this.barcodeSearchResult.next(result);
        this.loadingResults = false;
      });
  }
}
