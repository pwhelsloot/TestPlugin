import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';

import { AmcsDropdownComponent } from './amcs-dropdown.component';

describe('AmcsDropdownComponent', () => {
  let fixture: ComponentFixture<AmcsDropdownComponent>;

  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [AmcsDropdownComponent]
    })
      .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsDropdownComponent);
    fixture.detectChanges();
  });
  /*
  it('should create', () => {
    expect(component).toBeTruthy();
  });
  */
});
