import { Directive, OnInit, Input, HostBinding } from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';

@Directive({
  selector: '[appImageViewer]'
})
export class ImageViewerDirective implements OnInit {
  imageData: any;
  @HostBinding('src') sanitizedImageData: any;
  @Input() appImageViewer: string;

  constructor(private sanitizer: DomSanitizer) { }

  ngOnInit() {
    this.imageData = this.appImageViewer;
    this.sanitizedImageData = this.sanitizer.bypassSecurityTrustUrl(this.imageData);
  }
}
