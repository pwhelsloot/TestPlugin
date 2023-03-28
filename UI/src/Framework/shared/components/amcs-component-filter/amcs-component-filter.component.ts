import { Component, ElementRef, EventEmitter, Input, OnChanges, OnDestroy, OnInit, Output, Renderer2, SimpleChanges, TemplateRef, ViewChild } from '@angular/core';
import { FilterOperation } from '@core-module/models/api/filters/filter-operation.enum';
import { IFilter } from '@core-module/models/api/filters/iFilter';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { IFilterableItem } from '@shared-module/models/ifilterable-item.interface';
import { ComponentFilterService } from '@shared-module/services/amcs-component-filter/amcs-component-filter.service';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { PopoverDirective } from 'ngx-bootstrap/popover';
import { Subject, Subscription } from 'rxjs';
import { withLatestFrom } from 'rxjs/operators';
import { isTruthy } from '../../../core/helpers/is-truthy.function';
import { ComponentFilterStorageService } from '../../services/amcs-component-filter/component-filter-storage.service';
import { ComponentFilterProperty } from './component-filter-property.model';
import { ComponentFilterTypeEnum } from './component-filter-type.enum';
import { ComponentFilter } from './component-filter.model';

@Component({
  selector: 'app-amcs-component-filter',
  templateUrl: './amcs-component-filter.component.html',
  styleUrls: ['./amcs-component-filter.component.scss'],
  providers: [ComponentFilterService.providers]
})
export class AmcsComponentFilterComponent extends AutomationLocatorDirective implements OnInit, OnChanges, OnDestroy {

  @Input('data') data: IFilterableItem[];
  @Input('properties') properties: ComponentFilterProperty[];
  @Input() extrasTemplate: TemplateRef<any> = null;
  @Input('isOverlay') isOverlay: boolean;
  @Input() showCustomiser = false;
  @Input() accordianTitle: string;
  @Input() enableDefaultFilter = false;
  @Input() defaultFilterRequest$: Subject<{ filter: ComponentFilter[] }>;
  @Input() refreshRequest$: Subject<void>;
  /**
       * Is used to filter the data in server instead of client. When the flag set to 'true' don't pass the data as an input.
       * @example [isApiFilter]='true' then dont pass [(data)]
  */
  @Input() isApiFilter = false;
  @Input() popoverPlacement = 'bottom';
  @Input() displayInLine = false;
  @Output() dataChange: EventEmitter<IFilterableItem[]> = new EventEmitter<IFilterableItem[]>();
  @Output() filterChanged: EventEmitter<ComponentFilter[]> = new EventEmitter<ComponentFilter[]>();
  // Emit ApiRequest filter
  @Output() apiFilterChange: EventEmitter<IFilter[]> = new EventEmitter<IFilter[]>();

