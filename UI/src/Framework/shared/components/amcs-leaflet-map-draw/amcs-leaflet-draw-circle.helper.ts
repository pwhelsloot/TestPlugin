import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsNotificationService } from '@core-module/services/amcs-notification.service';
import { AmcsLeafletDrawMapCircle } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-draw-map-circle';
import { AmcsLeafletMapGrouping } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-grouping.model';
import { Layer } from 'leaflet';
import { AmcsLeafletMapDrawService } from './amcs-leaflet-map-draw.service';

export class AmcsLeafletDrawCircleHelper {
    constructor(
        public layers: AmcsLeafletMapGrouping[],
        private notificationService: AmcsNotificationService,
        private drawIntersectErrorText: string,
        private amcsLeafletMapDrawService: AmcsLeafletMapDrawService
    ) {}

    handleEditCircleDrawn(layer: Layer) {
        const newCircle: AmcsLeafletDrawMapCircle = this.validateCircleAgainstOverlaps(layer as L.Circle);
        if (isTruthy(newCircle)) {
            this.amcsLeafletMapDrawService.editShapeDrawn.next(newCircle);
        }
    }

    private validateCircleAgainstOverlaps(layer: L.Circle): AmcsLeafletDrawMapCircle {
        const newCircle: AmcsLeafletDrawMapCircle = AmcsLeafletDrawMapCircle.fromLayer(layer);
        let isValid = true;
        []
            .concat(...this.layers.map((x) => x.circles))
            .filter((l) => l.id !== layer['id'])
            .forEach((circle: AmcsLeafletDrawMapCircle) => {
                if (newCircle.overlaps(circle)) {
                    isValid = false;
                    this.notificationService.showLongNotification(this.drawIntersectErrorText);
                    return;
                }
            });
        return isValid ? newCircle : null;
    }
}
