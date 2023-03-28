import { EventEmitter, Injectable, QueryList } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AmcsTabComponent } from '@shared-module/components/amcs-tab/amcs-tab.component';
import { AmcsFormGroup } from '@shared-module/forms/AmcsFormGroup.model';
import { BaseFormGroup } from '@shared-module/forms/base-form-group.model';
import { Observable, of, zip } from 'rxjs';
import { filter, switchMap, take } from 'rxjs/operators';

@Injectable()
export class AmcsTabControlService {
    title: string;
    tabContentHeight: string;
    onSelectedTabIdChanged: EventEmitter<number> = new EventEmitter<number>();
    onSelectedTabIdBeforeChange: EventEmitter<number> = new EventEmitter<number>();
    onSelectedTabInitialised: EventEmitter<string> = new EventEmitter<string>();
    onSubmit: EventEmitter<any> = new EventEmitter<any>();
    onCancel: EventEmitter<any> = new EventEmitter<any>();
    tabs: QueryList<AmcsTabComponent> = new QueryList<AmcsTabComponent>();
    isReadOnlyMode = false;
    isCurrentTabValid = true;

    get currentTabIndex(): number {
        return this._currentTabIndex;
    }

    set currentTabIndex(newIndex: number) {
        // RDM - Need this as we now set currentTabIndex programatically via 'setCurrentTabWithId'.
        // this causes this method to run, then it runs again as mat-tab-group element fires a change event
        if (this._currentTabIndex === newIndex) {
            return;
        }

        // If previous value exists then check its validility before changing
        if (this._currentTabIndex !== undefined && this._currentTabIndex !== null) {
            this.checkCurrentTabValid();
        }
        const tab: AmcsTabComponent = this.getTabAt(newIndex);
        this.onSelectedTabIdBeforeChange.emit(tab.innerComponent.id);
        this._currentTabIndex = newIndex;
        this.tabChanged();
    }

    private _currentTabIndex: number;

    submit() {
        if (!this.isReadOnlyMode) {
            this.allTabsValid().subscribe((valid: boolean) => {
                if (valid) {
                    this.onSubmit.emit(true);
                } else {
                    this.currentTabIndex = this.indexOfFirstInvalidTab();
                    this.onSubmit.emit(false);
                }
            });
        }
    }

    cancel() {
        this.tabs.forEach((element) => {
            if (element.innerComponent.formService.form != null) {
                this.getFormGroup(element.innerComponent.formService.form).reset();
            }
        });
        this.onCancel.emit(true);
    }

    setCurrentTabWithId(id: number) {
        this.currentTabIndex = this.tabs.toArray().indexOf(this.getTabWithId(id));
    }

    private getTabAt(index: number): AmcsTabComponent {
        if (this.tabs.toArray().length - 1 < index) {
            index = this.tabs.toArray().length - 1;
        }
        return this.tabs.toArray()[index];
    }

    private getTabWithId(id: number): AmcsTabComponent {
        return this.tabs.toArray().find((x) => x.innerComponent.id === id);
    }

    private tabChanged() {
        const tab: AmcsTabComponent = this.getTabAt(this.currentTabIndex);
        this.initialisedTab(tab);
        this.onSelectedTabIdChanged.emit(tab.innerComponent.id);
    }

    private checkCurrentTabValid() {
        const currentTab = this.getTabAt(this.currentTabIndex);
        this.checkTabValid(currentTab);
    }

    private checkTabValid(tab: AmcsTabComponent) {
        // First check all linked tabs
        if (tab.innerComponent.linkedIds != null) {
            tab.innerComponent.linkedIds.forEach((tabId) => {
                const linkedTab = this.getTabWithId(tabId);
                this.checkTabValid(linkedTab);
            });
        }

        this.initialisedTab(tab);
        // now check tab is valid once initialised
        tab.innerComponent.formService.initialised
            .pipe(
                filter((x) => x),
                take(1)
            )
            .subscribe(() => {
                tab.innerComponent.formService
                    .checkValidation()
                    .pipe(take(1))
                    .subscribe((valid: boolean) => {
                        if (valid) {
                            tab.state.error = false;
                            this.isCurrentTabValid = true;
                        } else {
                            if (tab.innerComponent.formService.form != null) {
                                this.markFormGroupTouched(this.getFormGroup(tab.innerComponent.formService.form));
                            }
                            tab.state.error = true;
                            this.isCurrentTabValid = false;
                        }
                    });
            });
    }

