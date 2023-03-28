import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-amcs-parent-card',
  templateUrl: './amcs-parent-card.component.html',
  styleUrls: ['./amcs-parent-card.component.scss']
})
export class AmcsParentCardComponent {

  @Input() removePadding = false;
  @Input() customClass: string = null;
  @Input() loading = false;

}
