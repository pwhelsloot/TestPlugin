import { Observable, of } from 'rxjs';
import { SharedTranslationsService } from '../../shared/services/shared-translations.service';

export class MockSharedTranslationsService implements Partial<SharedTranslationsService> {
  translations: Observable<string[]>;

  constructor() {
    const translationArray = new Array<string>();
    translationArray['map.map-providers.satellite'] = 'Satellite';
    translationArray['map.map-providers.terrain'] = 'Terrain';
    translationArray['map.map-providers.classic'] = 'Classic';
    translationArray['map.drawmap.toolbar.cancel'] = 'cancel';
    translationArray['map.drawmap.polygon.continue'] = 'Continue or select first point to finish';
    translationArray['map.drawmap.polygon.createArea'] = 'New Area';
    translationArray['map.drawmap.polygon.intersecterror'] = 'Error: Area points cannot overlap';
    translationArray['map.drawmap.polygon.nextpoint'] = 'Select next point';
    translationArray['map.drawmap.polygon.start'] = 'Select a starting point';
    translationArray['map.drawmap.toolbar.cancel'] = 'Cancel';
    translationArray['map.drawmap.toolbar.save'] = 'Finish';
    translationArray['map.drawmap.toolbar.tooltips.cancel'] = 'Cancel Area';
    translationArray['map.drawmap.toolbar.tooltips.undo'] = 'Remove last point';
    translationArray['map.drawmap.toolbar.tooltips.save'] = 'Finish Area';
    translationArray['map.drawmap.toolbar.undo'] = 'undo';
    this.translations = of(translationArray);
  }
}
