import { AfterViewInit, Component, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import SignaturePad from 'signature_pad';

@Component({
  selector: 'app-amcs-signature-pad',
  templateUrl: './amcs-signature-pad.component.html',
  styleUrls: ['./amcs-signature-pad.component.scss']
})
export class AmcsSignaturePadComponent extends AutomationLocatorDirective implements AfterViewInit {
  @ViewChild('signaturePad', { static: true }) signaturePadElement: { nativeElement: any };

  signaturePad: any;
  img;
  @Input() width = 450;
  @Input() height = 300;
  @Input() penColor = '#000000';
  @Input() showPreview = true;
  @Input() showOutputFile = true;
  @Output('onSave') onSave: EventEmitter<string> = new EventEmitter<string>();

  ngAfterViewInit(): void {
    this.signaturePad = new SignaturePad(this.signaturePadElement.nativeElement);
    this.signaturePad.penColor = this.penColor;
  }

  clear() {
    this.signaturePad.clear();
    this.img = null;
  }

  undo() {
    const data = this.signaturePad.toData();
    if (data) {
      data.pop(); // remove the last dot or line
      this.signaturePad.fromData(data);
    }
  }

  savePNG() {
    if (!this.signaturePad.isEmpty()) {
      this.img = this.signaturePad.toDataURL('image/png');
      this.onSave.emit(this.img);
    }
  }

}
