import { TooltipOptions } from 'leaflet';

export interface AmcsCustomTooltipOptions extends TooltipOptions {
  // can be used to change classes in tooltip when updating marker Icon ((de)selecting marker)
  selectedTooltipClassName?: string;
  originalTooltipClassName?: string;
  delayDuration?: number;
}
