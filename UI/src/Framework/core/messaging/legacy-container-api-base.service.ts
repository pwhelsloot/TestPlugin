import { environment } from '@environments/environment';

/**
 * Base for creating a Legacy ContainerAppApi
 */
export abstract class LegacyContainerAppApiBase {
  constructor(readonly applicationName?: string) {}

  /**
   * The Window element to send messages to
   */
  protected contentWindow: Window;

  /**
   * Set the Window to use for this window.onmessage api
   */
  abstract setWindow();

  sendMessage(key: string, request) {
    if (!environment.production) {
      console.log(`Sending ${key}`);
    }
    this.contentWindow.postMessage([key, request], '*');
  }
}
