export interface ITableItem {
    id: number;
    isSelected: boolean;
    isSelectDisabled?: boolean;
    filter(text: string): boolean;
}
