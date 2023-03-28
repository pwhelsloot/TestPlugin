import { Injectable, NgZone, OnDestroy } from '@angular/core';
import { isNullOrUndefined, isTruthy } from '@core-module/helpers/is-truthy.function';
import { environment } from '@environments/environment';
// import { environment } from '@environments/environment';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ReplaySubject, Subject, Subscription } from 'rxjs';
import { SignalRCallbackFn } from './signalr-event';
import { SignalREventListener } from './signalr-event-listener';
import { SignalREventListenerUnsubscriber } from './signalr-event-listener-unsubscriber';
import { SignalRStatus } from './signalr-status.model';

@Injectable()
export class SignalRClient implements OnDestroy {
  readonly connectionLost$ = new Subject<boolean>();
  readonly connectionError$ = new Subject<boolean>();
  readonly connectionActive$ = new Subject<boolean>();
  readonly reconnected$ = new Subject<string>();

  constructor(private readonly zone: NgZone) {}

  private origin = `${window.coreServiceRoot}/${environment.applicationApiPrefix}`;
  private endPoint = 'signalr';
  private statusSubscription: Subscription;
  private listeners: { [eventName: string]: SignalRCallbackFn[] };
  private connection: HubConnection;
  private status$: ReplaySubject<SignalRStatus>;

  private get HubUrl(): string {
    const fullUrl = `${this.origin}/${this.endPoint}`;
    return fullUrl;
  }

  ngOnDestroy(): void {
    if (this.statusSubscription) {
      this.statusSubscription.unsubscribe();
    }
    this.destroyConnection();
  }

  /**
   * Setup connection with SignalR
   * @param endPoint endpoint which should match with the url used in the HubRegistration
   * @param origin optional origin, defaults to coreServiceRoot and applicationApiPrefix
   */
  create(endPoint: string, origin?: string): void {
    if (isTruthy(endPoint)) {
      this.endPoint = endPoint;
    }

    if (isTruthy(origin)) {
      this.origin = origin;
    }

    this.setupConnection();
  }

  listenFor<T = any>(event: string, next: (value: T) => void): Subscription {
    if (!this.hasActiveConnection()) {
      this.log(`Attempted to set listener: ${event} on empty connection`);
      return Subscription.EMPTY;
    } else {
      const listener = new SignalREventListener<T>(event);
      this.listen(listener);

      const subscription = listener.subscribe(next);
      subscription.add(new SignalREventListenerUnsubscriber<T>(this, listener));
      return subscription;
    }
  }

  stopListening<T>(listener: SignalREventListener<T>): void {
    if (isNullOrUndefined(listener)) {
      throw new Error('Listener cannot be empty!');
    }

    if (!this.listeners[listener.event]) {
      this.listeners[listener.event] = [];
    }

    if (!this.hasActiveConnection()) {
      this.log('Attempted to remove listener from empty connection');
    } else {
      for (const callback of this.listeners[listener.event]) {
        this.connection.off(listener.event, callback);
      }
    }

    this.listeners[listener.event] = [];
  }

  log(message: string): void {
    console.log(`[SignalR]: ${message}`);
  }

  private start(): void {
    this.status$ = new ReplaySubject<SignalRStatus>(1);
    this.statusSubscription = this.status$.subscribe((status) => {
      if (status === SignalRStatus.active) {
        this.connectionActive$.next();
      } else if (status === SignalRStatus.disconnected) {
        this.connectionLost$.next();
      } else if (status === SignalRStatus.error) {
        this.connectionError$.next();
      }
    });
    if (this.hasActiveConnection()) {
      this.listeners = {};
      this.connection
        .start()
        .then(() => {
          this.status$.next(SignalRStatus.active);
        })
        .catch((err) => {
          this.status$.next(SignalRStatus.error);
        });
    } else {
      this.status$.next(SignalRStatus.disconnected);
    }
  }

  private setupConnection(): void {
    this.connection = new HubConnectionBuilder()
      .withUrl(this.HubUrl)
      .withAutomaticReconnect([0, 3000, 5000, 10000, 15000, 30000])
      .build();

    this.start();

    this.connection.onclose((error) => {
      if (isTruthy(error)) {
        this.status$.next(SignalRStatus.error); // TODO: Error message?
      } else {
        this.status$.next(SignalRStatus.disconnected);
      }
      this.connection = undefined;
      this.log(`Client disconnected with error ${error}`);
    });

    this.connection.onreconnected((id) => {
      this.reconnected$.next(id);
    });
  }

  private listen<T>(listener: SignalREventListener<T>): void {
    if (isNullOrUndefined(listener)) {
      throw new Error('Listener cannot be empty!');
    }

    const callback: SignalRCallbackFn = (...args: any[]) => {
      this.run(() => {
        let casted: T = null;
        if (args.length > 0) {
          casted = args[0] as T;
        }
        listener.next(casted);
      });
    };
    this.setListener(callback, listener);
  }

  private setListener<T>(callback: SignalRCallbackFn, listener: SignalREventListener<T>): void {
    this.connection.on(listener.event, callback);

    if (isNullOrUndefined(this.listeners[listener.event])) {
      this.listeners[listener.event] = [];
    }

    this.listeners[listener.event].push(callback);
  }

  private destroyConnection() {
    if (this.connection) {
      return this.connection.stop().then();
    } else {
      return null;
    }
  }

  private run(func: () => void) {
    this.zone.run(() => func()); // or outside of angular?
  }

  private hasActiveConnection() {
    return !!this.connection;
  }
}
