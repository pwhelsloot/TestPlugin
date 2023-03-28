import { NO_ERRORS_SCHEMA } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AmcsFormGroupComponent } from './amcs-form-group.component';

describe('AmcsFormGroupComponent', () => {
  let component: AmcsFormGroupComponent;
  let fixture: ComponentFixture<AmcsFormGroupComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      declarations: [AmcsFormGroupComponent],
    });
    fixture = TestBed.createComponent(AmcsFormGroupComponent);
    component = fixture.componentInstance;
  });

  it('has form-group class by default', () => {
    const className = getComponentClass(fixture);
    expect(className).toEqual('form-group');
  });

  describe('hasError', () => {
    it(`has default value`, () => {
      expect(component.hasError).toEqual(false);
    });

    it(`enables class when true`, () => {
      component.hasError = false;
      fixture.detectChanges();
      let exists = hasClass(fixture, 'has-error');
      expect(exists).toBeFalse();

      component.hasError = true;
      fixture.detectChanges();
      exists = hasClass(fixture, 'has-error');
      expect(exists).toBeTruthy();
    });
  });
  describe('hasSuccess', () => {
    it(`has default value`, () => {
      expect(component.hasSuccess).toEqual(false);
    });

    it(`enables class when true`, () => {
      component.hasSuccess = false;
      fixture.detectChanges();
      let exists = hasClass(fixture, 'has-success');
      expect(exists).toBeFalse();

      component.hasSuccess = true;
      fixture.detectChanges();
      exists = hasClass(fixture, 'has-success');
      expect(exists).toBeTruthy();
    });
  });

  describe('inline', () => {
    it(`has default value`, () => {
      expect(component.inline).toEqual(false);
    });

    it(`enables class when true`, () => {
      component.inline = false;
      fixture.detectChanges();
      let exists = hasClass(fixture, 'form-md-line-input');
      expect(exists).toBeFalse();

      component.inline = true;
      fixture.detectChanges();
      exists = hasClass(fixture, 'form-md-line-input');
      expect(exists).toBeTruthy();
    });
  });

  describe('hasActions', () => {
    it(`has default value`, () => {
      expect(component.hasActions).toEqual(false);
    });

    it(`enables class when true`, () => {
      component.hasActions = false;
      fixture.detectChanges();
      let exists = hasClass(fixture, 'actions');
      expect(exists).toBeFalse();

      component.hasActions = true;
      fixture.detectChanges();
      exists = hasClass(fixture, 'actions');
      expect(exists).toBeTruthy();
    });
  });
});

function getComponentClass(fixture: ComponentFixture<AmcsFormGroupComponent>) {
  return fixture.nativeElement.className;
}

function hasClass(fixture: ComponentFixture<AmcsFormGroupComponent>, search: string) {
  let className = getComponentClass(fixture);
  const exists = className.indexOf(search) > -1;
  return exists;
}
