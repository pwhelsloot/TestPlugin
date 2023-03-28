import { inject, TestBed } from '@angular/core/testing';
import { InstrumentationService } from '@core-module/services/logging/instrumentationService.service';
import { Control, ControlOptions, latLng } from 'leaflet';
import { Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { AmcsLeafletMapIconType } from '../../shared/amcs-leaflet-map/enums/amcs-leaflet-icon-type.enum';
import { AmcsMapProvider } from '../../shared/amcs-leaflet-map/enums/amcs-map-provider.enum';
import { AmcsLeafletMapGrouping } from '../../shared/amcs-leaflet-map/models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapService } from './amcs-leaflet-map.service';
import { ApplicationConfigurationStore } from './config/application-configuration.store';

describe('Service: AmcsLeafletMapService', () => {
  // let mapService: AmcsLeafletMapService;
  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy('Map service Observer');
  const mockApplicationInsightsService = new InstrumentationService();
  const mockConfigStore = new ApplicationConfigurationStore(mockApplicationInsightsService);

  beforeEach(() => {
    TestBed.configureTestingModule({
      // Provide the service that we want to Test
      providers: [AmcsLeafletMapService, { provide: ApplicationConfigurationStore, useValue: mockConfigStore }]
    });
  });

  afterEach(() => {
    destroy.next();
    observer.calls.reset();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('should set map provider', inject(
    [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
      //arrange
      mapService.selectedProvider$.pipe(takeUntil(destroy)).subscribe(observer);
      const expectedProvider = AmcsMapProvider.HereMapsTerrain;
      //act
      mapService.setMapProvider(expectedProvider);

      //assert
      expect(observer).toHaveBeenCalledWith(expectedProvider);

      //once because its a Replaysubject with a initial value)
      expect(observer).toHaveBeenCalledTimes(1);
    }));

  it('should set map legend', inject(
    [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
      //arrange
      mapService.legend$.pipe(takeUntil(destroy)).subscribe(observer);
      const expectedLegend = [AmcsLeafletMapIconType.GpsServiceLocation, AmcsLeafletMapIconType.GpsRouteVisitCompleted];
      //act
      mapService.setLegend(expectedLegend);

      //assert
      expect(observer).toHaveBeenCalledWith(expectedLegend);
      expect(observer).toHaveBeenCalledTimes(1);
    }));

  it('should set map state', inject(
    [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
      //arrange
      mapService.mapBounds$.pipe(takeUntil(destroy)).subscribe(observer);
      mapService.layerGroups$.pipe(takeUntil(destroy)).subscribe(observer);
      const grouping = new AmcsLeafletMapGrouping();
      grouping.id = 9999;
      const expectedGroupings = [grouping];
      const expectedBounds = latLng(100, 100).toBounds(50);

      //act
      mapService.setState(expectedGroupings, expectedBounds);

      //assert
      expect(observer).toHaveBeenCalledWith(expectedGroupings);
      expect(observer).toHaveBeenCalledWith(expectedBounds);

      //3 because of behaviour subject
      expect(observer).toHaveBeenCalledTimes(3);
    }));

  it('should clear map state', inject(
    [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
      //arrange
      mapService.layerGroups$.pipe(takeUntil(destroy)).subscribe(observer);

      //act
      mapService.clearState();

      //assert
      expect(observer).toHaveBeenCalledWith([]);
      expect(observer).toHaveBeenCalledTimes(1);
    }));

  it('should set map priority pane visibility', inject(
    [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
      //arrange
      mapService.showPriorityPane$.pipe(takeUntil(destroy)).subscribe(observer);
      const expected = false;

      //act
      mapService.setPriorityPaneVisible(expected);

      //assert
      expect(observer).toHaveBeenCalledWith(false);
      expect(observer).toHaveBeenCalledTimes(1);
    }));

  describe(('Controls'), () => {
    it('should add control', inject(
      [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
        //arrange
        mapService.controls$.pipe(takeUntil(destroy)).subscribe(observer);
        const expectedControl1 = new Control({ position: 'topright' } as ControlOptions);
        const expectedControl2 = new Control({ position: 'topright' } as ControlOptions);
        const controlId1 = 999;
        const controlId2 = 100;

        //act
        mapService.addMapControl(expectedControl1, controlId1);
        mapService.addMapControl(expectedControl2, controlId2);

        //assert
        expect(observer).toHaveBeenCalledWith([expectedControl1, expectedControl2]);

        //twice because its a BehaviourSubject(has an initial value)
        expect(observer).toHaveBeenCalledTimes(3);
      }));

    it('should not add duplicate controls', inject(
      [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
        //arrange
        mapService.controls$.pipe(takeUntil(destroy)).subscribe(observer);
        const expectedControl = new Control({ position: 'topright' } as ControlOptions);
        const duplicateControl = new Control({ position: 'topleft' } as ControlOptions);
        const controlId = 999;

        //act
        //different controls, same id = duplicate
        mapService.addMapControl(expectedControl, controlId);
        mapService.addMapControl(duplicateControl, controlId);

        //assert
        expect(observer).toHaveBeenCalledWith([expectedControl]);

        //twice because its a BehaviourSubject(has an initial value)
        expect(observer).toHaveBeenCalledTimes(2);
      }));

    it('should refresh controls', inject(
      [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
        //arange
        mapService.controls$.pipe(takeUntil(destroy)).subscribe(observer);
        const expectedControl1 = new Control({ position: 'topright' } as ControlOptions);
        const expectedControl2 = new Control({ position: 'topright' } as ControlOptions);
        const controlId1 = 999;
        const controlId2 = 100;

        //act
        mapService.addMapControl(expectedControl1, controlId1);
        mapService.addMapControl(expectedControl2, controlId2);
        observer.calls.reset();
        mapService.refreshMapControls();

        //assert
        expect(observer).toHaveBeenCalledWith([expectedControl1, expectedControl2]);
        expect(observer).toHaveBeenCalledTimes(1);
      }));
  });

  it('should trigger remove shape layers', inject(
    [AmcsLeafletMapService], (mapService: AmcsLeafletMapService) => {
      //arrange
      mapService.removeShapeLayersSubject.pipe(takeUntil(destroy)).subscribe(observer);

      //act
      mapService.removeShapeLayers();

      //assert
      expect(observer).toHaveBeenCalledWith(undefined);
      expect(observer).toHaveBeenCalledTimes(1);
    }));
});
