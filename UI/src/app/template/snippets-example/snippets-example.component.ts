import { Component, OnInit } from '@angular/core';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';
import { SnippetExampleFormService } from '../services/snippet-example/snippet-example-form.service';

@Component({
  selector: 'app-snippets-example',
  templateUrl: './snippets-example.component.html',
  styleUrls: ['./snippets-example.component.scss'],
  providers: SnippetExampleFormService.providers,
})
@aiComponent('Template Snippets Example')
export class SnippetsExampleComponent implements OnInit {
  viewReady: AiViewReady;

  constructor(public formService: SnippetExampleFormService) {
    this.viewReady = formService.viewReady;
  }

  ngOnInit() {
    this.formService.requestEditorData();
  }
}
