import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ScheduleDailyEditorComponent } from './schedule-daily-editor.component';

describe('ScheduleDailyEditorComponent', () => {
  let component: ScheduleDailyEditorComponent;
  let fixture: ComponentFixture<ScheduleDailyEditorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ScheduleDailyEditorComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ScheduleDailyEditorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
