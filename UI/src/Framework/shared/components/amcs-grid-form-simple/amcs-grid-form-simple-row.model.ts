import { IGridFormSimpleRowForm } from './amcs-grid-form-simple-row-form.interface';

export class AmcsGridFormSimpleRow {

  isError = false;

  constructor(
    public id: number,
    public description: string,
    public form: IGridFormSimpleRowForm,
    public lookup?: any,
    public rowTextHighlighted?: boolean,
    public tooltipText?: string) {
  }
}
