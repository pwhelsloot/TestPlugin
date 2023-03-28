import { Subject } from 'rxjs';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class InnerTileServiceUI {

    // Usually only accessed by animations/framework code however
    // there is very rare/specific occasions a component must use this. For
    // example when changing a tiles col-lg or other classes dynamically (changing it's size
    // without the actual browser window resizing)
    innerTilesChanged = new Subject();

    private innerTileGroupsNumber = 0;

    getCurrentGroupNumber(): number {
        return this.innerTileGroupsNumber;
    }

    getNextGroupNumber(): number {
        if (this.innerTileGroupsNumber >= 20000) {
            this.innerTileGroupsNumber = 0;
        }
        this.innerTileGroupsNumber = this.innerTileGroupsNumber + 1;
        return this.innerTileGroupsNumber;
    }
}
