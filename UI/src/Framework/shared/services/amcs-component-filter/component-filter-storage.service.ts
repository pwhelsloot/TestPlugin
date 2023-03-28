import { Injectable } from '@angular/core';
import { BaseService } from '../../../core/services/base.service';
import { ComponentFilter } from '../../components/amcs-component-filter/component-filter.model';

@Injectable()
export class ComponentFilterStorageService extends BaseService {

    private storedFilters: ComponentFilter[];

    getFilters() {
        return this.storedFilters;
    }

    storeFilters(filters: ComponentFilter[]) {
        this.storedFilters = filters;
    }
}
