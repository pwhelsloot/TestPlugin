import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleAdvancedEditorComponent } from './schedule-advanced-editor.component';

describe('ScheduleAdvancedEditorComponent', () => {
  let component: ScheduleAdvancedEditorComponent;
  let fixture: ComponentFixture<ScheduleAdvancedEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScheduleAdvancedEditorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleAdvancedEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
