import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleMonthlyEditorComponent } from './schedule-monthly-editor.component';

describe('ScheduleMonthlyEditorComponent', () => {
  let component: ScheduleMonthlyEditorComponent;
  let fixture: ComponentFixture<ScheduleMonthlyEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScheduleMonthlyEditorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleMonthlyEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
