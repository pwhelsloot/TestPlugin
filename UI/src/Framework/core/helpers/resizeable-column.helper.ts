import { ResizeableColumn } from '@core-module/models/grid/resizeable-column.model';
import { Subject } from 'rxjs';
import { throttleTime, takeUntil } from 'rxjs/operators';

export class ResizeableColumnHelper {

    constructor(private columns: ResizeableColumn[]) {
        this.columnArrayLength = this.columns.length;
        if (this.columnArrayLength <= 0) {
            return;
        }
        // A minimum percentage, needs to be relative to the no of columns but also ensure its never above 15%
        this.minimumPercentage = Math.min(60 / this.columnArrayLength, 15);
        // Calculate total percentage for all columns
        this.totalPercentage = this.columns.map(c => c.widthPercentage).reduce((sum, current) => sum + current);
        this.setUpResizeStream();
    }

    private unsubscribe = new Subject();
    private requestResize = new Subject<{ column: ResizeableColumn; newWidth: number }>();
    private columnOriginalWidth: number;
    private minimumPercentage: number;
    private hangoverPercentage: number;
    private totalPercentage: number;
    private columnArrayLength: number;

    onResizing(column: ResizeableColumn, newWidth: number) {
        this.requestResize.next({ column, newWidth });
    }

    onResizeStart(width: number) {
        this.columnOriginalWidth = width;
    }

    destroy() {
        this.unsubscribe.next();
        this.unsubscribe.complete();
    }

    // We use flex-basis % style to give columns width. This method handles a requested change in width of a column (in pixels) and can figure out
    // what new flex-basis % it needs. It also reduces/increases the flex-basis % of the other columns to ensure we maintain the same overall width.
    private setUpResizeStream() {
        this.requestResize.pipe(
            throttleTime(50, undefined, { leading: true, trailing: true }),
            takeUntil(this.unsubscribe)
        ).subscribe((data: { column: ResizeableColumn; newWidth: number }) => {
            // Calculate total available width
            const relativeFraction = data.column.widthPercentage / this.totalPercentage;
            const totalWidthOfColumns = this.columnOriginalWidth / relativeFraction;

            // Get new percentage
            const newPercentage = (data.newWidth / totalWidthOfColumns) * this.totalPercentage;

            // Ensure new percentage its not outwidth min or max.
            if (newPercentage < this.minimumPercentage || newPercentage > this.totalPercentage - ((this.columnArrayLength - 1) * this.minimumPercentage)) {
                return;
            }
            const newPercentageDifference = data.column.widthPercentage - newPercentage;

            // Set new percentage
            data.column.widthPercentage = newPercentage;
            this.columnOriginalWidth = data.newWidth;

            // Figure out how this affects other columns
            const percentageDifferencePerColumn: number = newPercentageDifference / (this.columnArrayLength - 1);

            // We don't want columns to go below minimum percentage so we might have some leftover % to apply to other columns
            this.hangoverPercentage = 0;
            this.columns.filter(x => x.key !== data.column.key).forEach((column: ResizeableColumn) => {
                this.updateColumnSize(column, percentageDifferencePerColumn);
            });

            // Final iteration through the items just incase we can get rid of the hangover %
            if (this.hangoverPercentage !== 0) {
                this.columns.filter(x => x.key !== data.column.key).forEach((column: ResizeableColumn) => {
                    this.updateColumnSize(column, 0);
                });
            }
            this.columns.forEach(element => {
                element.afterResize();
            });
        });
    }

    private updateColumnSize(column: ResizeableColumn, percentageDifferencePerColumn: number) {
        const percentageToAdd = this.hangoverPercentage + percentageDifferencePerColumn;
        // if result would be below minimum then just set minimum and add difference to hangover
        if ((column.widthPercentage + percentageToAdd) < this.minimumPercentage) {
            this.hangoverPercentage = (column.widthPercentage + percentageToAdd) - this.minimumPercentage;
            column.widthPercentage = this.minimumPercentage;
        } else {
            column.widthPercentage += percentageToAdd;
            this.hangoverPercentage = 0;
        }
    }
}
