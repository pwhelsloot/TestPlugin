import { Observable, ReplaySubject } from 'rxjs';

export class Configuration<T = string | boolean> {
  readonly valueAsync: Observable<T>;
  readonly configId: string;

  get value(): T {
    return this.currentValueField;
  }

  constructor(configId: string) {
    this.configId = configId;
    this.valueAsync = this.valueSubject.asObservable();
  }

  private readonly valueSubject: ReplaySubject<T> = new ReplaySubject<T>(1);
  private currentValueField: T;

  setValue(value: T) {
    const typedValue: T = value as T;
    this.valueSubject.next(typedValue);
    this.currentValueField = typedValue;
  }
}
