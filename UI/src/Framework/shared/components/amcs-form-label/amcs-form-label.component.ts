import { Component, Input } from '@angular/core';

@Component({
  // eslint-disable-next-line @angular-eslint/component-selector
  selector: '[app-amcs-form-label]',
  templateUrl: './amcs-form-label.component.html',
  styleUrls: ['./amcs-form-label.component.scss'],
})
export class AmcsFormLabelComponent {
  @Input()
  label: string;

  @Input()
  isRequired: boolean;
}
