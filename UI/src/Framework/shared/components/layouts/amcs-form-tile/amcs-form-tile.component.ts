import { Component, EventEmitter, forwardRef, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { ControlContainer, FormControl } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { FormDisplayModeEnum } from '@core-module/models/forms/form-display-model.enum';
import { AmcsFormDirective, createFormGroupDirective } from '../amcs-form/amcs-form.directive';
import { FormTileOptions } from './form-tile-options.model';

@Component({
  selector: 'app-amcs-form-tile',
  templateUrl: './amcs-form-tile.component.html',
  styleUrls: ['./amcs-form-tile.component.scss'],
  // RDM - Help for this from here https://stackoverflow.com/questions/53561484/access-formgroupdirective-of-container-component-inside-an-embeddedview
  // We want to have our 'formControlName' controls in a different component from the [formGroup] tag, only possible via providing a controlContainer.
  providers: [
    { provide: AmcsFormDirective, useExisting: forwardRef(() => AmcsFormTileComponent) },
    {
      provide: ControlContainer,
      useFactory: createFormGroupDirective,
      deps: [AmcsFormDirective]
    }
  ]
})
export class AmcsFormTileComponent extends AmcsFormDirective implements OnInit, OnChanges {
  loadingContainerSize = 120;
  enableFormActions = false;
  FormDisplayMode = FormDisplayModeEnum;

  /**
 * Options that will be used for the Form Tile
 *
 * @type {FormTileOptions}
 * @memberof AmcsFormTileComponent
 */
  @Input() options: FormTileOptions;

  /**
 * Emitted when the return button is clicked
 *
 * @memberof AmcsFormTileComponent
 */
  @Output() onReturn = new EventEmitter();

  /**
   * Emitted when the log button is clicked
   *
   * @memberof AmcsFormTileComponent
   */
  @Output() onViewLog = new EventEmitter();

  /**
 * AmcsFormComponent outputs
 */
  @Output() onSave = new EventEmitter();
  @Output() onContinue = new EventEmitter();
  @Output() onCancel = new EventEmitter();
  @Output() onBack = new EventEmitter();

  isEditMode = false;
  hasButtons = false;

  ngOnInit(): void {
    this.setUpEditMode();
    this.setUpButtons();
  }

  ngOnChanges(changes: SimpleChanges): void {
    super.ngOnChanges(changes);
    if (changes['form']) {
      this.setUpEditMode();
    }
    if (changes['options']) {
      if (isTruthy(this.options)) {
        this.loadingContainerSize = this.options.formOptions.loadingContainerSize;
        this.enableFormActions = this.options.formOptions.enableFormActions;
      }
      this.setUpButtons();
    }
  }

  private setUpButtons(): void {
    if (!isTruthy(this.options)) {
      this.hasButtons = false;
      return;
    }
    this.hasButtons = this.options.enableReturn || this.options.enableLog;
  }

  private setUpEditMode(): void {
    if (!isTruthy(this.form)) {
      this.isEditMode = false;
      return;
    }
    const primaryKey: FormControl = this.form.getPrimaryKeyControl();
    this.isEditMode = isTruthy(primaryKey.value);
  }
}
