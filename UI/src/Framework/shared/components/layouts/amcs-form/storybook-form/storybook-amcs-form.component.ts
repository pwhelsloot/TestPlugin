import { Component, Input, OnChanges, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { AmcsNotificationService } from '@core-module/services/amcs-notification.service';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { FormTileOptions } from '@shared-module/components/layouts/amcs-form-tile/form-tile-options.model';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { Subject, Subscription } from 'rxjs';
import { debounceTime, filter, tap } from 'rxjs/operators';
import { StorybookAmcsFormModel } from './storybook-amcs-form-data.model';
import { StoryBookAmcsForm } from './storybook-amcs-form.model';

@Component({
  selector: 'app-storybook-amcs-form',
  templateUrl: './storybook-amcs-form.component.html'
})
export class StorybookAmcsFormComponent implements OnInit, OnChanges, OnDestroy {
  tablesLoading = false;
  tables: { id: number; description: string; reserved: string[] }[] = [];
  tableTextSubject = new Subject<string>();

  dropdownItems: ILookupItem[] = [
    { id: 1, description: 'Breakfast' },
    { id: 2, description: 'Lunch' },
    { id: 3, description: 'Dinner' }
  ];

  options: ILookupItem[] = [
    { id: 1, description: 'Option 1' },
    { id: 2, description: 'Option 2' },
    { id: 3, description: 'Option 3' }
  ];
  dateConfig = new AmcsDatepickerConfig();
  form: StoryBookAmcsForm = null;
  formTileOptions = new FormTileOptions();

  @Input() showFormTile = true;
  @Input() featureTitle: string = null;
  @Input() editorTitle: string = null;
  @Input() enableReturn = false;
  @Input() enableLog = false;
  @Input() enableSave = true;
  @Input() enableContinue = false;
  @Input() enableBack = false;
  @Input() enableCancel = true;
  @Input() enableDelete = false;

  constructor(private formBuilder: FormBuilder, private notificationService: AmcsNotificationService) {}

  private allTables: { id: number; description: string; reserved: string[] }[] = [];
  private tableSubscription: Subscription;

  ngOnInit(): void {
    this.setUpTables();
    this.form = AmcsFormBuilder.buildForm(this.formBuilder, new StorybookAmcsFormModel(), StoryBookAmcsForm);
    this.notificationService.normalDuration = 1000;
  }

  ngOnChanges(): void {
    this.formTileOptions = new FormTileOptions();
    this.formTileOptions.featureTitle = this.featureTitle;
    this.formTileOptions.editorTitle = this.editorTitle;
    this.formTileOptions.enableReturn = this.enableReturn;
    this.formTileOptions.enableLog = this.enableLog;
    this.formTileOptions.formOptions.actionOptions.enableSave = this.enableSave;
    this.formTileOptions.formOptions.actionOptions.enableContinue = this.enableContinue;
    this.formTileOptions.formOptions.actionOptions.enableBack = this.enableBack;
    this.formTileOptions.formOptions.actionOptions.enableCancel = this.enableCancel;
    this.formTileOptions.formOptions.actionOptions.checkPristine = true;
    this.formTileOptions.formOptions.actionOptions.disableSave = false;
    this.formTileOptions.formOptions.actionOptions.enableDelete = this.enableDelete;
  }
  ngOnDestroy() {
    this.tableSubscription?.unsubscribe();
  }

  onSave() {
    const dataModel: StorybookAmcsFormModel = AmcsFormBuilder.parseForm(this.form, StoryBookAmcsForm);
    this.allTables[dataModel.table.id - 1].reserved.push(`${dataModel.date} - ${dataModel.dropdownId}`);
    this.notificationService.showNotification('Booking Saved');
    this.tables = [];
  }
  private setUpTables() {
    this.allTables = [
      { id: 1, description: 'Inside 1', reserved: [] },
      { id: 2, description: 'Inside 2', reserved: [] },
      { id: 3, description: 'Inside 3', reserved: [] },
      { id: 4, description: 'Inside 4', reserved: [] },
      { id: 5, description: 'Inside 5', reserved: [] },
      { id: 6, description: 'Inside 6', reserved: [] },
      { id: 7, description: 'Outside 1', reserved: [] },
      { id: 8, description: 'Outside 2', reserved: [] },
      { id: 9, description: 'Outside 3', reserved: [] },
      { id: 10, description: 'Outside 4', reserved: [] },
      { id: 11, description: 'Outside 5', reserved: [] },
      { id: 12, description: 'Terrace 1', reserved: [] },
      { id: 13, description: 'Terrace 2', reserved: [] },
      { id: 14, description: 'Terrace 3', reserved: [] },
      { id: 15, description: 'Party table', reserved: [] }
    ];

    this.tableSubscription = this.tableTextSubject
      .pipe(
        filter((x) => !!x),
        tap(() => {
          this.tables = [];
          this.tablesLoading = true;
        }),
        debounceTime(100)
      )
      .subscribe((searchText: string) => {
        this.tables = this.allTables.filter(
          (x) =>
            x.description.toLowerCase().includes(searchText.toLowerCase()) &&
            this.form.dropdownId.value &&
            !x.reserved.some((res) => res === `${this.form.date.value} - ${this.form.dropdownId.value}`)
        );
        this.tablesLoading = false;
      });
  }
}
