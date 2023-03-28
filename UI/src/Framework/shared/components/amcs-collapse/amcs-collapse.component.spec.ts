import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsCollapseComponent } from './amcs-collapse.component';

describe('AmcsCollapseComponent', () => {
  let fixture: ComponentFixture<AmcsCollapseComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ AmcsCollapseComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsCollapseComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
