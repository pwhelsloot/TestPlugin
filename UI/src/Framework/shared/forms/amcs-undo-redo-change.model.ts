import { AmcsUndoRedoChangeStatus } from './amcs-undo-redo-change-status.enum';

/**
 * @deprecated To be deleted
 */
export class AmcsUndoRedoChange {
    status: AmcsUndoRedoChangeStatus = AmcsUndoRedoChangeStatus.Active;

    constructor(public key: string, public oldValue: any, public newValue: any) {
    }
}
