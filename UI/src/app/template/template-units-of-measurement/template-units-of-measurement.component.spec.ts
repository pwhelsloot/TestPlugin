import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { AiLoggingService } from '@core-module/services/logging/ai-logging.service';
import { MockProvider } from 'ng-mocks';
import { TemplateUnitsOfMeasurementComponent } from './template-units-of-measurement.component';

describe('TemplateUnitsOfMeasurementComponent', () => {
  let component: TemplateUnitsOfMeasurementComponent;
  let fixture: ComponentFixture<TemplateUnitsOfMeasurementComponent>;
  AiLoggingService.aiLoggingServiceReference = MockProvider(AiLoggingService).useFactory();

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [TemplateUnitsOfMeasurementComponent],
    }).compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TemplateUnitsOfMeasurementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
