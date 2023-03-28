import { isNullOrUndefined } from '@core-module/helpers/is-truthy.function';
import { Subject } from 'rxjs';

export class SignalREventListener<T> extends Subject<T> {
  constructor(readonly event: string) {
    super();
    if (isNullOrUndefined(event)) {
      throw new Error('Event cannot be empty!');
    }
  }
}
