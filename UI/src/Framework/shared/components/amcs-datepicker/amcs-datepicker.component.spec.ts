import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsDatepickerComponent } from './amcs-datepicker.component';

describe('AmcsDatepickerComponent', () => {
  let fixture: ComponentFixture<AmcsDatepickerComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsDatepickerComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsDatepickerComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
