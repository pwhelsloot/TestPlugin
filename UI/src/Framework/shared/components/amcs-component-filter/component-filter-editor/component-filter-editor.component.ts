import { Component, ElementRef, Input, OnInit, Renderer2 } from '@angular/core';
import { FormControlDisplay } from '@core-module/models/forms/form-control-display.enum';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ComponentFilterFormService } from '@shared-module/services/amcs-component-filter/component-filter-editor-form.service';

@Component({
  selector: 'app-component-filter-editor',
  templateUrl: './component-filter-editor.component.html',
  styleUrls: ['./component-filter-editor.component.scss'],
  providers: [ComponentFilterFormService.providers]
})
export class ComponentFilterEditorComponent extends AutomationLocatorDirective implements OnInit {

  displayMode = FormControlDisplay.Grid;
  @Input('isOverlay') isOverlay: boolean;

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public formService: ComponentFilterFormService) {
    super(elRef, renderer);
  }

  ngOnInit() {
    this.formService.buildForm();
  }

}
