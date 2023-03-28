import { Component, HostBinding, Input, OnInit } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { getDesktopColumns, getMobileColumns, verifyIsValidBSColumnSize } from '../bootstrap-helper';
import { AmcsColumnWidth } from './amcs-column-width';

@Component({
  selector: 'app-amcs-column',
  templateUrl: './amcs-column.component.html',
  styleUrls: ['./amcs-column.component.scss'],
})
export class AmcsColumnComponent implements OnInit {
  /**
   * Size for all screens
   * if omitted will make column automatically scale
   *
   * @type {AmcsColumnWidth}
   * @memberof AmcsColumnComponent
   */
  @Input() size: AmcsColumnWidth;

  /**
   * Size for mobile screens
   *
   * @type {AmcsColumnWidth}
   * @memberof AmcsColumnComponent
   */
  @Input() mSize: AmcsColumnWidth;
  @HostBinding('class') hostClasses: string;

  ngOnInit() {
    if (isTruthy(this.size)) { verifyIsValidBSColumnSize(this.size, 'desktop size'); }
    if (isTruthy(this.mSize)) { verifyIsValidBSColumnSize(this.mSize, 'mobile size'); }
  }

  /**
   * Set column size classes
   *
   * @memberof AmcsColumnComponent
   */
  setColumnSizes(desktopSize: number) {
    const classes: string[] = [];
    if (this.size > 0) {
      classes.push(getDesktopColumns(this.size));
    } else if (desktopSize > 0) {
      classes.push(getDesktopColumns(desktopSize));
    } else {
      classes.push(getDesktopColumns(12));
    }

    if (this.mSize > 0) {
      classes.push(getMobileColumns(this.mSize));
    } else {
      classes.push(getMobileColumns(12));
    }
    this.hostClasses = classes.join(' ');
  }
}
