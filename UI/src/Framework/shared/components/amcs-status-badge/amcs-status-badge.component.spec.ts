import { ComponentFixture, TestBed, waitForAsync } from '@angular/core/testing';
import { AmcsStatusBadgeComponent } from './amcs-status-badge.component';

describe('AmcsStatusBadgeComponent', () => {
  let component: AmcsStatusBadgeComponent;
  let fixture: ComponentFixture<AmcsStatusBadgeComponent>;

  beforeEach(
    waitForAsync(() => {
      TestBed.configureTestingModule({
        declarations: [AmcsStatusBadgeComponent]
      }).compileComponents();
    })
  );

  beforeEach(() => {
    fixture = TestBed.createComponent(AmcsStatusBadgeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should set label to progress', () => {
    component.progress = '10%';

    component.ngOnInit();

    expect(component.label).toEqual('10%');
  });

  it('should not set label to progress', () => {
    component.label = 'test';
    component.progress = '10%';

    component.ngOnInit();

    expect(component.label).toEqual('test');
  });
});
