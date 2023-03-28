import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable()
export class AmcsCalendarMultiDayService {
  isMultidayHovered = new Subject<boolean>();

  onMultidayHoverTrigger(isHovered: boolean) {
    this.isMultidayHovered.next(isHovered);
  }
}
