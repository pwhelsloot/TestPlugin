import { Component } from '@angular/core';
import { AmcsModalChildComponent } from '@shared-module/components/amcs-modal/amcs-modal-child-component.interface';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-error-modal',
  templateUrl: './error-modal.component.html',
  styleUrls: ['./error-modal.component.scss']
})
export class ErrorModalComponent implements AmcsModalChildComponent {
  loading = new BehaviorSubject(true);
  extraData: any;
}
