import { Component, Input, TemplateRef } from '@angular/core';

@Component({
  selector: 'app-amcs-custom-tile',
  templateUrl: './amcs-custom-tile.component.html',
  styleUrls: ['./amcs-custom-tile.component.scss']
})
export class AmcsCustomTileComponent {
  @Input('isForm') isForm = false;
  @Input('loading') loading = false;
  @Input('isChild') isChild = false;
  @Input('bodyMinHeight') bodyMinHeight: number;
  @Input() bodyWidthpx: number = null;
  @Input('bodyBackgroundColor') bodyBackgroundColor: string;
  @Input('titleBackgroundColor') titleBackgroundColor: string;
  @Input('customClass') customClass: string;
  @Input('compName') compName: string = null;
  @Input() titleMenuTemplate: TemplateRef<any> = null;
}
