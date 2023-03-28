import { Layer } from 'leaflet';

export class OperationalAreaRequest {
    layer: Layer;
    operationalAreaId?: number;
    exclusionOperationalAreaId?: number;
    businessTypeIds?: number[];
    companyOutletIds?: number[];
    isServiceExclusion: boolean;
    parentId?: number;
    exclusionNonOverlapError?: string;
    operationalAreaNonOverlapError?: string;

    constructor() {
        this.isServiceExclusion = false;
    }
}
