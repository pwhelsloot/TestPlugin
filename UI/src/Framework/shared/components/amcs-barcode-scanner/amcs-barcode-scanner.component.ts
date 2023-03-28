import { Component, EventEmitter, Input, OnDestroy, OnInit, Output, ViewChild, ViewEncapsulation } from '@angular/core';
import { AmcsBeep } from '@core-module/helpers/amcs-beep';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ZXingScannerComponent } from '@zxing/ngx-scanner';

@Component({
  selector: 'app-amcs-barcode-scanner',
  templateUrl: './amcs-barcode-scanner.component.html',
  styleUrls: ['./amcs-barcode-scanner.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AmcsBarcodeScannerComponent extends AutomationLocatorDirective implements OnInit, OnDestroy {

  @Input() device: MediaDeviceInfo | null = null;
  @Input() formats: string[];
  @Input() tryHarder = false;
  @Input() playSound = false;
  @Input() debounceTime: number; // in milliseconds
  @Input() customClass: string;

  @Output('scanSuccess') scanSuccess: EventEmitter<string> = new EventEmitter<string>();
  @Output('scanFailure') scanFailure: EventEmitter<any> = new EventEmitter<any>();
  @Output('scanError') scanError: EventEmitter<Error> = new EventEmitter<Error>();
  @Output('scanComplete') scanComplete: EventEmitter<any> = new EventEmitter<any>();
  @Output('camerasFound') camerasFound: EventEmitter<MediaDeviceInfo[]> = new EventEmitter<MediaDeviceInfo[]>();
  @Output('camerasNotFound') camerasNotFound: EventEmitter<any> = new EventEmitter<any>();
  @Output('permissionResponse') permissionResponse: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output('hasDevices') hasDevices: EventEmitter<boolean> = new EventEmitter<boolean>();
  @Output('autoStarted') autoStarted: EventEmitter<void> = new EventEmitter<void>();
  @Output('autoStarting') autoStarting: EventEmitter<boolean> = new EventEmitter<boolean>();

  @ViewChild('scanner') scanner: ZXingScannerComponent;

  deviceFound = false;
  hasPermission = false;
  loading = true;
  debouncing = false;

  ngOnInit(): void {
    setTimeout(() => {
      // Consider loaded after 5 seconds
      this.loading = false;
    }, 5000);
  }

  ngOnDestroy(): void {
    if (isTruthy(this.scanner)) {
      this.scanner.scanStop();
      this.scanner.enable = false;
    }
  }

  onScanSuccess(e: string) {
    if (!this.debouncing) {
      this.debouncing = true;
      this.scanSuccess.emit(e);

      if (this.playSound) {
        AmcsBeep.beep();
      }

      setTimeout(() => {
        this.debouncing = false;
      }, (isTruthy(this.debounceTime) && this.debounceTime > 0) ? this.debounceTime : 0);
    }
  }

  onScanFailure(e: any) {
    this.scanFailure.emit(e);
  }

  onScanError(e: Error) {
    this.scanError.emit(e);
  }

  onScanComplete(e: any) {
    this.scanComplete.emit(e);
  }

  onCamerasFound(e: MediaDeviceInfo[]) {
    this.camerasFound.emit(e);
  }

  onCamerasNotFound(e: any) {
    this.camerasNotFound.emit(e);
  }

  onPermissionResponse(e: boolean) {
    this.hasPermission = e;
    this.loading = false;
    this.permissionResponse.emit(e);
  }

  onHasDevices(e: boolean) {
    this.deviceFound = e;
    this.hasDevices.emit(e);
  }

  onAutoStarted() {
    this.autoStarted.emit();
  }

  onAutoStarting(e: boolean) {
    this.autoStarting.emit(e);
  }
}
