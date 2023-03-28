import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { BytePicture } from '@coremodels/pictures/byte-picture.model';
import { AutomationLocatorDirective } from '@shared-module/directives/automation-locator.directive';

/**
 * @todo Refactor to use the app-amcs-carousel
 */
@Component({
  selector: 'app-amcs-picture-viewer',
  templateUrl: './amcs-picture-viewer.component.html',
  styleUrls: ['./amcs-picture-viewer.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class AmcsPictureViewerComponent extends AutomationLocatorDirective {

  @Input('pictures') pictures: BytePicture[];
  @Output('pictureChanged') pictureChanged: EventEmitter<number> = new EventEmitter<number>();
}
