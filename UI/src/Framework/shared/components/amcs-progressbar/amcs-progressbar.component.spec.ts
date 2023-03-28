import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsProgressbarComponent } from './amcs-progressbar.component';

describe('AmcsProgressbarComponent', () => {
  let fixture: ComponentFixture<AmcsProgressbarComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AmcsProgressbarComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsProgressbarComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
