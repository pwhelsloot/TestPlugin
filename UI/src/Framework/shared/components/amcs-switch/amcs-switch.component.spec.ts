import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsSwitchComponent } from './amcs-switch.component';

describe('AmcsSwitchComponent', () => {
  let fixture: ComponentFixture<AmcsSwitchComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AmcsSwitchComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsSwitchComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
