import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsTypeaheadComponent } from './amcs-typeahead.component';

describe('AmcsTypeaheadComponent', () => {
  let fixture: ComponentFixture<AmcsTypeaheadComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsTypeaheadComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsTypeaheadComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
