import { ComponentFixture, TestBed } from '@angular/core/testing';
import { NO_ERRORS_SCHEMA } from '@angular/core';
import { AmcsFormLabelComponent } from './amcs-form-label.component';

describe('AmcsFormLabelComponent', () => {
  let component: AmcsFormLabelComponent;
  let fixture: ComponentFixture<AmcsFormLabelComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      declarations: [AmcsFormLabelComponent]
    });
    fixture = TestBed.createComponent(AmcsFormLabelComponent);
    component = fixture.componentInstance;
  });

  it('can load instance', () => {
    expect(component).toBeTruthy();
  });
});
