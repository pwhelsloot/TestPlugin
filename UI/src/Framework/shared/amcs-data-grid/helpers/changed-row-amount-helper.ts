export class ChangedRowAmountHelper {
  private rowDataTracker = {};

  getChangeAmount(): number {
    return Object.keys(this.rowDataTracker).length;
  }

  storeEdit(rowId: string, columnId: string) {
    if (!this.rowDataTracker[rowId]) {
      this.rowDataTracker[rowId] = {};
    }
    this.rowDataTracker[rowId][columnId] = true;
  }

  reset() {
    this.rowDataTracker = {};
  }
}
