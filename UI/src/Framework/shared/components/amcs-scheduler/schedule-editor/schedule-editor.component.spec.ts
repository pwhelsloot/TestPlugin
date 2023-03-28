import { EventEmitter, NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ReactiveFormsModule } from '@angular/forms';
import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { ScheduleEditorApi } from '@shared-module/models/amcs-scheduler/schedule-editor-api.model';
import { Schedule } from '@shared-module/models/amcs-scheduler/schedule.model';
import { ScheduleEditorService } from '@shared-module/services/amcs-scheduler/schedule-editor.service';
import { MockProvider } from 'ng-mocks';
import { ScheduleEditorComponent } from './schedule-editor.component';

describe('ScheduleEditorComponent', () => {
  const defaultSchedule = new Schedule();
  const onSave = new EventEmitter<Schedule>();
  const onCancel = new EventEmitter<void>();
  const onScheduleEditorReady = new EventEmitter<ScheduleEditorApi>();
  let service: ScheduleEditorService;
  let component: ScheduleEditorComponent;
  let fixture: ComponentFixture<ScheduleEditorComponent>;

  beforeEach(async () => {
    TestBed.configureTestingModule({
      declarations: [ScheduleEditorComponent],
      imports: [ReactiveFormsModule],
      providers: [MockProvider(ScheduleEditorService)],
      schemas: [NO_ERRORS_SCHEMA],
    });

    TestBed.overrideComponent(ScheduleEditorComponent, {
      set: {
        providers: [],
      },
    });
    TestBed.compileComponents();
    service = TestBed.inject(ScheduleEditorService);
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleEditorComponent);
    component = fixture.componentInstance;
    defaultSchedule.expire = AmcsDate.create();
    component.schedule = defaultSchedule;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should create', () => {
    // arrange
    spyOn(service, 'init');

    // act
    component.ngOnInit();

    // asset
    expect(service.init).toHaveBeenCalledOnceWith(defaultSchedule, onSave, onCancel, onScheduleEditorReady, false);
  });
});
