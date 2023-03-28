import { BaseService } from '@coreservices/base.service';
import { ElementRef } from '@angular/core';
import { Subject } from 'rxjs';
import { takeUntil, throttleTime } from 'rxjs/operators';
import { isTruthy } from '@core-module/helpers/is-truthy.function';

export class AmcsInnerTileHelper extends BaseService {

    constructor(private el: ElementRef, private groupId: number) {
        super();
        this.setPaddingRequest.pipe(
            throttleTime(50, undefined, { leading: true, trailing: true }),
            takeUntil(this.unsubscribe))
            .subscribe(() => {
                this.doSetPadding();
            });
    }

    private setPaddingRequest = new Subject();

    setPadding() {
        this.setPaddingRequest.next();
    }

    private doSetPadding() {
        const allChildPortlets = this.el.nativeElement.querySelectorAll('.amcs-inner-container.container-group-' + this.groupId.toString());
        if (isTruthy(allChildPortlets) && allChildPortlets.length > 0) {
            if (isTruthy(allChildPortlets) && allChildPortlets.length > 0) {
                // Need to wipe our horizontal / vertical classes
                this.removeClasses(allChildPortlets);
                // Populate positions
                const domPositions = new Array<DOMRect>();
                for (let index = 0; index < allChildPortlets.length; index++) {
                    const element = allChildPortlets[index];
                    domPositions[index] = element.getBoundingClientRect();
                }

                // Apply last-horizontal to the furthest right elements
                const furthestRight: number = domPositions.reduce((pos, element) => Math.max(pos, element.right), 0);
                for (let index = 0; index < domPositions.length; index++) {
                    const element = allChildPortlets[index];
                    if (Math.round(domPositions[index].right) >= Math.round(furthestRight)) {
                        element.className = element.className + ' last-horizontal';
                    }
                }

                // Apply last-vertical to the furthest bottom elements
                const furthestBottom: number = domPositions.reduce((pos, element) => Math.max(pos, element.bottom), 0);
                for (let index = 0; index < domPositions.length; index++) {
                    const element = allChildPortlets[index];
                    if (Math.round(domPositions[index].bottom) >= Math.round(furthestBottom)) {
                        element.className = element.className + ' last-vertical';
                    }
                }
            }
        }
    }

    private removeClasses(allChildPortlets: any[]) {
        allChildPortlets.forEach(element => {
            element.className = element.className.replace(new RegExp('last-vertical', 'g'), '');
            element.className = element.className.replace(new RegExp('last-horizontal', 'g'), '');
        });
    }
}
