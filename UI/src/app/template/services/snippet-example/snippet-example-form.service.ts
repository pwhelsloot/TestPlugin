import { Injectable } from '@angular/core';
import { BaseService } from '@core-module/services/base.service';
import { FormBuilder } from '@angular/forms';
import { PreviousRouteService } from '@core-module/services/previous-route.service';
import { ActivatedRoute, Params } from '@angular/router';
import { take, switchMap } from 'rxjs/operators';
import { AmcsFormBuilder } from '@shared-module/forms/amcs-form-builder.model';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { SnippetExampleForm } from '@app/template/models/snippets-example/snippet-example-form.model';
import { SnippetExampleEditorData } from '@app/template/models/snippets-example/snippet-example-editor-data.model';
import { TemplateTranslationsService } from '../template-translations.service';
import { TemplateModuleAppRoutes } from '@app/template/template-module-app-routes.constants';
import { ApiBusinessService } from '@core-module/services/service-structure/api-business.service';
import { ApiFilters } from '@core-module/services/service-structure/api-filter-builder';
import { ApiFilter } from '@core-module/services/service-structure/api-filter';
import { SnippetExample } from '@app/template/models/snippets-example/snippet-example.model';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';

// I'm a form service created via 'form-service' snippet.
@Injectable()
export class SnippetExampleFormService extends BaseService {
  static providers = [SnippetExampleFormService];

  loading = true;
  form: SnippetExampleForm = null;
  editorData: SnippetExampleEditorData = null;
  viewReady = new AiViewReady();

  constructor(
    private readonly formBuilder: FormBuilder,
    private readonly businessService: ApiBusinessService,
    private readonly translationsService: TemplateTranslationsService,
    private readonly previousRouteService: PreviousRouteService,
    private readonly route: ActivatedRoute
  ) {
    super();
  }

  requestEditorData() {
    this.route.params.pipe(take(1)).subscribe(async (params: Params) => {
      let id: number = +params['id'];
      if (isTruthy(id)) {
        id = null;
      }
      this.editorData = await this.businessService.getAsync<SnippetExampleEditorData>(
        new ApiFilters().add(ApiFilter.equal('SnippetExampleId', id)).build(),
        SnippetExampleEditorData
      );
      this.buildForm();
      this.viewReady.next();
      this.loading = false;
    });
  }

  save() {
    if (this.form.checkIfValid()) {
      this.translationsService.translations
        .pipe(
          take(1),
          switchMap((translations: string[]) => {
            return this.businessService.save<SnippetExample>(
              AmcsFormBuilder.parseForm(this.form, SnippetExampleForm),
              translations['template.snippetExample.saved'],
              SnippetExample
            );
          })
        )
        .subscribe((id: number) => {
          if (id !== null) {
            this.return();
          }
        });
    }
  }

  return() {
    // RDM - 'returnScreen' was manually changed here to 'dashboard'
    this.previousRouteService.navigationToPreviousUrl(['../' + TemplateModuleAppRoutes.dashboard], { relativeTo: this.route });
  }

  private buildForm() {
    this.form = AmcsFormBuilder.buildForm(this.formBuilder, this.editorData.dataModel, SnippetExampleForm, this.formBuilder);
  }
}
