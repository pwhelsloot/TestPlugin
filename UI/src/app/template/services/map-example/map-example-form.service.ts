import { Injectable } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { AmcsFormService } from '@core-module/services/amcs-form.service';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { take, switchMap } from 'rxjs/operators';
import { MapExampleEditorData } from '@app/template/models/map-example/map-example-editor-data.model';
import { TemplateTranslationsService } from '../template-translations.service';
import { MapExampleForm } from '@app/template/models/map-example/map-example-form';
import { MapExampleService } from './map-example.service';
import { FormOptions } from '@shared-module/components/layouts/amcs-form/form-options.model';

@Injectable()
export class MapExampleFormService extends AmcsFormService<MapExampleEditorData> {
  static providers = [MapExampleFormService, MapExampleService];

  mapPointId: number;
  form: MapExampleForm = null;
  formOptions = new FormOptions();
  editorData: MapExampleEditorData;

  constructor(
    private readonly translationsService: TemplateTranslationsService,
    private readonly formBuilder: FormBuilder,
    private readonly mapExampleService: MapExampleService
  ) {
    super();
  }

  buildForm() {
    this.populateForm();
  }

  populateForm() {
    this.form = AmcsFormBuilder.buildForm(this.formBuilder, this.editorData.mapExample, MapExampleForm);
  }

  save() {
    const mapPoint = AmcsFormBuilder.parseForm(this.form, MapExampleForm, this.mapPointId);
    return this.translationsService.translations.pipe(
      take(1),
      switchMap((translations: string[]) =>
        this.mapExampleService.saveMapPoint(mapPoint, translations['template.mapExample.editor.mapPointSavedNotification'])
      )
    );
  }

  isValid() {
    return this.form.checkIfValid();
  }

  return() {}
}
