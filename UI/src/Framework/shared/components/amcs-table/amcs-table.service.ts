import { ElementRef, QueryList, Injectable } from '@angular/core';

@Injectable()
export class AmcsTableService {

    private isEdge = navigator.userAgent.indexOf('Edge') > -1;

    scrollIntoView(id: number, rowsRef: QueryList<ElementRef>) {
        if (id && rowsRef) {
            setTimeout(() => {
                const element: ElementRef = rowsRef.find(x => x.nativeElement.id === id.toString());
                if (element && !this.isElementInViewport(element)) {
                    if (this.isEdge) {
                        element.nativeElement.scrollIntoView();
                        window.scrollBy(0, -(element.nativeElement.offsetHeight - 10));
                    } else {
                        window.scrollTo({ top: element.nativeElement.offsetTop + 90, behavior: 'smooth' });
                    }
                }
            }, 300);
        }
    }

    scrollToTop(element: ElementRef) {
        setTimeout(() => {
            if (this.isEdge) {
                element.nativeElement.scrollIntoView();
                window.scrollBy(0, -(element.nativeElement.offsetHeight - 10));
            } else {
                window.scrollTo({ top: element.nativeElement.offsetTop - 50, behavior: 'smooth' });
            }
        }, 300);
    }

    private isElementInViewport(element: ElementRef): boolean {
        const rect = element.nativeElement.getBoundingClientRect();
        return (
            rect.top >= 0 &&
            rect.left >= 0 &&
            rect.bottom <= (window.innerHeight || document.documentElement.clientHeight) &&
            rect.right <= (window.innerWidth || document.documentElement.clientWidth)
        );
    }
}
