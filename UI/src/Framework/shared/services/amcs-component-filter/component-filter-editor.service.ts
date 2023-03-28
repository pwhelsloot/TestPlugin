import { Injectable, Optional } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { ComponentFilterPropertyValueType } from '@shared-module/components/amcs-component-filter/component-filter-property-value-type.enum';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { ComponentFilterTypeEnum } from '@shared-module/components/amcs-component-filter/component-filter-type.enum';
import { ComponentFilterType } from '@shared-module/components/amcs-component-filter/component-filter-type.model';
import { ComponentFilter } from '@shared-module/components/amcs-component-filter/component-filter.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Observable, ReplaySubject, Subject } from 'rxjs';
import { takeUntil, withLatestFrom } from 'rxjs/operators';
import { ComponentFilterService } from './amcs-component-filter.service';
import { ComponentFilterApiOptionService } from './component-filter-api-option.service';

@Injectable()
export class ComponentFilterEditorService extends BaseService {
  componentFilterProperties$: Observable<ComponentFilterProperty[]>;
  componentFilterTypes$: Observable<ComponentFilterType[]>;

  constructor(
    private componentFilterService: ComponentFilterService,
    private translationsService: SharedTranslationsService,
    // We should avoid Optional if possible, here it's saving us passing state across multiple files
    @Optional() private componentFilterApiOptionService: ComponentFilterApiOptionService
  ) {
    super();
    this.setupGridFilterTypesStream();
    this.setupFilterPropertiesStream();
  }

  private componentFilterPropertiesSubject = new ReplaySubject<ComponentFilterProperty[]>(1);
  private componentFilterTypesSubject = new ReplaySubject<ComponentFilterType[]>(1);
  private componentFilterTypesRequestSubject = new Subject<{
    propertyType: number;
    textOptionsRestricted: boolean;
  }>();

  requestComponentFilterTypes(propertyType: number, textOptionsRestricted = false) {
    this.componentFilterTypesRequestSubject.next({ propertyType, textOptionsRestricted });
  }

  sendFilter(filter: ComponentFilter) {
    this.componentFilterService.applyFilter(filter);
  }

  closeEditor() {
    this.componentFilterService.closeEditor();
  }

  private setupFilterPropertiesStream() {
    this.componentFilterProperties$ = this.componentFilterPropertiesSubject.asObservable();

    this.componentFilterService.filterProperties$.pipe(takeUntil(this.unsubscribe)).subscribe((properties: ComponentFilterProperty[]) => {
      this.componentFilterPropertiesSubject.next(properties);
    });
  }

  private setupGridFilterTypesStream() {
    this.componentFilterTypes$ = this.componentFilterTypesSubject.asObservable();

    this.componentFilterTypesRequestSubject
      .pipe(withLatestFrom(this.translationsService.translations), takeUntil(this.unsubscribe))
      .subscribe((data) => {
        const propertyType: number = data[0].propertyType;
        const translations: string[] = data[1];
        const textOptionsRestricted: boolean = data[0].textOptionsRestricted;
        const filterTypes = this.buildComponentFilterTypes(propertyType, translations, textOptionsRestricted);
        this.componentFilterTypesSubject.next(filterTypes);
      });
  }

  private buildComponentFilterTypes(propertyType: number, translations: string[], textOptionsRestricted: boolean): ComponentFilterType[] {
    const filterTypes: ComponentFilterType[] = [];

    if (propertyType === ComponentFilterPropertyValueType.text) {
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.equal, translations['componentFilter.filterTypes.equal']));
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.notEqual, translations['componentFilter.filterTypes.notEqual']));
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.startsWith, translations['componentFilter.filterTypes.startsWith']));
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.endsWith, translations['componentFilter.filterTypes.endsWith']));
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.contains, translations['componentFilter.filterTypes.contains']));

      // Below filters not supported by api.  The textOptionsRestricted is used when the filter selection is used in a call to the API
      if (this.componentFilterApiOptionService === null && !textOptionsRestricted) {
        filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.isEmpty, translations['componentFilter.filterTypes.isEmpty']));
        filterTypes.push(
          new ComponentFilterType(ComponentFilterTypeEnum.isNotEmpty, translations['componentFilter.filterTypes.isNotEmpty'])
        );
      }
    } else if (propertyType === ComponentFilterPropertyValueType.number || propertyType === ComponentFilterPropertyValueType.date) {
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.lessThan, translations['componentFilter.filterTypes.lessThan']));
      filterTypes.push(
        new ComponentFilterType(ComponentFilterTypeEnum.greaterThan, translations['componentFilter.filterTypes.greaterThan'])
      );
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.equal, translations['componentFilter.filterTypes.equal']));
      filterTypes.push(
        new ComponentFilterType(ComponentFilterTypeEnum.lessThanOrEqual, translations['componentFilter.filterTypes.lessThanOrEqual'])
      );
      filterTypes.push(
        new ComponentFilterType(ComponentFilterTypeEnum.greaterThanOrEqual, translations['componentFilter.filterTypes.greaterThanOrEqual'])
      );
    } else if (propertyType === ComponentFilterPropertyValueType.boolean) {
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.equal, translations['componentFilter.filterTypes.equal']));
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.notEqual, translations['componentFilter.filterTypes.notEqual']));
    } else if (propertyType === ComponentFilterPropertyValueType.enum) {
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.equal, translations['componentFilter.filterTypes.equal']));
      filterTypes.push(new ComponentFilterType(ComponentFilterTypeEnum.notEqual, translations['componentFilter.filterTypes.notEqual']));
    }

    return filterTypes;
  }
}
