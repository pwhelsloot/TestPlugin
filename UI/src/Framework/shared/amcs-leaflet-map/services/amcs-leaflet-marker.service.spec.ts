/* tslint:disable:no-unused-variable */
/* eslint-disable no-duplicate-imports */
import { TestBed } from '@angular/core/testing';
import { Layer, map, MarkerClusterGroup } from 'leaflet';
import { AmcsCustomIconOptions } from '../custom-icon/amcs-custom-icon-options';
import { AmcsLeafletDataType } from '../enums/amcs-leaflet-datatype.enum';
import { AmcsLeafletMapGrouping } from '../models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapMarker } from '../models/amcs-leaflet-map-marker.model';
import { AmcsLeafletMarkerService } from './amcs-leaflet-marker.service';

describe('Service: AmcsLeafletMarker', () => {
  let markerService: AmcsLeafletMarkerService;
  const leafletMap = generateMap();

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AmcsLeafletMarkerService],
    });
    markerService = TestBed.inject(AmcsLeafletMarkerService); // get reference to the Service we are testing
  });

  describe('layer click event', () => {
    it('should listen to click event', () => {
      //arrange
      markerService.initialize(leafletMap);
      const { group, mapMarker } = generateTestMarkerGroupGroup();
      const layer = new Layer();
      mapMarker.isClickable = true;
      let clickFunction = jasmine.createSpy();

      //act
      markerService.setIsClickable(mapMarker, group, layer, clickFunction);
      layer.fireEvent('click');

      //assert
      expect(clickFunction).toHaveBeenCalled();
    });

    it('should ignore click event', () => {
      //arrange
      markerService.initialize(leafletMap);
      const { group, mapMarker } = generateTestMarkerGroupGroup();
      const layer = new Layer();
      mapMarker.isClickable = false;
      const clickFunction = jasmine.createSpy();

      //act
      markerService.setIsClickable(mapMarker, group, layer, clickFunction);
      layer.fireEvent('click');

      //assert
      expect(clickFunction).not.toHaveBeenCalled();
    });
  });

  describe('layer drag event', () => {
    it('should listen to drag event', () => {
      //arrange
      markerService.initialize(leafletMap);
      const mapMarker = generateMarker();
      const layer = new Layer();
      mapMarker.isDraggable = true;
      const dragFunction = jasmine.createSpy();

      //act
      markerService.setDraggedFunction(mapMarker, layer, dragFunction);
      layer.fireEvent('dragend');

      //assert
      expect(dragFunction).toHaveBeenCalled();
    });

    it('should ignore drag event', () => {
      //arrange
      markerService.initialize(leafletMap);
      const mapMarker = generateMarker();
      const layer = new Layer();
      mapMarker.isDraggable = true;
      const dragFunction = jasmine.createSpy();

      //act
      markerService.setDraggedFunction(mapMarker, layer, dragFunction);
      layer.fireEvent('dragend');

      //assert
      expect(dragFunction).toHaveBeenCalled();
    });
  });

  describe('generate layers from group', () => {
    it('should generate without ClusterGroup', () => {
      //arrange
      markerService.initialize(leafletMap);
      const { group, mapMarker } = generateTestMarkerGroupGroup();
      spyOn(markerService, 'setDraggedFunction');
      spyOn(markerService, 'setIsClickable');

      //act
      markerService.generateLayersFromGroup(group);
      const layer = markerService.getLayers().find((l) => l['_leaflet_id'] === mapMarker.leafletLayerId);

      //assert
      expect(layer).toBeDefined();
      expect(markerService.setDraggedFunction).toHaveBeenCalled();
      expect(markerService.setIsClickable).toHaveBeenCalled();
    });

    it('should generate with ClusterGroup', () => {
      //arrange
      const clusterGroup = new MarkerClusterGroup({
        disableClusteringAtZoom: 18,
      });
      markerService.initialize(leafletMap, clusterGroup);
      const { group, mapMarker } = generateTestMarkerGroupGroup();
      spyOn(markerService, 'setDraggedFunction');
      spyOn(markerService, 'setIsClickable');

      //act
      markerService.generateLayersFromGroup(group);
      const layerOnMap = markerService.getLayers().find((l) => l['_leaflet_id'] === mapMarker.leafletLayerId);
      const layerInGroup = markerService.getClusterGroup().getLayer(mapMarker.leafletLayerId);

      //assert
      expect(layerOnMap).toBeDefined();
      expect(layerInGroup).toBeDefined();
      expect(markerService.setDraggedFunction).toHaveBeenCalled();
      expect(markerService.setIsClickable).toHaveBeenCalled();
    });

    it('should generate layer with click action', () => {
      //arrange
      markerService.initialize(leafletMap);
      const { group, mapMarker } = generateTestMarkerGroupGroup();
      spyOn(markerService, 'setDraggedFunction');
      spyOn(markerService, 'setIsClickable');

      //act
      markerService.generateLayersFromGroup(group);
      const layer = markerService.getLayers().find((l) => l['_leaflet_id'] === mapMarker.leafletLayerId);

      //assert
      expect(layer).toBeDefined();
      expect(markerService.setDraggedFunction).toHaveBeenCalled();
      expect(markerService.setIsClickable).toHaveBeenCalled();
    });
  });
});

function generateTestMarkerGroupGroup() {
  const group = new AmcsLeafletMapGrouping();
  group.id = 1111;
  const mapMarker = generateMarker();
  group.markers.push(mapMarker);
  return { group, mapMarker };
}

function generateMarker() {
  const mapMarker = new AmcsLeafletMapMarker(AmcsLeafletDataType.Default, [100, 100], 'Test', new AmcsCustomIconOptions());
  mapMarker.id = 222;
  return mapMarker;
}

function generateMap() {
  const container = document.createElement('div');
  document.body.appendChild(container);
  const leafletMap = map(container, {
    zoom: 12,
    scrollWheelZoom: false,
  });
  return leafletMap;
}
