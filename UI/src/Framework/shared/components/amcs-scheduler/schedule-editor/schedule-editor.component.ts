import { Component, EventEmitter, Input, OnDestroy, OnInit, Output } from '@angular/core';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { FormTileOptions } from '@shared-module/components/layouts/amcs-form-tile/form-tile-options.model';
import { ScheduleEditorApi } from '@shared-module/models/amcs-scheduler/schedule-editor-api.model';
import { ScheduleTypeEnum } from '@shared-module/models/amcs-scheduler/schedule-type.enum';
import { Schedule } from '@shared-module/models/amcs-scheduler/schedule.model';
import { ScheduleEditorService } from '@shared-module/services/amcs-scheduler/schedule-editor.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-schedule-editor',
  templateUrl: './schedule-editor.component.html',
  styleUrls: ['./schedule-editor.component.scss'],
  providers: [ScheduleEditorService.providers],
})
export class ScheduleEditorComponent implements OnInit, OnDestroy {
  ScheduleTypeEnum = ScheduleTypeEnum;
  dateConfig = new AmcsDatepickerConfig();
  @Input() schedule: Schedule;
  @Input() options: FormTileOptions;
  @Input() hideParentTile = false;

  @Output() onSave = new EventEmitter<Schedule>();
  @Output() onCancel = new EventEmitter<void>();
  @Output() onScheduleEditorReady = new EventEmitter<ScheduleEditorApi>();

  constructor(readonly service: ScheduleEditorService) { }

  private subscription: Subscription;

  ngOnInit(): void {
    if (this.options) {
      this.service.options = this.options;
    }
    this.service.init(this.schedule, this.onSave, this.onCancel, this.onScheduleEditorReady, this.hideParentTile);
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