  @ViewChild('pop') popoverDirective: PopoverDirective;
  showFilter = false;
  emittingData = false;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public componentFilterService: ComponentFilterService,
    public componentFilterStorageService: ComponentFilterStorageService,
    private translationsService: SharedTranslationsService) {
    super(elRef, renderer);
  }

  private subscriptions = new Array<Subscription>();

  ngOnInit() {
    this.subscriptions.push(this.componentFilterService.filteredData$
      .subscribe(filteredData => {
        this.data = filteredData;
        this.emittingData = true;
        this.dataChange.emit(this.data);
      }));

    const filters = this.componentFilterStorageService.getFilters();

    if (isTruthy(filters) && filters.length > 0) {
      setTimeout(() => {
        this.componentFilterService.loadFilters(filters);
      }, 0);
    }

    if (this.defaultFilterRequest$) {
      this.subscriptions.push(this.defaultFilterRequest$.pipe(
        withLatestFrom(this.translationsService.translations)
      ).subscribe((data) => {
        if (this.enableDefaultFilter) {
          this.loadFilters(data[0].filter, data[1]);
        }
      }));
    }

    if (this.refreshRequest$) {
      this.subscriptions.push(this.refreshRequest$.subscribe(() => {
        const appliedFilters = this.componentFilterService.getFilters();
        if (appliedFilters) {
          this.componentFilterService.loadFilters(appliedFilters);
        }
      }));
    }

    this.subscriptions.push(this.componentFilterService.appliedFilters$.subscribe(appliedFilters => {
      this.filterChanged.emit(appliedFilters);
      if (this.isApiFilter) {
        this.apiFilter(appliedFilters);
        if (this.componentFilterService.isDeleteFilter) {
          this.componentFilterService.isDeleteFilter = false;
        } else {
          this.popoverDirective.toggle();
        }
      }
    }));
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['properties'] && this.properties) {
      this.componentFilterService.updateFilterProperties(this.properties);
    }
    if (changes['data'] && this.data) {
      if (this.emittingData) {
        this.emittingData = false;
      } else {
        setTimeout(() => {
          this.componentFilterService.initaliseData(this.data);
        }, 0);
      }
    }
  }

  ngOnDestroy() {
    this.subscriptions.forEach(x => x.unsubscribe());

    const filters = this.componentFilterService.getFilters();
    this.componentFilterStorageService.storeFilters(filters);
  }

  toggleFilter() {
    this.showFilter = !this.showFilter;
  }

  loadFilters(filter: ComponentFilter[], translations: string[]) {
    filter.forEach(x => {
      x.filterType = this.loadFilterType(x.filterTypeId, translations);
    });
    this.componentFilterService.loadFilters(filter);
  }

  loadFilterType(filterTypeId: number, translations: string[]) {
    switch (filterTypeId) {
      case ComponentFilterTypeEnum.equal:
        return translations['componentFilter.filterTypes.equal'];
      case ComponentFilterTypeEnum.notEqual:
        return translations['componentFilter.filterTypes.notEqual'];
      default:
        return null;
    }
  }

  private apiFilter(appliedFilters: ComponentFilter[]) {
    let filters: IFilter[] = [];
    if (appliedFilters && appliedFilters.length) {
      appliedFilters.forEach(x => {
        filters.push({
          filterOperation: this.apiFilterOperation(x.filterTypeId),
          name: x.propertyKey,
          value: x.filterTextValue ?? x.filterNumberValue ?? x.filterDateValue ?? x.filterBooleanValue ?? x.filterEnumValue
        });
      });
    }
    this.apiFilterChange.emit(filters);
  }

  private apiFilterOperation(filterTypeId: number) {
    let filterOperation: FilterOperation;
    if (filterTypeId) {
      switch (filterTypeId) {
        case ComponentFilterTypeEnum.equal:
          filterOperation = FilterOperation.Equal;
          break;
        case ComponentFilterTypeEnum.notEqual:
          filterOperation = FilterOperation.NotEqual;
          break;
        case ComponentFilterTypeEnum.startsWith:
          filterOperation = FilterOperation.StartsWith;
          break;
        case ComponentFilterTypeEnum.endsWith:
          filterOperation = FilterOperation.EndsWith;
          break;
        case ComponentFilterTypeEnum.contains:
          filterOperation = FilterOperation.Contains;
          break;
        case ComponentFilterTypeEnum.greaterThan:
          filterOperation = FilterOperation.GreaterThan;
          break;
        case ComponentFilterTypeEnum.greaterThanOrEqual:
          filterOperation = FilterOperation.GreaterThanOrEqual;
          break;
        case ComponentFilterTypeEnum.lessThan:
          filterOperation = FilterOperation.LessThan;
          break;
        case ComponentFilterTypeEnum.lessThanOrEqual:
          filterOperation = FilterOperation.LessThanOrEqual;
          break;
        case ComponentFilterTypeEnum.isEmpty:
          filterOperation = FilterOperation.Equal;
          break;
        case ComponentFilterTypeEnum.isNotEmpty:
          filterOperation = FilterOperation.NotEqual;
          break;
      }
    }
    return filterOperation;
  }
}
