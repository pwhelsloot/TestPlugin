import { FormControl } from '@angular/forms';
import { BaseForm } from '@shared-module/forms/base-form.model';
import { SnippetExample } from './snippet-example.model';

// I'm an form data-model created via 'form-model' snippet.
export class SnippetExampleForm extends BaseForm<SnippetExample, SnippetExampleForm> {
  id: FormControl;

  buildForm(dataModel: SnippetExample, extraParams: any[]): SnippetExampleForm {
    const form = new SnippetExampleForm();
    form.id = new FormControl(dataModel.snippetExampleId);
    return form;
  }

  parseForm(typedForm: SnippetExampleForm, extraParams: any[]): SnippetExample {
    const dataModel = new SnippetExample();
    dataModel.snippetExampleId = typedForm.id.value;
    return dataModel;
  }
}