    // Really should move this to a helper in future
    private markFormGroupTouched(formGroup: FormGroup) {
        if (formGroup != null && formGroup.status !== 'DISABLED') {
            (Object as any).values(formGroup.controls).forEach((group: FormGroup) => {
                group.markAsTouched();

                if (group.controls) {
                    (Object as any).values(group.controls).forEach((innerGroup: FormGroup) => {
                        innerGroup.markAsTouched();
                        if (innerGroup.controls) {
                            this.markFormGroupTouched(innerGroup);
                        }
                    });
                }
            });
        }
    }

    // Complicated but we need to ensure we don't go firing the 'submit' event until
    // we are happy that all tabs needed are initalised and their validation has been checked.
    private allTabsValid(): Observable<boolean> {
        // Build a list of all unique tabs to check
        const tabsToCheck: AmcsTabComponent[] = [];
        this.tabs.forEach((tab) => {
            tab.innerComponent.formService.initialised
                .pipe(
                    filter((x) => x),
                    take(1)
                )
                .subscribe(() => {
                    this.pushTabToList(tabsToCheck, tab);
                    if (tab.innerComponent.linkedIds != null) {
                        tab.innerComponent.linkedIds.forEach((tabId) => {
                            const linkedTab = this.getTabWithId(tabId);
                            this.pushTabToList(tabsToCheck, linkedTab);
                        });
                    }
                });
        });

        // Now we initalise all these tabs and build a collection of initalised = 'true' Obserables
        const initalisedStreams: Observable<boolean>[] = [];
        tabsToCheck.forEach((tab) => {
            this.initialisedTab(tab);
            initalisedStreams.push(tab.innerComponent.formService.initialised.pipe(filter((x) => x)));
        });

        // Now we combine all these observables into one stream, this means it'll fire once after all
        // initalisedStreams have set initalised = 'true'
        return zip(...initalisedStreams).pipe(
            switchMap(() => {
                // Now we iterate back over our list of tabs to check and run the validation code.
                tabsToCheck.forEach((tab) => {
                    tab.innerComponent.formService
                        .checkValidation()
                        .pipe(take(1))
                        .subscribe((valid: boolean) => {
                            if (valid) {
                                tab.state.error = false;
                            } else {
                                if (tab.innerComponent.formService.form != null) {
                                    this.markFormGroupTouched(this.getFormGroup(tab.innerComponent.formService.form));
                                }
                                tab.state.error = true;
                            }
                        });
                });
                // Finally we return the results, is every tab without error?
                return of(this.tabs.toArray().every((t) => !t.state.error));
            })
        );
    }

    private pushTabToList(tabsToCheck: AmcsTabComponent[], tab: AmcsTabComponent) {
        if (!tabsToCheck.some((x) => x.innerComponent.id === tab.innerComponent.id)) {
            tabsToCheck.push(tab);
        }
    }

    private indexOfFirstInvalidTab(): number {
        let indexOfInvalidTab = 0;
        const tabsArray = this.tabs.toArray();
        if (tabsArray[this._currentTabIndex].state.error) {
            return this._currentTabIndex;
        }
        for (let index = 0; index < this.tabs.length; index++) {
            const element = tabsArray[index];
            if (!element.state.error) {
                indexOfInvalidTab++;
            } else {
                break;
            }
        }
        return indexOfInvalidTab;
    }

    private initialisedTab(tab: AmcsTabComponent) {
        tab.innerComponent.formService.initialised.pipe(take(1)).subscribe((initialised: boolean) => {
            if (!initialised) {
                tab.innerComponent.formService.initialise();
                tab.innerComponent.formService.initialised
                    .pipe(
                        filter((x) => x),
                        take(1)
                    )
                    .subscribe(() => {
                        this.onSelectedTabInitialised.emit(tab.innerComponent.id.toString());
                        if (this.isReadOnlyMode && tab.innerComponent.formService.form != null) {
                            this.getFormGroup(tab.innerComponent.formService.form).disable();
                        }
                    });
            }
        });
    }

    private getFormGroup(form: AmcsFormGroup | BaseFormGroup): FormGroup {
        if (form instanceof AmcsFormGroup) {
            return form.formGroup;
        } else if (form instanceof BaseFormGroup) {
            return form.htmlFormGroup;
        } else {
            throw new Error('Error, form group type not recognised');
        }
    }
}
