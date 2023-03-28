import * as L from 'leaflet';

L.Edit.Circle = L.Edit.CircleMarker.extend({
    // Fix for https://github.com/Leaflet/Leaflet.draw/issues/945
    _createResizeMarker() {
        const center = this._shape.getLatLng(),
            resizemarkerPoint = this._getResizeMarkerPoint(center);

        this._resizeMarkers = [];
        this._resizeMarkers.push(this._createMarker(resizemarkerPoint, this.options.resizeIcon));
    },
    _getResizeMarkerPoint(latlng) {
        const delta = this._shape._radius * Math.cos(Math.PI / 4);
        const point = this._map.project(latlng);
        return this._map.unproject([point.x + delta, point.y - delta]);
    },

    _resize(latlng) {
        const moveLatLng = this._moveMarker.getLatLng();
        let radius;

        if (L.GeometryUtil.isVersion07x()) {
            radius = moveLatLng.distanceTo(latlng);
        } else {
            radius = this._map.distance(moveLatLng, latlng);
        }

        this._shape.setRadius(radius);

        this._map.fire(L.Draw.Event.EDITRESIZE, { layer: this._shape });
    },
    // Disable any circle middle marker from being able to move
    _createMoveMarker() {
        const center = this._shape.getLatLng();
        this._moveMarker = this._createMarker(center, this.options.moveIcon);
        this._moveMarker.options.draggable = false;
    }
});
