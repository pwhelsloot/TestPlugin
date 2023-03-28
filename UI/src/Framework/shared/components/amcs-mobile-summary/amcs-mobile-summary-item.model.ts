import { AmcsMobileSummaryItemEnum } from './amcs-mobile-summary-item-type.enum';

export class AmcsMobileSummaryItem {
  constructor(public id: number, public title: string, public value: string, public type = AmcsMobileSummaryItemEnum.Text, public customClass?: string) { }
}
