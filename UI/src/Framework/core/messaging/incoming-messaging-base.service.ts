import { environment } from '@environments/environment';
import { PlatformCommunicationMethods } from '@amcs/platform-communication';

export abstract class IncomingMessagingBase {
  messageHandler: PlatformCommunicationMethods;

  /**
   * Setup the core messaging handler
   * To support legacy we always register incoming messages from the window.onmessage api
   */
  initialiseIncomingMessaging(methods?: PlatformCommunicationMethods) {
    this.messageHandler = methods ?? this.availableMethods();
  }

  abstract availableMethods(): PlatformCommunicationMethods;
  /**
   * Handles incoming messages from window.onmessage
   * @param event
   */
  handleMessage(event: MessageEvent) {
    const message: MessageEvent = event as MessageEvent;
    if (this.isValidMessage(event)) {
      const messageKey = message.data[0];
      const messagePayload = message.data[1];

      const handler = this.messageHandler[messageKey] as Function;
      if (handler) {
        if (!environment.production) {
          console.log(`Received ${messageKey}`);
        }
        handler(messagePayload);
      }
    }
  }

  /**
   * Verify a MessageEvent is valid
   * @param message
   * @returns
   */
  isValidMessage(message: MessageEvent) {
    return message && message.data[0];
  }
}
