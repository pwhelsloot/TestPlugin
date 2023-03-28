// eslint-disable-next-line max-classes-per-file
export interface DateCellEditorConfig {
  range: DateCellEditorRange;
  /**
   * translation key
   */
  error: string;
  isRequired: boolean;
}

export interface DateCellEditorRange {
  min?: string;
  max?: string;
}
