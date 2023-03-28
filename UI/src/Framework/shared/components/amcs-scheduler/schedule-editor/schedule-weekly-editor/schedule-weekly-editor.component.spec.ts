import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleWeeklyEditorComponent } from './schedule-weekly-editor.component';

describe('ScheduleWeeklyEditorComponent', () => {
  let component: ScheduleWeeklyEditorComponent;
  let fixture: ComponentFixture<ScheduleWeeklyEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScheduleWeeklyEditorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleWeeklyEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
