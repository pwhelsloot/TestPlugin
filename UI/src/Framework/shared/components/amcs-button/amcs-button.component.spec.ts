import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsButtonComponent } from './amcs-button.component';

describe('AmcsButtonComponent', () => {
  let fixture: ComponentFixture<AmcsButtonComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AmcsButtonComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsButtonComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
