import { AfterContentInit, Component, ElementRef, EventEmitter, Input, OnInit, Output, Renderer2 } from '@angular/core';
import { AmcsFilterService } from '@coreservices/amcs-filter.service';
import { AmcsFilterComponent } from '@shared-module/components/amcs-filter/amcs-filter.component';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';

/**
 * @deprecated Marked for removal, use any of the other amcs-selects instead
 */
@Component({
  selector: 'app-amcs-filter-single-select',
  templateUrl: './amcs-filter-single-select.component.html',
  styleUrls: ['./amcs-filter-single-select.component.scss']
})
export class AmcsFilterSingleSelectComponent extends AmcsFilterComponent implements OnInit, AfterContentInit {

  @Input('filterDefinitionId') filterDefinitionId: number;
  @Output('optionSelected') optionSelected: EventEmitter<number> = new EventEmitter<number>();
  @Output('loaded') loaded: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor(
    protected elRef: ElementRef, protected renderer: Renderer2,
    public filterService: AmcsFilterService,
    public translationService: SharedTranslationsService) {
    super(filterService, elRef, renderer, translationService);
  }

  ngOnInit() {
    this.open = true;
    this.filterService.switchToSingleFilterMode(this.filterDefinitionId);
  }

  ngAfterContentInit() {
    this.loaded.next(true);
  }
}
