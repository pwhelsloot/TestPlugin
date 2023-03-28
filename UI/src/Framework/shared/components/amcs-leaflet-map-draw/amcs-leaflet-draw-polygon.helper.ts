import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AmcsNotificationService } from '@core-module/services/amcs-notification.service';
import { AmcsLeafletMapGrouping } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-grouping.model';
import { AmcsLeafletMapPolygon } from '@shared-module/amcs-leaflet-map/models/amcs-leaflet-map-polygon.model';
import { OperationalAreaRequest } from '@shared-module/amcs-leaflet-map/models/operational-area-request.model';
import { BusinessTypeLookup } from '../../../core/models/lookups/business-type-lookup.model';
import { AmcsLeafletMapDrawService } from './amcs-leaflet-map-draw.service';

export class AmcsLeafletDrawPolygonHelper {
    constructor(
        public layers: AmcsLeafletMapGrouping[],
        private notificationService: AmcsNotificationService,
        private drawIntersectErrorText: string,
        private amcsLeafletMapDrawService: AmcsLeafletMapDrawService
    ) { }

    handlePolygonDrawn(request: OperationalAreaRequest, businessTypes?: BusinessTypeLookup[]) {
        const newPolygon: AmcsLeafletMapPolygon = this.validateOverlaps(request, businessTypes);
        if (isTruthy(newPolygon)) {
            this.amcsLeafletMapDrawService.newPolygonDrawn.next(newPolygon);
        }
    }

    handleEditPolygonDrawn(request: OperationalAreaRequest, businessTypes?: BusinessTypeLookup[]) {
        const newPolygon: AmcsLeafletMapPolygon = this.validateOverlaps(request, businessTypes);
        if (isTruthy(newPolygon)) {
            this.amcsLeafletMapDrawService.editShapeDrawn.next(newPolygon);
        }
    }

    private validateOverlaps(
        request: OperationalAreaRequest,
        businessTypes?: BusinessTypeLookup[]
    ): AmcsLeafletMapPolygon {
        if (request.isServiceExclusion) {
            return this.validateExclusionPolygonAgainstOverlaps(request);
        } else {
            return this.validatePolygonAgainstOverlaps(request, businessTypes);
        }
    }

    private validateExclusionPolygonAgainstOverlaps(
        request: OperationalAreaRequest
    ): AmcsLeafletMapPolygon {
        const layer = request.layer as L.Polygon;
        const newPolygon: AmcsLeafletMapPolygon = AmcsLeafletMapPolygon.fromLayer(layer);
        let isValid = false;
        let nonOverlapTitle: string;
        []
            .concat(...this.layers.map((x) => x.polygons))
            .filter((l) => l.id !== layer['id'])
            .forEach((polygon: AmcsLeafletMapPolygon) => {
                if (!polygon.op.isServiceExclusion && polygon.op.operationalAreaId === request.operationalAreaId) {
                    if (newPolygon.overlaps(polygon)) {
                        isValid = true;
                    } else {
                        nonOverlapTitle = polygon.title;
                    }
                }
            });
        if (!isValid) {
            this.notificationService.showLongNotification(request.exclusionNonOverlapError.replace('{0}', nonOverlapTitle));
        }
        return isValid ? newPolygon : null;
    }

    private validatePolygonAgainstOverlaps(
        request: OperationalAreaRequest,
        businessTypes?: BusinessTypeLookup[]
    ): AmcsLeafletMapPolygon {
        const layer = request.layer as L.Polygon;
        const newPolygon: AmcsLeafletMapPolygon = AmcsLeafletMapPolygon.fromLayer(layer);
        let isValid = true;
        let nonOverlapTitle: string[] = [];
        const overlappingBusinessTypes: BusinessTypeLookup[] = [];
        []
            .concat(...this.layers.map((x) => x.polygons))
            .filter((l) => l.id !== layer['id'])
            .forEach((polygon: AmcsLeafletMapPolygon) => {
                if (newPolygon.overlaps(polygon) && !polygon.op?.isServiceExclusion) {
                    if (isTruthy(request.businessTypeIds) && request.businessTypeIds.length > 0) {
                        const lineBusiness = polygon.op.businessTypeIds.filter(function(item) {
                            return request.businessTypeIds.indexOf(item) > -1;
                        });

                        if (lineBusiness?.length > 0) {
                            const companyOutlets = polygon.op.companyOutletIds.filter(function(item) {
                                return request.companyOutletIds.indexOf(item) > -1;
                            });

                            if (companyOutlets?.length > 0) {
                                lineBusiness.forEach((element) => {
                                    if (!isTruthy(overlappingBusinessTypes.find((x) => x.id === element))) {
                                        overlappingBusinessTypes.push(businessTypes.find((t) => t.id === element));
                                    }
                                });
                            }
                        }
                    } else {
                        isValid = false;
                        this.notificationService.showLongNotification(this.drawIntersectErrorText);
                        return;
                    }
                } else if (polygon.op && polygon.op.isServiceExclusion && polygon.op.operationalAreaId === request.operationalAreaId && !newPolygon.overlaps(polygon)) {
                    isValid = false;
                    nonOverlapTitle.push(polygon.title);
                }
            });
        if (overlappingBusinessTypes.length > 0) {
            isValid = false;
            const businessTypeString = overlappingBusinessTypes.map((x) => x.description).join(', ');
            this.notificationService.showLongNotification(this.drawIntersectErrorText.replace('{0}', businessTypeString));
        } else if (nonOverlapTitle.length) {
            const titles = nonOverlapTitle.map((x) => x).join(', ');
            this.notificationService.showLongNotification(request.operationalAreaNonOverlapError.replace('{0}', titles));
        }
        return isValid ? newPolygon : null;
    }
}
