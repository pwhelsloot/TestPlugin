import { Injectable } from '@angular/core';
import { ErpSaveService } from '../erp-save.service';
import { Observable, BehaviorSubject, Subject } from 'rxjs';
import { BaseService } from '../base.service';
import { takeUntil, combineLatest } from 'rxjs/operators';

@Injectable()
export class FormSavingService extends BaseService {

  formSaving$: Observable<boolean>;

  constructor(private erpSaveService: ErpSaveService) {
    super();
    this.formSaving$ = this.formSavingSubject.asObservable();

    this.formSubmittedSubject.pipe(
      combineLatest(this.erpSaveService.saveAvailable$),
      takeUntil(this.unsubscribe)
    ).subscribe(data => {
      const formSubmitted: boolean = data[0];
      const saveAvailable: boolean = data[1];

      if (formSubmitted && !saveAvailable) {
        this.saveInProgress = true;
        this.formSavingSubject.next(true);
      } else if (formSubmitted && saveAvailable && this.saveInProgress) {
        // reset the state
        this.saveInProgress = false;
        this.formSubmittedSubject.next(false);
      } else {
        // either form submitted is false or the save hasn't started yet
        this.formSavingSubject.next(false);
      }
    });
  }

  private formSavingSubject = new BehaviorSubject<boolean>(false);
  private formSubmittedSubject = new Subject<boolean>();
  private saveInProgress: boolean;

  formSubmitted() {
    this.formSubmittedSubject.next(true);
  }
}
