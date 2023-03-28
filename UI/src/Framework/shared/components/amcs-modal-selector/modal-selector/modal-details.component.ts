import { Component, ElementRef, OnInit, Renderer2 } from '@angular/core';
import { AmcsModalChildComponent } from '@shared-module/components/amcs-modal/amcs-modal-child-component.interface';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { BehaviorSubject, Subject } from 'rxjs';
import { ModalDetailsService } from './modal-details.service';

@Component({
  selector: 'app-modal-details',
  templateUrl: './modal-details.component.html',
  styleUrls: ['./modal-details.component.scss'],
  providers: [ModalDetailsService]
})
export class ModalDetailsComponent extends AutomationLocatorDirective implements OnInit, AmcsModalChildComponent {
  extraData: any;
  loading = new BehaviorSubject<boolean>(true);
  externalClose = new Subject<any>();

  constructor(protected elRef: ElementRef, protected renderer: Renderer2, readonly service: ModalDetailsService) {
    super(elRef, renderer);
  }

  ngOnInit() {
    this.service.initialise(this.extraData, this.loading, this.externalClose);
  }
}
