import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { ComponentFilterHelper } from '@shared-module/components/amcs-component-filter/component-filter-helper.service';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { ComponentFilterTypeEnum } from '@shared-module/components/amcs-component-filter/component-filter-type.enum';
import { ComponentFilter } from '@shared-module/components/amcs-component-filter/component-filter.model';
import { IFilterableItem } from '@shared-module/models/ifilterable-item.interface';
import { Observable, ReplaySubject } from 'rxjs';

@Injectable()
export class ComponentFilterService extends BaseService {

  static providers = [ComponentFilterService];

  filterProperties$: Observable<ComponentFilterProperty[]>;
  filteredData$: Observable<IFilterableItem[]>;
  appliedFilters$: Observable<ComponentFilter[]>;
  appliedFilters: ComponentFilter[] = [];
  filterIdCounter = 0;
  isDeleteFilter = false;

  showEditor = false;

  constructor() {
    super();
    this.setupGridColumnsStream();
    this.setupFilteredDataStream();
    this.setupAppliedFiltersStream();
  }

  private originalData: IFilterableItem[];
  private filterPropertiesSubject = new ReplaySubject<ComponentFilterProperty[]>(1);
  private filteredDataSubject = new ReplaySubject<IFilterableItem[]>(1);
  private appliedFiltersSubject = new ReplaySubject<ComponentFilter[]>(1);

  initaliseData(data: IFilterableItem[]) {
    this.originalData = data;
    this.filterData();
  }

  openEditor() {
    this.showEditor = true;
  }

  closeEditor() {
    this.showEditor = false;
  }

  applyFilter(filter: ComponentFilter) {
    if (this.appliedFilters.find(f => f.propertyKey === filter.propertyKey && f.filterTypeId === filter.filterTypeId)) {
      this.appliedFilters = this.appliedFilters.filter(f => f.propertyKey !== filter.propertyKey && f.filterTypeId !== filter.filterTypeId);
    }

    this.filterIdCounter++;
    filter.filterId = this.filterIdCounter;
    this.appliedFilters.push(filter);
    this.appliedFiltersSubject.next(this.appliedFilters);
    this.filterData();
  }

  deleteFilter(filterId: number, event: any) {
    event.stopPropagation();
    this.isDeleteFilter = true;
    this.appliedFilters = this.appliedFilters.filter(f => f.filterId !== filterId);
    this.appliedFiltersSubject.next(this.appliedFilters);
    this.filterData();
  }

  removeAll() {
    this.appliedFilters = [];
    this.appliedFiltersSubject.next(this.appliedFilters);
    this.filterData();
  }

  updateFilterProperties(properties: ComponentFilterProperty[]) {
    this.filterPropertiesSubject.next(properties);
  }

  loadFilters(filters: ComponentFilter[]) {
    this.appliedFilters = filters;

    if (this.originalData) {
      this.filterData();
    }

    this.appliedFilters.forEach(filter => {
      if (filter.filterId > this.filterIdCounter) {
        this.filterIdCounter = filter.filterId;
      }
    });
  }

  getFilters() {
    return this.appliedFilters;
  }

  private setupGridColumnsStream() {
    this.filterProperties$ = this.filterPropertiesSubject.asObservable();
  }

  private setupFilteredDataStream() {
    this.filteredData$ = this.filteredDataSubject.asObservable();
  }

  private setupAppliedFiltersStream() {
    this.appliedFilters$ = this.appliedFiltersSubject.asObservable();
  }

  private filterData() {
    let filteredData: IFilterableItem[] = this.originalData;

    if (filteredData && filteredData.length) {
      this.appliedFilters.forEach(filter => {
        switch (filter.filterTypeId) {
          case ComponentFilterTypeEnum.equal:
            filteredData = ComponentFilterHelper.isEqual(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.notEqual:
            filteredData = ComponentFilterHelper.isNotEqual(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.startsWith:
            filteredData = ComponentFilterHelper.startsWith(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.endsWith:
            filteredData = ComponentFilterHelper.endsWith(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.contains:
            filteredData = ComponentFilterHelper.contains(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.isEmpty:
            filteredData = ComponentFilterHelper.isEmpty(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.isNotEmpty:
            filteredData = ComponentFilterHelper.isNotEmpty(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.greaterThan:
            filteredData = ComponentFilterHelper.isGreaterThan(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.greaterThanOrEqual:
            filteredData = ComponentFilterHelper.isGreaterThanOrEqual(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.lessThan:
            filteredData = ComponentFilterHelper.isLessThan(filteredData, filter);
            break;
          case ComponentFilterTypeEnum.lessThanOrEqual:
            filteredData = ComponentFilterHelper.isLessThanOrEqual(filteredData, filter);
            break;
        }
      });
    }
    this.filteredDataSubject.next(filteredData);
  }
}
