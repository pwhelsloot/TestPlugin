import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';
import { ImageCroppedEvent } from 'ngx-image-cropper';

/**
 * @todo Move to shared/components
 */
@Component({
  selector: 'app-image-manipulation',
  templateUrl: './image-manipulation.component.html',
  styleUrls: ['./image-manipulation.component.scss']
})
export class ImageManipulationComponent extends AutomationLocatorDirective implements OnInit, OnChanges {
  @Output('imageCroppedComplete') imageCroppedComplete = new EventEmitter<{ image: string }>();
  @Input() file: any;

  eventsSubscription: any;
  imageChangedEvent: any = '';
  croppedImage: any = '';
  resizedImage = '150';
  loadImage = '';
  hideCanvas = true;

  ngOnInit() {
    this.hideCanvas = false;
    if (!HTMLCanvasElement.prototype.toBlob) {
      Object.defineProperty(HTMLCanvasElement.prototype, 'toBlob', {
        value(callback, type, quality) {
          const dataURL = this.toDataURL(type, quality).split(',')[1];
          setTimeout(function() {
            const binStr = atob(dataURL);
            const len = binStr.length;
            const arr = new Uint8Array(len);
            for (let i = 0; i < len; i++) {
              arr[i] = binStr.charCodeAt(i);
            }
            callback(new Blob([arr], { type: type || 'image/png' }));
          });
        }
      });
    }
  }

  fileChangeEvent(event: any): void {
    this.imageChangedEvent = event;
  }

  imageCropped(event: ImageCroppedEvent) {
    this.croppedImage = event.base64;
    this.imageCroppedComplete.emit(this.croppedImage);
  }

  onImageResized(val: string) {
    this.hideCanvas = false;
    this.resizedImage = val;
  }

  loadIntoCropper() {
    this.loadImage = this.resizedImage;
    this.hideCanvas = true;
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes['file'] && changes.file.currentValue != null) {
      if (changes.file.currentValue.currentTarget != null) {
        this.fileChangeEvent(changes.file.currentValue);
      }
    }
  }
}
