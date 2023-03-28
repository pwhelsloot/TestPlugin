export abstract class ResizeableColumn {
    key: string;
    widthPercentage: number;
    abstract afterResize();
}
