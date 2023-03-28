import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsLeafletMapShapeEnum } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-shape.enum';
import * as L from 'leaflet';
import 'leaflet-draw';
import { AmcsLeafletMapBaseShape } from '../models/amcs-leaflet-map-base-shape.model';
import { OperationalAreaRequest } from '../models/operational-area-request.model';
import './amcs-map-draw.extensions';

export class AmcsMapDrawControl extends L.Control.Draw {
  currentlyEditableShape: number;

  constructor(
    position: L.ControlPosition,
    drawableShapes: AmcsLeafletMapShapeEnum[],
    translations: string[],
    public editableShapes: L.FeatureGroup
  ) {
    super({
      position,
      draw: AmcsMapDrawControl.createDrawOptions(translations, drawableShapes),
      edit: AmcsMapDrawControl.createEditOptions(editableShapes),
    });
    this.setDrawTranslations(translations);
  }
  private editHandler: any;
  private editShapesToolbar: L.EditToolbar;
  private map: L.Map;
  private cancelShapeId: number;

  onAdd(map: L.Map): HTMLElement {
    this.map = map;
    return super.onAdd(map);
  }

  /**
   * Reset editing of map
   *
   * @memberof AmcsMapDrawControl
   */
  resetEditMode() {
    this.map.doubleClickZoom.enable();
    if (this.editHandler) {
      this.editHandler.disable();
    }
    this.editableShapes.clearLayers();
  }

  updateShape(shape: AmcsLeafletMapBaseShape) {
    this.map.eachLayer((l) => {
      if (shape.id === l['id']) {
        this.map.removeLayer(l);
        this.map.addLayer(shape.toLayer());
      }
    });
  }

  addShape(shape: L.Layer) {
    this.map.addLayer(shape);
  }

  removeShape(shapeId: number) {
    this.map.eachLayer((l) => {
      if (shapeId === l['id']) {
        this.map.removeLayer(l);
      }
    });
  }

  revertEditLayer() {
    if (this.editHandler) {
      this.editHandler.revertLayers();
    }
  }

  cancelEditing() {
    this.revertEditLayer();
    this.resetEditMode();
    this.clearExclusion();
    this.currentlyEditableShape = undefined;
  }

  toggleEditShape(shapeId: number) {
    this.cancelEditing(); // revert the currently editing layer
    if (this.currentlyEditableShape !== shapeId) {
      this.map.doubleClickZoom.disable();
      this.editShapesToolbar = this['_toolbars']['edit'];
      this.editHandler = this.editShapesToolbar.getModeHandlers(this.map as L.DrawMap)[0].handler;
      // disable Draw control on editing
      const disableDrawActiveMode = this['_toolbars']['draw']._activeMode;
      if (isTruthy(disableDrawActiveMode)) {
        disableDrawActiveMode.handler.disable();
      }
      this.findAndEnableShapeEdit(shapeId);
    }
  }

  private findAndEnableShapeEdit(shapeId: number) {
    this.map.eachLayer((l) => {
      const op: OperationalAreaRequest = l['data'];
      if (shapeId === l['id']) {
        this.currentlyEditableShape = shapeId;
        this.editableShapes.addLayer(l);
        this.editHandler.enable();
        if (op?.isServiceExclusion) {
          this.cancelShapeId = shapeId;
          this.exclusionLayer(l, true);
        }
        this.map.fitBounds(new L.FeatureGroup([l]).getBounds());
      } else if (op?.isServiceExclusion && shapeId === op?.parentId) {
        this.cancelShapeId = shapeId;
        this.exclusionLayer(l, true);
      }
    });
  }

  private clearExclusion() {
    if (this.cancelShapeId) {
      this.map.eachLayer((l) => {
        const op: OperationalAreaRequest = l['data'];
        if (op?.isServiceExclusion && (this.cancelShapeId === op?.parentId || this.cancelShapeId === l['id'])) {
          this.exclusionLayer(l, false);
        }
      });
    }
    this.cancelShapeId = null;
  }

  private exclusionLayer(l: L.Layer, show: boolean) {
    this.map.removeLayer(l);
    (l as L.Polygon).options.className = show ? 'amcs-draw-polygon-exclusion' : 'amcs-draw-polygon-exclusion amcs-exclusion-polygon-hide';
    if (show && l['title']) {
      (l as L.Polygon).bindTooltip(l['title'], { permanent: true, className: 'amcs-draw-polygon-exclusion' });
    } else {
      (l as L.Polygon).bindTooltip(null, { permanent: false, opacity: 0 });
    }
    this.map.addLayer(l);
  }

  private static createEditOptions(editableShapes: L.FeatureGroup): L.Control.EditOptions {
    const options: L.Control.EditOptions = {
      featureGroup: editableShapes,
      remove: false,
      edit: {
        selectedPathOptions: {
          dashArray: '5, 30',
          fill: true,
          fillOpacity: 0.5,
        },
      },
    };
    return options;
  }

  private static createDrawOptions(translations: string[], drawableShapes: AmcsLeafletMapShapeEnum[]): L.Control.DrawOptions {
    const options: L.Control.DrawOptions = {
      marker: false,
      polyline: false,
      circle: false,
      circlemarker: false,
      rectangle: false,
      polygon: false,
    };
    drawableShapes.forEach((shape: AmcsLeafletMapShapeEnum) => {
      switch (shape) {
        case AmcsLeafletMapShapeEnum.circle:
          options.circle = {
            shapeOptions: {
              className: 'amcs-draw-circle',
            },
          };
          break;

        case AmcsLeafletMapShapeEnum.polygon:
          options.polygon = {
            allowIntersection: false,
            drawError: {
              message: translations['map.drawmap.polygon.intersecterror'],
            },
            shapeOptions: {
              className: 'amcs-draw-polygon',
            },
          };
          break;

        case AmcsLeafletMapShapeEnum.polyline:
          options.polyline = {
            allowIntersection: false,
            drawError: {
              message: translations['map.drawmap.polyline.intersecterror'],
            },
            shapeOptions: {
              className: 'amcs-draw-polyline',
            },
          };
          break;
      }
    });
    return options;
  }

  private setDrawTranslations(translations: string[]) {
    L.drawLocal.draw.handlers.polygon.tooltip.cont = translations['map.drawmap.polygon.nextpoint'];
    L.drawLocal.draw.handlers.polygon.tooltip.start = translations['map.drawmap.polygon.start'];
    L.drawLocal.draw.handlers.polygon.tooltip.end = translations['map.drawmap.polygon.continue'];
    L.drawLocal.draw.toolbar.buttons.polygon = translations['map.drawmap.polygon.createArea'];
    L.drawLocal.draw.toolbar.finish.text = translations['map.drawmap.toolbar.save'];
    L.drawLocal.draw.toolbar.finish.title = translations['map.drawmap.toolbar.tooltips.save'];
    L.drawLocal.draw.toolbar.undo.text = translations['map.drawmap.toolbar.undo'];
    L.drawLocal.draw.toolbar.undo.title = translations['map.drawmap.toolbar.tooltips.undo'];
    L.drawLocal.draw.toolbar.actions.text = translations['map.drawmap.toolbar.cancel'];
    L.drawLocal.draw.toolbar.actions.title = translations['map.drawmap.toolbar.tooltips.cancel'];
  }
}
