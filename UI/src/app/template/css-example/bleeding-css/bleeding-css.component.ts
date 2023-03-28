import { Component, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-bleeding-css',
  templateUrl: './bleeding-css.component.html',
  styleUrls: ['./bleeding-css.component.scss'],
  encapsulation: ViewEncapsulation.None, // NEVER USE THIS!!
})
export class BleedingCssComponent implements OnInit {
  constructor() {}

  ngOnInit() {}
}
