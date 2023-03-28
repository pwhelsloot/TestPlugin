import { GridViewport } from './grid-viewport.enum';

export class GridColumnViewport {
  viewport: GridViewport;
  visible: boolean;
  widthPercentage: number;

  constructor(viewport: GridViewport, visible: boolean, widthPercentage?: number) {
    this.viewport = viewport;
    this.visible = visible;
    this.widthPercentage = widthPercentage || null;
  }
}
