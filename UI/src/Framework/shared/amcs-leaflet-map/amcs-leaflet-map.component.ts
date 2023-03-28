import {
  AfterViewInit,
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnChanges,
  OnDestroy, Output,
  Renderer2,
  SimpleChanges,
  ViewChild,
  ViewEncapsulation
} from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { Subscription } from 'rxjs';
import { tap } from 'rxjs/operators';
import { AmcsLeafletMapHeightUnit } from './models/amcs-leaflet-map-height-unit';
import { AmcsLeafletCircleService } from './services/amcs-leaflet-circle.service';
import { AmcsLeafletMapEngineService } from './services/amcs-leaflet-map-engine.service';
import { AmcsLeafletMarkerService } from './services/amcs-leaflet-marker.service';
import { AmcsLeafletPolygonService } from './services/amcs-leaflet-polygon.service';
import { AmcsLeafletPolylineService } from './services/amcs-leaflet-polyline.service';

@Component({
  selector: 'app-amcs-leaflet-map',
  templateUrl: './amcs-leaflet-map.component.html',
  encapsulation: ViewEncapsulation.None,
  styleUrls: ['./amcs-leaflet-map.component.scss'],
  providers: [
    AmcsLeafletMapEngineService,
    AmcsLeafletPolygonService,
    AmcsLeafletPolylineService,
    AmcsLeafletMarkerService,
    AmcsLeafletCircleService,
  ],
})
export class AmcsLeafletMapComponent extends AutomationLocatorDirective implements OnChanges, OnDestroy, AfterViewInit {
  /**
   * Longitude the map should initially center on
   *
   * @type {number}
   * @memberof AmcsLeafletMapComponent
   */
  @Input() longitude: number;

  /**
   * Latitude the map should initially center on
   *
   * @type {number}
   * @memberof AmcsLeafletMapComponent
   */
  @Input() latitude: number;

  /**
   * Initial map zoom
   *
   * @memberof AmcsLeafletMapComponent
   */
  @Input() zoom = 20;

  /**
   * Actual height of the map being displayed
   *
   * @memberof AmcsLeafletMapComponent
   */
  @Input() mapHeight = 600;

  /**
   * Map height unit
   *
   * @type {AmcsLeafletMapHeightUnit}
   * @memberof AmcsLeafletMapComponent
   */
  @Input() mapHeightUnit: AmcsLeafletMapHeightUnit = 'px';

  /**
   * Whether to show 'expanded' features on the map
   *
   * @memberof AmcsLeafletMapComponent
   */
  @Input() expandedMode = false;

  @Input() customClass: string = null;

  /**
   * Enable Marker Cluster Groups
   *
   * @memberof AmcsLeafletMapComponent
   */
  @Input() enableMarkerCluster = false;

  /**
   * Reference to the map div
   *
   * @type {ElementRef}
   * @memberof AmcsLeafletMapComponent
   */
  @ViewChild('map') mapDiv: ElementRef;

  /**
   * Event that is emitted whenever a marker is being dragged
   *
   * @type {(EventEmitter<[number, number] | string>)}
   * @memberof AmcsLeafletMapComponent
   */
  @Output() readonly markerDragged: EventEmitter<[number, number] | string> = new EventEmitter<[number, number] | string>();

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, public mapEngine: AmcsLeafletMapEngineService) {
    super(elRef, renderer);
  }

  private subscription: Subscription;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes.enableMarkerCluster) {
      this.mapEngine.options.enableMarkerCluster = changes.enableMarkerCluster.currentValue;
    }
    if (changes.longitude) {
      this.mapEngine.options.longitude = changes.longitude.currentValue;
    }
    if (changes.latitude) {
      this.mapEngine.options.latitude = changes.latitude.currentValue;
    }
    if (changes.zoom) {
      this.mapEngine.options.zoom = changes.zoom.currentValue;
    }
    if (changes.mapHeightUnit) {
      this.mapEngine.options.mapHeightUnit = changes.mapHeightUnit.currentValue;
      this.setHeightOfMapViewport();
    }
    if (changes.mapHeight) {
      this.mapEngine.options.mapHeight = changes.mapHeight.currentValue;
      this.setHeightOfMapViewport();
    }
    if (changes.expandedMode) {
      this.mapEngine.options.expandedMode = changes.expandedMode.currentValue;
    }
  }

  ngAfterViewInit() {
    this.setHeightOfMapViewport();
    this.mapEngine.initialize(this.mapDiv.nativeElement);
    this.subscription = this.mapEngine.onMarkerDragged$.pipe(tap((event) => this.markerDragged.emit(event))).subscribe();
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  fitToBounds() {
    this.mapEngine.tryFitToBounds();
  }

  setHeightOfMapViewport() {
    if (isTruthy(this.mapDiv) && isTruthy(this.mapDiv.nativeElement)) {
      this.mapDiv.nativeElement.style.height = this.mapHeight + this.mapHeightUnit;
      this.mapEngine.invalidateSize();
    }
  }
}
