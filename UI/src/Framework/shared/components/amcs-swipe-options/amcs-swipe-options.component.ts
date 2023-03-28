import { animate, state, style, transition, trigger } from '@angular/animations';
import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-amcs-swipe-options',
  templateUrl: './amcs-swipe-options.component.html',
  styleUrls: ['./amcs-swipe-options.component.scss'],
  animations: [
    trigger('showHideOptions', [
      state('false', style({
        transform: 'translateX(0)'
      })),
      state('true', style({
        transform: 'translateX(-{{optionsWidth}})'
      }), { params: { optionsWidth: '100px' } }),
      transition('false=>true', animate('0.3s ease-in')),
      transition('true=>false', animate('0.3s ease-out'))
    ])
  ]
})
export class AmcsSwipeOptionsComponent {

  @Input() height = '100px';
  @Input() optionsWidth = '100px';
  @Input() customClass: string;
  showOptions = 'false';

  onSwipe(e: any) {
    const x = Math.abs(e.deltaX) > 40 ? (e.deltaX > 0 ? 'Right' : 'Left') : '';

    switch (x) {
      case 'Left':
        this.showOptions = 'true';
        break;
      case 'Right':
        this.showOptions = 'false';
        break;
    }
  }
}
