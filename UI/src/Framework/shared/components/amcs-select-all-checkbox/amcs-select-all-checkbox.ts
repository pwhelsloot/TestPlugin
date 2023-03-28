import { AfterViewInit, Component, ElementRef, Host, Input, OnDestroy, Renderer2, ViewChild } from '@angular/core';
import { MatCheckbox, MatCheckboxChange } from '@angular/material/checkbox';
import { MatSelect } from '@angular/material/select';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-amcs-select-all-checkbox',
  templateUrl: './amcs-select-all-checkbox.html'
})
export class AmcsSelectAllCheckboxComponent extends AutomationLocatorDirective implements AfterViewInit, OnDestroy {
  @Input() text = 'Select all';
  @ViewChild('selectAllCheckbox') selectAllCheckbox: MatCheckbox;

  constructor(@Host() private matSelect: MatSelect,
    protected elRef: ElementRef, protected renderer: Renderer2) {
    super(elRef, renderer);
  }
  private availableOptions = [];
  private selectedOptions = [];

  private subscriptions = new Array<Subscription>();

  ngAfterViewInit() {
    // timeout is needed here for when the component is used within an ngIf
    setTimeout(() => {
      this.availableOptions = this.matSelect.options.map(x => x.value);
      this.subscriptions.push(
        this.matSelect.options.changes.subscribe(() => {
          this.availableOptions = this.matSelect.options.map(x => x.value);
          this.updateState();
        })
      );
    });

    this.selectedOptions = this.matSelect.ngControl.control.value;
    this.subscriptions.push(
      this.matSelect.ngControl.valueChanges.subscribe(res => {
        this.selectedOptions = res;
        this.updateState();
      })
    );
    setTimeout(() => {
      this.updateState();
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  toggleSelection($event: MatCheckboxChange) {
    if ($event.checked) {
      this.matSelect.ngControl.control.setValue(this.availableOptions);
      this.setChecked(true);
    } else {
      this.matSelect.ngControl.control.setValue([]);
      this.setChecked(false);
    }
  }

  private updateState() {
    if (this.areAllSelected()) {
      this.setChecked(true);
    } else {
      this.setChecked(false);
    }
  }

  private setChecked(isChecked: boolean) {
    this.selectAllCheckbox.checked = isChecked;
  }

  private areAllSelected() {
    return this.availableOptions.every(a => this.selectedOptions.includes(a));
  }
}
