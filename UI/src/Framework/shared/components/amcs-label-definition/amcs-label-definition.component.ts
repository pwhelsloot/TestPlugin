import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { Subscription } from 'rxjs';

/**
 * @deprecated Marked to be moved to settings module in PlatformUI
 */
@Component({
  selector: 'app-amcs-label-definition',
  templateUrl: './amcs-label-definition.component.html',
  styleUrls: ['./amcs-label-definition.component.scss']
})
export class AmcsLabelDefinitionComponent implements OnInit, OnDestroy {

  mandatoryTooltip: string;
  editableTooltip: string;
  visibleTooltip: string;
  notMandatoryTooltip: string;
  notEditableTooltip: string;
  notVisibleTooltip: string;
  @Input('isMandatory') isMandatory = false;
  @Input('isEditable') isEditable = false;
  @Input('isVisible') isVisible = false;
  @Input('showVisible') showVisible = true;

  constructor(private appTranslationsService: SharedTranslationsService) { }

  private translationSubscription: Subscription;

  ngOnInit() {
    this.translationSubscription = this.appTranslationsService.translations.subscribe((translations: string[]) => {
      this.mandatoryTooltip = translations['form.definition.mandatory'];
      this.notMandatoryTooltip = translations['form.definition.notmandatory'];
      this.editableTooltip = translations['form.definition.editable'];
      this.notEditableTooltip = translations['form.definition.noteditable'];
      this.visibleTooltip = translations['form.definition.visible'];
      this.notVisibleTooltip = translations['form.definition.notvisible'];
    });
  }

  ngOnDestroy() {
    this.translationSubscription.unsubscribe();
  }
}
