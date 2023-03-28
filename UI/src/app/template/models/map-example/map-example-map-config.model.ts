import { TemplateTranslationsService } from '@app/template/services/template-translations.service';
import { MapExampleDrawService } from '@app/template/services/map-example/map-example-draw.service';
import { MapExampleListService } from '@app/template/services/map-example/map-example-list.service';
import { AmcsLeafletMapService } from '@core-module/services/amcs-leaflet-map.service';
import { AmcsModalService } from '@shared-module/components/amcs-modal/amcs-modal.service';

/* We use this to inject in the services for them to be avaliable on the map so the component attached
 to the map can use these services*/
export class MapExampleMapConfig {
  constructor(
    public mapExampleListService: MapExampleListService,
    public mapExampleDrawService: MapExampleDrawService,
    public translationService: TemplateTranslationsService,
    public amcsleafletMapService: AmcsLeafletMapService,
    public modalService: AmcsModalService
  ) {}
}
