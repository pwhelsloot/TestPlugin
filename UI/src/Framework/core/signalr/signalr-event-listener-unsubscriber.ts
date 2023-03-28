import { SignalRClient } from '@core-module/signalr/signalr-client.service';
import { Unsubscribable } from 'rxjs';
import { SignalREventListener } from './signalr-event-listener';

export class SignalREventListenerUnsubscriber<T> implements Unsubscribable {
  constructor(private readonly signalRClient: SignalRClient, private readonly eventListener: SignalREventListener<T>) {
  }

  unsubscribe(): void {
    this.signalRClient.stopListening(this.eventListener);
  }
}
