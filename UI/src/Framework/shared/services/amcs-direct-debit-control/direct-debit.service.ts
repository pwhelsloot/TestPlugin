import { Injectable } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { BaseService } from '@core-module/services/base.service';
import { BankLookup } from '@shared-module/models/amcs-direct-debit-control/bank-lookup.model';
import { DirectDebitForm } from '@shared-module/models/amcs-direct-debit-control/direct-debit-form.model';
import { DirectDebitUniqueKeyRequest } from '@shared-module/models/amcs-direct-debit-control/direct-debit-unique-key-request.model';
import { DirectDebitUniqueKeyResponse } from '@shared-module/models/amcs-direct-debit-control/direct-debit-unique-key-response.model';
import { Observable, of, ReplaySubject, Subject } from 'rxjs';
import { distinctUntilChanged, map, take, takeUntil, throttleTime } from 'rxjs/operators';
import { DirectDebitDataService } from './direct-debit.service.data';

/**
 * @deprecated Move to PlatformUI
 */
@Injectable()
export class DirectDebitService extends BaseService {

    bankNameRequestSubject: Subject<string> = new Subject<string>();
    bankNameResults$: Observable<BankLookup[]>;
    oldRIBKey: string;

    constructor(public dataService: DirectDebitDataService) {
        super();
    }
    private bankNameSearchSubject = new ReplaySubject<BankLookup[]>(1);
    private request: DirectDebitUniqueKeyRequest = new DirectDebitUniqueKeyRequest();

    getUMRNumber(type: string, form: DirectDebitForm) {
        this.request.type = type;
        this.dataService.generateUniqueKey(this.request).pipe(take(1)).subscribe((response: DirectDebitUniqueKeyResponse) => {
            form.umr.setValue(response.uniqueKey);
        });
    }

    getRIBNumber(type: string, form: DirectDebitForm) {
        this.request.type = type;
        this.request.nationalBankCode = form.nationalBankCode.value;
        this.request.nationalCheckDigits = form.nationalCheckDigits.value;
        this.request.sortCode = form.sortCode.value;
        this.request.accountNo = form.accountNo.value;
        this.dataService.generateUniqueKey(this.request).pipe(take(1)).subscribe((response: DirectDebitUniqueKeyResponse) => {
            form.ribNumber.setValue(response.uniqueKey);
            form.nationalCheckDigits.setValue(response.nationalCheckDigits);
        });
    }

    setupBankNameSearchStream(bankList: BankLookup[]) {
        this.bankNameResults$ = this.bankNameSearchSubject.asObservable();
        this.bankNameRequestSubject.pipe(
            distinctUntilChanged(),
            throttleTime(300, undefined, { leading: true, trailing: true }),
            takeUntil(this.unsubscribe),
            map((searchTerm: string) => {
                if (isTruthy(searchTerm)) {
                    const filteredBanks = bankList.filter(p => p.description.toUpperCase().startsWith(searchTerm.toUpperCase()));
                    return filteredBanks;
                } else {
                    return of([]);
                }
            })).subscribe((results: BankLookup[]) => {
                if (isTruthy(results) && results.length > 0) {
                    this.bankNameSearchSubject.next(results);
                } else {
                    this.bankNameSearchSubject.next(null);
                }
            });
    }

    setUpListeners(directDebitForm: DirectDebitForm) {
        if (isTruthy(directDebitForm)) {
            directDebitForm.nationalCheckDigits.valueChanges.pipe(takeUntil(this.unsubscribe), distinctUntilChanged()).subscribe((data: string) => {
                if (isTruthy(this.oldRIBKey) && this.oldRIBKey !== data) {
                    directDebitForm.ribNumber.setValue(null);
                }
                this.oldRIBKey = data;
            });
        }
    }
}
