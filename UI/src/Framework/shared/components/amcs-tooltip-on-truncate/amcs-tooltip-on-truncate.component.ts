import { AfterViewInit, ChangeDetectorRef, Component, ElementRef, HostListener, Input, OnDestroy, OnInit, Renderer2, ViewChild } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { TooltipDirective } from 'ngx-bootstrap/tooltip';
import { ReplaySubject, Subject, Subscription } from 'rxjs';
import { throttleTime, withLatestFrom } from 'rxjs/operators';

@Component({
    selector: 'app-amcs-tooltip-on-truncate',
    templateUrl: './amcs-tooltip-on-truncate.component.html',
    styleUrls: ['./amcs-tooltip-on-truncate.component.scss']
})
export class AmcsTooltipOnTruncateComponent implements OnInit, OnDestroy, AfterViewInit {

    isTruncated = false;
    tooltipText: string;

    @Input() set text(text: string) {
        this.tooltipText = text;
        this.textSubject.next(text);
    }
    @Input() maxWidth = '100%';
    @Input() manualResize: Subject<void>;
    @Input('truncateText') truncateText = true;
    @Input() xOffset = 0;
    @Input() toolTipPlacement = 'auto';
    @Input('attachToBody') attachToBody = true;

    @Input() hideText = false;

    @Input() set isTooltipActive(val: boolean) {
        if (isTruthy(this.pop)) {
            if (val) {
                this.pop.show();
            } else {
                this.pop.hide();
            }
            this.cdr.detectChanges();
        }
    }

    @ViewChild('truncatableElement') truncatableElement: ElementRef;
    @ViewChild('pop') pop: TooltipDirective;
    containerbody: string;

    constructor(
        protected elRef: ElementRef, protected renderer: Renderer2,
        public cdr: ChangeDetectorRef) {
    }

    private widthRequiredSubject = new ReplaySubject<number>(1);
    private textSubject = new ReplaySubject<string>(1);
    private resizeSubject = new Subject();
    private textSubscription: Subscription;
    private widthSubscription: Subscription;
    private resizeSubscription: Subscription;
    private manualResizeSubscription: Subscription;

    ngOnInit() {
        if (this.attachToBody) {
            this.containerbody = 'body';
        }
        this.widthSubscription = this.widthRequiredSubject.subscribe((widthRequired: number) => {
            this.checkIfTooltipRequired(widthRequired);
        });
        this.resizeSubscription = this.resizeSubject.pipe(
            throttleTime(300, undefined, { leading: true, trailing: true }),
            withLatestFrom(this.widthRequiredSubject)
        ).subscribe(data => {
            const widthRequired: number = data[1];
            this.checkIfTooltipRequired(widthRequired);
        });
        if (isTruthy(this.manualResize)) {
            this.manualResizeSubscription = this.manualResize.pipe(
                throttleTime(300, undefined, { leading: true, trailing: true }),
                withLatestFrom(this.widthRequiredSubject)
            ).subscribe(data => {
                const widthRequired: number = data[1];
                this.checkIfTooltipRequired(widthRequired);
            });
        }
    }

    ngAfterViewInit() {
        this.textSubscription = this.textSubject.subscribe((text: string) => {
            setTimeout(() => {
                this.calculateWidthRequired(text);
            }, 0);
        });
    }

    ngOnDestroy() {
        this.textSubscription.unsubscribe();
        this.widthSubscription.unsubscribe();
        this.resizeSubscription.unsubscribe();
        if (isTruthy(this.manualResizeSubscription)) {
            this.manualResizeSubscription.unsubscribe();
        }
    }

    @HostListener('window:resize', ['$event'])
    onResize() {
        this.resizeSubject.next();
    }

    // Calculate the width required for the text and font
    private calculateWidthRequired(text: string) {
        if (!isTruthy(text)) {
            this.widthRequiredSubject.next(0);
        } else {
            // Bit of a hack but to decided how much width is needed we need
            // to create a absolutely positioned div and put the text in
            const div = this.renderer.createElement('div');
            this.renderer.setStyle(div, 'position', 'absolute');
            this.renderer.setStyle(div, 'left', '-99in');
            this.renderer.setStyle(div, 'whiteSpace', 'nowrap');
            this.renderer.setStyle(div, 'font', this.truncatableElement.nativeElement.style.font);
            const textElement = this.renderer.createText(text);
            this.renderer.appendChild(div, textElement);
            this.renderer.appendChild(this.truncatableElement.nativeElement, div);
            this.widthRequiredSubject.next(div.clientWidth);
            // Once we know the total width required we remove the div
            this.renderer.removeChild(this.truncatableElement.nativeElement, div);
            this.renderer.removeChild(div, textElement);
        }
    }

    // Check if we need to show a tooltip (does the element holding the text have enough width?)
    private checkIfTooltipRequired(widthRequired: number) {
        this.isTruncated = ((this.truncatableElement.nativeElement.offsetWidth - this.xOffset) < widthRequired);
    }
}
