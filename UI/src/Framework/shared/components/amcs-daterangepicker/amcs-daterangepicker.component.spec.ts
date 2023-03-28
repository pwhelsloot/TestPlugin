import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsDaterangepickerComponent } from './amcs-daterangepicker.component';

describe('AmcsDaterangepickerComponent', () => {
  let fixture: ComponentFixture<AmcsDaterangepickerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsDaterangepickerComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsDaterangepickerComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
