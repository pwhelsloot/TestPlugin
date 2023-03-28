import { Subject } from 'rxjs';

export class AiViewReady extends Subject<boolean> {
  next(value?: boolean) {
    super.next(value);
  }
}
