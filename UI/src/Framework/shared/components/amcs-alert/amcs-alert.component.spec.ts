import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsAlertComponent } from './amcs-alert.component';

describe('AmcsAlertComponent', () => {
  let fixture: ComponentFixture<AmcsAlertComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsAlertComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsAlertComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
