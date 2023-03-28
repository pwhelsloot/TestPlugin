import { environment } from '@environments/environment';

export class PluginLogger {
  private static readonly sectorPrefix = '[Plugin]';

  static log(...parameters: Parameters<typeof console.log>) {
    if (this.isDev()) {
      console.log(this.formatMessage(parameters[0]), parameters);
    }
  }

  static logWarn(...parameters: Parameters<typeof console.warn>) {
    if (this.isDev()) {
      console.warn(this.formatMessage(parameters[0]), parameters);
    }
  }

  static logError(...parameters: Parameters<typeof console.error>) {
    if (this.isDev()) {
      console.error(this.formatMessage(parameters[0]), parameters);
    }
  }

  static isDev() {
    return !environment.production;
  }

  private static formatMessage(msg: string) {
    return `${this.sectorPrefix} ${msg}`;
  }
}
