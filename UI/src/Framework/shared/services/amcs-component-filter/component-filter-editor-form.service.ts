import { Injectable } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ILookupItem } from '@core-module/models/lookups/lookup-item.interface';
import { BaseService } from '@coreservices/base.service';
import { ComponentFilterForm } from '@shared-module/components/amcs-component-filter/component-filter-form.model';
import { ComponentFilterPropertyValueType } from '@shared-module/components/amcs-component-filter/component-filter-property-value-type.enum';
import { ComponentFilterProperty } from '@shared-module/components/amcs-component-filter/component-filter-property.model';
import { ComponentFilterTypeEnum } from '@shared-module/components/amcs-component-filter/component-filter-type.enum';
import { ComponentFilterType } from '@shared-module/components/amcs-component-filter/component-filter-type.model';
import { ComponentFilter } from '@shared-module/components/amcs-component-filter/component-filter.model';
import { AmcsDatepickerConfig } from '@shared-module/components/amcs-datepicker/amcs-datepicker-config.model';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { BehaviorSubject } from 'rxjs';
import { take, takeUntil } from 'rxjs/operators';
import { isTruthy } from '../../../core/helpers/is-truthy.function';
import { ComponentFilterEditorService } from './component-filter-editor.service';

@Injectable()
export class ComponentFilterFormService extends BaseService {
  static providers = [ComponentFilterFormService, ComponentFilterEditorService];

  form: ComponentFilterForm;

  filterProperties: ComponentFilterProperty[];
  filterTypes: ComponentFilterType[];
  propertyType = ComponentFilterPropertyValueType.text;
  ComponentFilterPropertyValueType = ComponentFilterPropertyValueType;
  booleanFilterTypes: ILookupItem[] = [];
  enumFilterTypes: ILookupItem[] = [];

  dateValueConfig: AmcsDatepickerConfig = Object.assign({}, { containerClass: 'amcs-datepicker' });

  initialised = new BehaviorSubject<boolean>(false);

  propertyPlaceholder: string;
  filterTypePlaceholder: string;
  booleanTypePlaceholder: string;
  enumTypePlaceholder: string;

  constructor(
    private formBuilder: FormBuilder,
    private componentFilterEditorService: ComponentFilterEditorService,
    private translationsService: SharedTranslationsService
  ) {
    super();
    this.setupFilterTypesStream();
    this.translationsService.translations.pipe(take(1)).subscribe((translations) => {
      this.propertyPlaceholder = translations['componentFilter.editor.propertyPlaceholder'];
      this.filterTypePlaceholder = translations['componentFilter.editor.filterTypePlaceholder'];
      this.booleanTypePlaceholder = translations['componentFilter.editor.booleanTypePlaceholder'];
      this.enumTypePlaceholder = translations['componentFilter.editor.enumTypePlaceholder'];

      if (this.booleanFilterTypes.length === 0) {
        this.booleanFilterTypes = [
          { id: 0, description: translations['componentFilter.editor.booleanFilter.false'] },
          { id: 1, description: translations['componentFilter.editor.booleanFilter.true'] },
        ];
      }
    });
  }

  buildForm() {
    this.componentFilterEditorService.componentFilterProperties$
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((filterProperties: ComponentFilterProperty[]) => {
        this.filterProperties = filterProperties;
      });
    this.form = AmcsFormBuilder.buildForm(this.formBuilder, new ComponentFilter(), ComponentFilterForm);
    this.setUpPropertyChangesStream();
    this.initialised.next(true);
  }

  save(event: any) {
    if (isTruthy(event)) {
      event.stopPropagation();
    }

    if (this.form.checkIfValid()) {
      const filter: ComponentFilter = AmcsFormBuilder.parseForm(this.form, ComponentFilterForm);

      const property = this.filterProperties.find((c) => c.id === filter.propertyId);
      filter.property = property.description;
      filter.propertyValueType = property.type;
      filter.propertyKey = property.propertyKey;

      const filterType = this.filterTypes.find((f) => f.id === filter.filterTypeId);
      filter.filterType = filterType.description;

      if(filter.propertyValueType === ComponentFilterPropertyValueType.enum) {
        filter.filterEnumDescriptionValue = this.getDescriptionFromEnum(filter.filterEnumValue);
      }

      this.componentFilterEditorService.sendFilter(filter);
      this.form.htmlFormGroup.reset();
      this.componentFilterEditorService.closeEditor();
    }
  }

  return(event: any) {
    event.stopPropagation();
    this.componentFilterEditorService.closeEditor();
  }

  private getDescriptionFromEnum(selectedValue: number): string {
    return this.enumFilterTypes?.find((enums) => enums.id === selectedValue)?.description;
  }

  private setupFilterTypesStream() {
    this.componentFilterEditorService.componentFilterTypes$
      .pipe(takeUntil(this.unsubscribe))
      .subscribe((filterTypes: ComponentFilterType[]) => {
        this.filterTypes = filterTypes;
      });
  }

  private setUpPropertyChangesStream() {
    this.form.property.valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe((propertyId) => {
      if (propertyId) {
        const property = this.filterProperties.find((c) => c.id === propertyId);
        this.propertyType = property.type;
        this.enumFilterTypes = property.enumTypeItems ?? [];
        this.componentFilterEditorService.requestComponentFilterTypes(property.type, property.textOptionsRestricted);
        this.updateValueValidation(this.propertyType);
      }
    });

    this.form.filterType.valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe((filterType) => {
      if (filterType === ComponentFilterTypeEnum.isEmpty || filterType === ComponentFilterTypeEnum.isNotEmpty) {
        this.form.filterTextValue.disable();
      } else {
        if (this.form.filterTextValue.disabled) {
          this.form.filterTextValue.enable();
        }
      }
    });
  }

  private updateValueValidation(columnType: number) {
    switch (columnType) {
      case ComponentFilterPropertyValueType.text: {
        this.form.filterTextValue.setValidators([Validators.required]);
        this.form.filterEnumValue.clearValidators();
        this.form.filterNumberValue.clearValidators();
        this.form.filterDateValue.clearValidators();
        break;
      }
      case ComponentFilterPropertyValueType.number: {
        this.form.filterNumberValue.setValidators([Validators.required]);
        this.form.filterEnumValue.clearValidators();
        this.form.filterTextValue.clearValidators();
        this.form.filterDateValue.clearValidators();
        break;
      }
      case ComponentFilterPropertyValueType.date: {
        this.form.filterDateValue.setValidators([Validators.required]);
        this.form.filterEnumValue.clearValidators();
        this.form.filterTextValue.clearValidators();
        this.form.filterNumberValue.clearValidators();
        break;
      }
      case ComponentFilterPropertyValueType.enum: {
        this.form.filterEnumValue.setValidators([Validators.required]);
        this.form.filterDateValue.clearValidators();
        this.form.filterTextValue.clearValidators();
        this.form.filterNumberValue.clearValidators();
        break;
      }
      default:
        return;
    }
    this.form.filterEnumValue.updateValueAndValidity();
    this.form.filterTextValue.updateValueAndValidity();
    this.form.filterNumberValue.updateValueAndValidity();
    this.form.filterDateValue.updateValueAndValidity();
  }
}
