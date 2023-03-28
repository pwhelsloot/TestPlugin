import { AmcsLeafletMapService } from '@coreservices/amcs-leaflet-map.service';
import { ApplicationConfigurationStore } from '@coreservices/config/application-configuration.store';
import { AmcsMapDisplayProvider } from '@shared-module/amcs-leaflet-map/enums/amcs-map-display-provider';
import { Control, ControlOptions, DomEvent, DomUtil, Map } from 'leaflet';
import { take } from 'rxjs/operators';
import { AmcsMapProvider } from '../enums/amcs-map-provider.enum';

export class AmcsMapProviderControl extends Control {

    options: ControlOptions;

    constructor(private mappingService: AmcsLeafletMapService, private translations: string[], private appConfigService: ApplicationConfigurationStore) {
        super();
        this.options = { position: 'bottomright' };
        this.appConfigService.configuration$.subscribe(c => this.configAvailableProviders(c.mapProviderConfiguration.mapDisplayProvider));
    }

    private availableMapProviders: AmcsMapProvider[];
    private currentItems: HTMLElement[] = [];

    onAdd(map: Map): HTMLElement {
        const container = DomUtil.create('div', 'map-provider-button');

        this.availableMapProviders.forEach((provider, idx) => {
            const selectedOption = DomUtil.create('div', 'map-provider-option', container);

            DomUtil.addClass(selectedOption, idx === 0 ? 'current-map-provider' : 'hidden');

            selectedOption.setAttribute('style', `order: ${idx + 1}`);
            selectedOption.setAttribute('data-current-item', `${idx + 1}`);

            DomUtil.create('i', this.generateIconClass(provider), selectedOption);
            const selectedOptionText = DomUtil.create('span', null, selectedOption);
            selectedOptionText.innerText = this.generateIconText(provider);

            const clickEvt = () => {
                this.mappingService.selectedProvider$.pipe(take(1)).subscribe(data => {
                    if (provider !== data) {
                        this.mappingService.setMapProvider(provider);
                        const selectedItem = this.currentItems.find(x => x.getAttribute('data-current-item') === `${idx + 1}`);
                        selectedItem.setAttribute('style', 'order: 1');
                        selectedItem.className = 'map-provider-option current-map-provider';

                        this.currentItems.filter(x => x.getAttribute('data-current-item') !== `${idx + 1}`).forEach((currentElement, currentIndex) => {
                            currentElement.setAttribute('style', `order: ${currentIndex + 2}`);
                            currentElement.className = 'map-provider-option hidden';
                        });
                    }
                });
            };

            DomEvent.on(selectedOption, 'click', clickEvt);
            this.currentItems.push(selectedOption);
        });

        const mouseEnterEvt = () => {
            this.currentItems.forEach(selectedElement => {
                if (DomUtil.hasClass(selectedElement, 'hidden')) {
                    DomUtil.removeClass(selectedElement, 'hidden');
                }
            });
        };

        const mouseLeaveEvt = () => {
            this.currentItems.forEach(selectedElement => {
                if (!DomUtil.hasClass(selectedElement, 'current-map-provider')) {
                    DomUtil.addClass(selectedElement, 'hidden');
                }
            });
        };

        DomEvent.on(container, 'mouseenter', mouseEnterEvt);
        DomEvent.on(container, 'mouseleave', mouseLeaveEvt);

        return container;
    }

