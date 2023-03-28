import { Injectable } from '@angular/core';
import { FormGroup } from '@angular/forms';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { BaseService } from '@coreservices/base.service';
import { takeUntil } from 'rxjs/operators';
import { AmcsUndoRedoChangeStatus } from './amcs-undo-redo-change-status.enum';
import { AmcsUndoRedoChange } from './amcs-undo-redo-change.model';

/**
 * @deprecated To be deleted
 */
@Injectable()
export class AmcsUndoRedoService extends BaseService {
    private formGroup: FormGroup;
    private keys: string[];
    private currentValues: { [key: string]: any } = [];
    private changes: AmcsUndoRedoChange[] = [];
    private suppressEvents = false;

    init(formGroup: FormGroup, keys?: string[]) {
        this.formGroup = formGroup;
        this.keys = keys;

        if (!isTruthy(this.keys) || this.keys.length === 0) {
            this.keys = Object.keys(this.formGroup.controls);
        }

        this.keys.forEach((key: string) => {
            // Initialize current value
            this.currentValues[key] = this.formGroup.controls[key].value === undefined ? null : this.formGroup.controls[key].value;

            this.formGroup.get(key).valueChanges.pipe(takeUntil(this.unsubscribe)).subscribe((newValue: any) => {
                if (this.suppressEvents) {
                    return;
                }

                // Get previous value
                const oldValue = this.currentValues[key] === undefined ? null : this.currentValues[key];
                if (newValue === undefined) {
                    newValue = null;
                }

                if (oldValue !== newValue) {
                    // Remove undone changes and push new change
                    this.removeUndoneChanges();
                    this.changes.push(new AmcsUndoRedoChange(key, oldValue, newValue));
                    this.removeExcessChanges();

                    // Update current value
                    this.currentValues[key] = newValue;
                }
            });
        });
    }

    undo(discardChange?: boolean) {
        // Get last active change
        const idx = this.getNextUndoChangeIndex();
        if (idx < 0) {
            return;
        }

        try {
            this.suppressEvents = true;

            const change = this.changes[idx];
            this.formGroup.get(change.key).setValue(change.oldValue);
            this.currentValues[change.key] = change.oldValue;

            if (discardChange) {
                this.changes.splice(idx, 1);
            } else {
                this.changes[idx].status = AmcsUndoRedoChangeStatus.Undone;
            }
        } finally {
            this.suppressEvents = false;
        }
    }

    redo() {
        const idx = this.getNextRedoChangeIndex();
        if (idx < 0) {
            return;
        }

        try {
            this.suppressEvents = true;

            const change = this.changes[idx];
            this.formGroup.get(change.key).setValue(change.newValue);
            this.currentValues[change.key] = change.newValue;
            this.changes[idx].status = AmcsUndoRedoChangeStatus.Active;
        } finally {
            this.suppressEvents = false;
        }
    }

    private removeUndoneChanges() {
        // Remove all undone changes
        const idx = this.changes.findIndex(x => x.status === AmcsUndoRedoChangeStatus.Undone);
        if (idx >= 0) {
            this.changes.splice(idx, this.changes.length - idx);
        }
    }

    private removeExcessChanges() {
        // Only retain last 100 changes
        if (this.changes.length > 100) {
            this.changes.splice(0, this.changes.length - 100);
        }
    }

    private getNextUndoChangeIndex(): number {
        let nextIdx = -1;

        if (this.changes.length > 0) {
            // Get last active change
            const idx = this.changes.findIndex(x => x.status === AmcsUndoRedoChangeStatus.Undone);
            if (idx < 0) {
                nextIdx = this.changes.length - 1;
            } else {
                nextIdx = idx - 1;
            }
        }
        return nextIdx;
    }

    private getNextRedoChangeIndex(): number {
        return this.changes.findIndex(x => x.status === AmcsUndoRedoChangeStatus.Undone);
    }
}
