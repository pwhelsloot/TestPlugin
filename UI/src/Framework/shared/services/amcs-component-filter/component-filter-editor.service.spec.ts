import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { ComponentFilterTypeEnum } from '@shared-module/components/amcs-component-filter/component-filter-type.enum';
import { ComponentFilterType } from '@shared-module/components/amcs-component-filter/component-filter-type.model';
import { BehaviorSubject, of, Subject } from 'rxjs';
import { takeUntil } from 'rxjs/operators';
import { SharedTranslationsService } from '../shared-translations.service';
import { ComponentFilterService } from './amcs-component-filter.service';
import { ComponentFilterApiOptionService } from './component-filter-api-option.service';
import { ComponentFilterEditorService } from './component-filter-editor.service';

describe('ComponentFilterEditorService', () => {
  let target: ComponentFilterEditorService;

  let componentFilterService: ComponentFilterService;
  let translationService: SharedTranslationsService;
  let componentFilterApiOptionService: ComponentFilterApiOptionService;

  const destroy: Subject<void> = new Subject();
  const observer: jasmine.Spy = jasmine.createSpy('Map service Observer');
  const componentFilterProperty = [new ComponentFilterProperty(1, 'Prop1 description', 1, 'prop1')];

  const filterTypes: { [key: number]: ComponentFilterType[] } = {
    1: [
      new ComponentFilterType(ComponentFilterTypeEnum.equal, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.notEqual, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.startsWith, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.endsWith, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.contains, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.isEmpty, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.isNotEmpty, undefined),
    ],
    2: [
      new ComponentFilterType(ComponentFilterTypeEnum.lessThan, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.greaterThan, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.equal, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.lessThanOrEqual, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.greaterThanOrEqual, undefined),
    ],
    3: [
      new ComponentFilterType(ComponentFilterTypeEnum.lessThan, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.greaterThan, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.equal, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.lessThanOrEqual, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.greaterThanOrEqual, undefined),
    ],
    4: [
      new ComponentFilterType(ComponentFilterTypeEnum.equal, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.notEqual, undefined),
    ],
    5: [
      new ComponentFilterType(ComponentFilterTypeEnum.equal, undefined),
      new ComponentFilterType(ComponentFilterTypeEnum.notEqual, undefined),
    ],
  };

  beforeEach(() => {
    translationService = {} as SharedTranslationsService;
    translationService.translations = new BehaviorSubject<string[]>([]);

    componentFilterService = {} as ComponentFilterService;
    componentFilterService.filterProperties$ = of(componentFilterProperty);

    target = new ComponentFilterEditorService(componentFilterService, translationService, null);
    target.componentFilterTypes$.pipe(takeUntil(destroy)).subscribe(observer);
  });

  afterEach(() => {
    destroy.next();
    observer.calls.reset();
  });

  afterAll(() => {
    destroy.complete();
  });

  it('should create the service', () => {
    expect(target).toBeDefined();
  });

  describe('requestComponentFilterTypes', () => {
    describe('In-memory filter types', () => {
      it('should return all text filter types', () => {
        target.requestComponentFilterTypes(1);

        Assert(filterTypes[1]);
      });

      it('should return all number filter types', () => {
        target.requestComponentFilterTypes(2);

        Assert(filterTypes[2]);
      });

      it('should return all date filter types', () => {
        target.requestComponentFilterTypes(3);

        Assert(filterTypes[3]);
      });

      it('should return all boolean filter types', () => {
        target.requestComponentFilterTypes(4);

        Assert(filterTypes[4]);
      });

      it('should return all enum filter types', () => {
        target.requestComponentFilterTypes(4);

        Assert(filterTypes[4]);
      });
    });

    describe('Api filter types', () => {
      beforeEach(() => {
        componentFilterApiOptionService = {} as ComponentFilterApiOptionService;

        target = new ComponentFilterEditorService(componentFilterService, translationService, componentFilterApiOptionService);
        target.componentFilterTypes$.pipe(takeUntil(destroy)).subscribe(observer);
      });

      it('should return all text filter types', () => {
        target.requestComponentFilterTypes(1);
        const apiFilterTypes = filterTypes[1].slice(0, -2);
        Assert(apiFilterTypes);
      });

      it('should return all number filter types', () => {
        target.requestComponentFilterTypes(2);

        Assert(filterTypes[2]);
      });

      it('should return all date filter types', () => {
        target.requestComponentFilterTypes(3);

        Assert(filterTypes[3]);
      });

      it('should return all boolean filter types', () => {
        target.requestComponentFilterTypes(4);

        Assert(filterTypes[4]);
      });

      it('should return all boolean filter types', () => {
        target.requestComponentFilterTypes(5);

        Assert(filterTypes[5]);
      });
    });
  });

  function Assert(actualFilterTypes: ComponentFilterType[]) {
    expect(observer).toHaveBeenCalledTimes(1);
    expect(observer).toHaveBeenCalledOnceWith(actualFilterTypes);
  }
});
