import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-amcs-card',
  templateUrl: './amcs-card.component.html',
  styleUrls: ['./amcs-card.component.scss']
})
export class AmcsCardComponent {
  @Input() customClass = '';
  @Input() loading = false;
}
