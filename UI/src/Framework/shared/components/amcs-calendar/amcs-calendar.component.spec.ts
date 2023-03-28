import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsCalendarComponent } from './amcs-calendar.component';

describe('AmcsCalendarComponent', () => {
  let fixture: ComponentFixture<AmcsCalendarComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsCalendarComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsCalendarComponent);
    fixture.detectChanges();
  });
  /*
    it('should create', () => {
      expect(component).toBeTruthy();
    });
    */
});
