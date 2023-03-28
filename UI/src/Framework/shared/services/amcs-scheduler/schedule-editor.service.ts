import { EventEmitter, Injectable } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { BaseService } from '@core-module/services/base.service';
import { FormTileOptions } from '@shared-module/components/layouts/amcs-form-tile/form-tile-options.model';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { ScheduleEditorApi } from '@shared-module/models/amcs-scheduler/schedule-editor-api.model';
import { ScheduleForm } from '@shared-module/models/amcs-scheduler/schedule-form.model';
import { Schedule } from '@shared-module/models/amcs-scheduler/schedule.model';
import { merge } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SharedTranslationsService } from '../shared-translations.service';

@Injectable()
export class ScheduleEditorService extends BaseService {
  static providers = [ScheduleEditorService];
  form: ScheduleForm = null;
  options = new FormTileOptions();

  constructor(private readonly formBuilder: FormBuilder, private readonly translationsService: SharedTranslationsService) {
    super();
  }

  private onSave: EventEmitter<Schedule>;
  private onCancel: EventEmitter<void>;
  private onScheduleEditorReady: EventEmitter<ScheduleEditorApi>;

  init(
    dataModel: Schedule,
    onSave: EventEmitter<Schedule>,
    onCancel: EventEmitter<void>,
    onScheduleEditorReady: EventEmitter<ScheduleEditorApi>,
    hideParentTile: boolean
  ) {
    this.options.editorTitle = this.translationsService.getTranslation('scheduleEditor.title');
    this.options.formOptions.enableFormActions = !hideParentTile;
    this.onSave = onSave;
    this.onCancel = onCancel;
    this.onScheduleEditorReady = onScheduleEditorReady;
    this.form = AmcsFormBuilder.buildForm(this.formBuilder, dataModel ?? new Schedule(), ScheduleForm);
    this.setupFormListeners();
    this.createEditorApi();
  }

  save() {
    const dataModel = AmcsFormBuilder.parseForm(this.form, ScheduleForm);
    this.onSave.emit(dataModel);
  }

  cancel() {
    this.onCancel.emit();
  }

  private createEditorApi() {
    const scheduleEditorApi = new ScheduleEditorApi();
    scheduleEditorApi.form = this.form;
    this.onScheduleEditorReady.emit(scheduleEditorApi);
  }

  private setupFormListeners() {
    this.form.isRepeat.valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe(() => {
      ScheduleForm.setRepeatConditionalValidation(this.form);
    });
    merge(this.form.isExpire.valueChanges, this.form.startDate.valueChanges)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(() => {
        ScheduleForm.setExpireConditionalValidation(this.form);
      });
    merge(this.form.startTime.valueChanges, this.form.expireDate.valueChanges)
      .pipe(takeUntil(this.unsubscribe))
      .subscribe(() => {
        ScheduleForm.setExpiryTimeValidation(this.form);
      });
    this.form.scheduleType.valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe(() => {
      ScheduleForm.setDailyConditionalValidation(this.form);
      ScheduleForm.setWeeklyConditionalValidation(this.form);
      ScheduleForm.setMonthlyConditionalValidation(this.form);
    });
    this.form.monthDaysType.valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe(() => {
      ScheduleForm.setMonthlyConditionalValidation(this.form);
    });
  }
}
