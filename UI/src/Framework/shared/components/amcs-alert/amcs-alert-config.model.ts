export class AmcsAlertConfig {
  // Provides one of four bootstrap supported contextual classes: success, info, warning and danger
  type: string;
  /** Is alert visible */
  isOpen: boolean;
  /** is alert dismissible by default */
  dismissible: boolean;
  /** default time before alert will dismiss */
  dismissOnTimeout?: number;
  /** is icon visable on the the alert */
  showIcon: boolean;

  message: string;
}