    private generateIconClass(currentProvider: AmcsMapProvider): string {
        switch (currentProvider) {
            case AmcsMapProvider.GoogleMapsRoadmap:
            case AmcsMapProvider.HereMapsStreet:
                return 'fas fa-map';
            case AmcsMapProvider.GoogleMapsSatellite:
            case AmcsMapProvider.HereMapsSatellite:
                return 'fas fa-satellite-dish';
            case AmcsMapProvider.GoogleMapsTerrain:
            case AmcsMapProvider.HereMapsTerrain:
                return 'fas fa-mountains';
            case AmcsMapProvider.Terrain:
            case AmcsMapProvider.Norway:
            case AmcsMapProvider.TopoGraphic:
            case AmcsMapProvider.TopoGraphicGrayScale:
            case AmcsMapProvider.Kartdata:
            case AmcsMapProvider.NorwayBaseMap:
            case AmcsMapProvider.NorwayBaseMapGreyScale:
            case AmcsMapProvider.European:
            case AmcsMapProvider.Seabed:
            case AmcsMapProvider.Nautical:
            case AmcsMapProvider.Toporaster:
            case AmcsMapProvider.Fjellskygge:
            case AmcsMapProvider.Basemap:
                return '';
            default:
                throw new Error('Invalid map provider');
        }
    }

    private generateIconText(currentProvider: AmcsMapProvider): string {
        switch (currentProvider) {
            case AmcsMapProvider.GoogleMapsRoadmap:
            case AmcsMapProvider.HereMapsStreet:
                return this.translations['map.map-providers.classic'];
            case AmcsMapProvider.GoogleMapsSatellite:
            case AmcsMapProvider.HereMapsSatellite:
                return this.translations['map.map-providers.satellite'];
            case AmcsMapProvider.GoogleMapsTerrain:
            case AmcsMapProvider.HereMapsTerrain:
            case AmcsMapProvider.Terrain:
                return this.translations['map.map-providers.terrain'];
            case AmcsMapProvider.Norway:
                return this.translations['map.map-providers.norway'];
            case AmcsMapProvider.TopoGraphic:
                return this.translations['map.map-providers.topoGraphic'];
            case AmcsMapProvider.TopoGraphicGrayScale:
                return this.translations['map.map-providers.topoGraphicGrayScale'];
            case AmcsMapProvider.Kartdata:
                return this.translations['map.map-providers.kartdata'];
            case AmcsMapProvider.NorwayBaseMap:
                return this.translations['map.map-providers.norwayBaseMap'];
            case AmcsMapProvider.NorwayBaseMapGreyScale:
                return this.translations['map.map-providers.norwayBaseMapGreyScale'];
            case AmcsMapProvider.European:
                return this.translations['map.map-providers.european'];
            case AmcsMapProvider.Seabed:
                return this.translations['map.map-providers.seabed'];
            case AmcsMapProvider.Nautical:
                return this.translations['map.map-providers.nautical'];
            case AmcsMapProvider.Toporaster:
                return this.translations['map.map-providers.toporaster'];
            case AmcsMapProvider.Fjellskygge:
                return this.translations['map.map-providers.fjellskygge'];
            case AmcsMapProvider.Basemap:
                return this.translations['map.map-providers.basemap'];
            default:
                throw new Error('Invalid map provider');
        }
    }

    private configAvailableProviders(mapDisplayProvider: string) {
        switch (mapDisplayProvider) {
            case AmcsMapDisplayProvider.Google:
                this.availableMapProviders = [AmcsMapProvider.GoogleMapsRoadmap, AmcsMapProvider.GoogleMapsTerrain, AmcsMapProvider.GoogleMapsSatellite];
                break;
            case AmcsMapDisplayProvider.Kartverket:
                this.availableMapProviders = [AmcsMapProvider.Norway,
                AmcsMapProvider.TopoGraphic,
                AmcsMapProvider.TopoGraphicGrayScale,
                AmcsMapProvider.Kartdata,
                AmcsMapProvider.NorwayBaseMap,
                AmcsMapProvider.NorwayBaseMapGreyScale,
                AmcsMapProvider.European,
                AmcsMapProvider.Seabed,
                AmcsMapProvider.Terrain,
                AmcsMapProvider.Nautical,
                AmcsMapProvider.Toporaster,
                AmcsMapProvider.Fjellskygge,
                AmcsMapProvider.Basemap];
                break;
            case AmcsMapDisplayProvider.Here:
                this.availableMapProviders = [AmcsMapProvider.HereMapsStreet, AmcsMapProvider.HereMapsTerrain, AmcsMapProvider.HereMapsSatellite];
                break;
        }
    }
}
