import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2, ViewChild } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ChartOptions } from 'chart.js';
import { BaseChartDirective } from 'ng2-charts';

@Component({
  selector: 'app-amcs-chart',
  templateUrl: './amcs-chart.component.html',
  styleUrls: ['./amcs-chart.component.scss']
})
export class AmcsChartComponent extends AutomationLocatorDirective implements OnInit, AfterViewInit {
  @Input() width: number;
  @Input() height: number;
  @Input() chartType: string;
  @Input() data: any[];
  @Input() datasets: any[];
  @Input() labels: string[];
  @Input() options: ChartOptions;
  @Input() colors: any[] = [
    {
      backgroundColor: [
        '#1488CA',
        '#DE4561',
        '#61C250',
        '#404545',
        '#FFA02F',
        '#FDC41F',
        '#004457',
        '#4E8532',
        '#428BCA',
        '#031E2F',
        '#FFF',
        '#000',
        '#60BD68',
        '#B2912F',
        '#B276B2',
        '#FAA43A',
        '#5DA5DA',
        '#F15854',
        '#DECF3F',
        '#F17CB0'
      ]
    }
  ];
  @Input() legend: boolean;
  @Input() useHtmlLegend = false;

  @Output() chartClick: EventEmitter<any> = new EventEmitter();
  @Output() chartHover: EventEmitter<any> = new EventEmitter();

  @ViewChild('baseChart') baseChart: BaseChartDirective;

  legendItems: any;
  widthString: string;
  heightString: string;

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, private cdRef: ChangeDetectorRef) {
    super(elRef, renderer);
  }

  ngOnInit() {
    if (isTruthy(this.height)) {
      this.heightString = this.height.toString() + 'px';
    }
    if (isTruthy(this.width)) {
      this.widthString = this.width.toString() + 'px';
    }
  }

  ngAfterViewInit() {
    if (isTruthy(this.options) && isTruthy(this.options.legendCallback) && this.useHtmlLegend) {
      this.legendItems = this.baseChart.chart.generateLegend();
      this.cdRef.detectChanges();
    }
  }

  onChartClick(e: any): void {
    this.chartClick.emit(e);
  }

  onChartHover(e: any): void {
    this.chartHover.emit(e);
  }
}
