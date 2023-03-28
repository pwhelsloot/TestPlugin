import { Component, OnInit } from '@angular/core';
import { aiComponent } from '@core-module/services/logging/ai-decorators';
import { AiViewReady } from '@core-module/services/logging/ai-view-ready.model';
import { AmcsDropdownIconEnum } from '@shared-module/components/amcs-dropdown/amcs-dropdown-icon-enum.model';
import { TemplateIngredientService } from '../services/template-ingredient.service';

@Component({
  selector: 'app-template-ingredients',
  templateUrl: './template-ingredients.component.html',
  styleUrls: ['./template-ingredients.component.scss'],
  providers: [TemplateIngredientService],
})
@aiComponent('Template Ingredients')
export class TemplateIngredientsComponent implements OnInit {
  AmcsDropdownIconEnum = AmcsDropdownIconEnum;
  viewReady: AiViewReady;

  constructor(public service: TemplateIngredientService) {
    this.viewReady = this.service.viewReady;
  }

  ngOnInit() {}
}
