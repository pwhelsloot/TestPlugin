import { ElementRef, NO_ERRORS_SCHEMA, Renderer2 } from '@angular/core';
import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AmcsGridActionColumnHeaderComponent } from '@shared-module/components/amcs-grid-action-column/amcs-grid-action-column-header/amcs-grid-action-column-header.component';

describe('AmcsGridActionColumnHeaderComponent', () => {
  let component: AmcsGridActionColumnHeaderComponent;
  let fixture: ComponentFixture<AmcsGridActionColumnHeaderComponent>;

  beforeEach(async () => {
    const elementRefStub = () => ({});
    const renderer2Stub = () => ({
          setAttribute: (el: any, name: string, value: string, namespace?: string | null) => void {},
          removeAttribute: (el: any, name: string, namespace?: string | null) => void {}
    });
    TestBed.configureTestingModule({
      schemas: [NO_ERRORS_SCHEMA],
      declarations: [AmcsGridActionColumnHeaderComponent],
      providers: [
        { provide: ElementRef, useFactory: elementRefStub },
        { provide: Renderer2, useFactory: renderer2Stub }
      ],
    });
    TestBed.overrideComponent(AmcsGridActionColumnHeaderComponent, {
      set: {
        providers: []
      }
    });
    TestBed.compileComponents();
    fixture = TestBed.createComponent(AmcsGridActionColumnHeaderComponent);
    component = fixture.componentInstance;
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
