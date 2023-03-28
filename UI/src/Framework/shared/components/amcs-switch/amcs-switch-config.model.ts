export class AmcsSwitchConfig {
  /** The text to show on the ON label of the switch */
  onText?: string;

  /** The text to show on the OFF label of the switch */
  offText?: string;

  /** The color of the ON label of the switch (primary,info,success,warning,danger,default) */
  onColor?: string;

  /** The color of the OFF label of the switch (primary,info,success,warning,danger,default) */
  offColor?: string;

  /** The size of the switch ("mini"|"small"|"normal"|"large") */
  size?: string;

  /** Boolean value to enable/disabled the switch */
  isDisabled?: boolean;

  /** Boolean value to flip the true/false values of the switch */
  isInverse?: boolean;
}
