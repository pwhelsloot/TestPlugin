import { Injectable, TemplateRef } from '@angular/core';
import { isTruthy } from '@core-module/helpers/is-truthy.function';
import { GridColumnAdvancedConfigSave } from '@core-module/models/column-customiser/grid-column-advanced-config-save.model';
import { GridColumnAdvancedConfig } from '@core-module/models/column-customiser/grid-column-advanced-config.model';
import { GridColumnAdvancedTypeEnum } from '@core-module/models/column-customiser/grid-column-advanced-type.enum';
import { CoreUserPreferenceKeys } from '@core-module/models/preferences/core-user-preference-keys.model';
import { MultiTileSelector } from '@core-module/models/selector/multi-tile-selector.model';
import { CoreUserPreferencesService } from '@core-module/services/preferences/core-user-preferences.service';
import { environment } from '@environments/environment';
import { Observable, of, ReplaySubject } from 'rxjs';
import { combineLatest, debounceTime, take } from 'rxjs/operators';
import { MultiListSelectorAdapter } from '../selector/multi-list-selector.adapter';
import { CoreTranslationsService } from '../translation/core-translation.service';
import { ColumnCustomiserAdapter } from './column-customiser.adapter';

@Injectable()
export class ColumnCustomiserService extends MultiListSelectorAdapter {
    static providers = [
        ColumnCustomiserService,
        {
            provide: MultiListSelectorAdapter,
            useExisting: ColumnCustomiserService
        }
    ];

    moveLeftText: string;
    moveRightText: string;
    tiles: MultiTileSelector[] = null;
    allColumns: GridColumnAdvancedConfig[];
    enabledAccordianColumns: GridColumnAdvancedConfig[];
    disabledAccordianColumns: GridColumnAdvancedConfig[];
    initalised = new ReplaySubject<boolean>(1);
    disabledColumnClass: string;
    enabledColumnClass: string;
    truncateText: boolean;

    constructor(
        private userPreferencesService: CoreUserPreferencesService,
        private translationsService: CoreTranslationsService,
        private adapter: ColumnCustomiserAdapter
    ) {
        super();
    }

    private gridColumnPreferences: { [key: string]: string };

    initialise(accordianTemplate: TemplateRef<any>) {
        this.userPreferencesService
            .get<{ [key: string]: string }>(CoreUserPreferenceKeys.gridColumnPreferences, {})
            .pipe(
                debounceTime(0), // Needed as initalised is called afterviewinit
                combineLatest(this.translationsService.translations),
                take(1)
            )
            .subscribe((data) => {
                this.adapter.accordianTemplate = accordianTemplate;

                this.gridColumnPreferences = data[0];
                const translations: string[] = data[1];

                this.moveLeftText = translations['columnCustomiser.left'];
                this.moveRightText = translations['columnCustomiser.right'];

                this.disabledColumnClass = this.adapter.disabledColumnClass;
                this.enabledColumnClass = this.adapter.enabledColumnClass;
                this.truncateText = this.adapter.truncateText;

                this.adapter
                    .getDefaultColumns$()
                    .pipe(take(1))
                    .subscribe((columns: GridColumnAdvancedConfig[]) => {
                        this.allColumns = columns;
                        // If columns have already been saved then replace them from defaults.
                        if (isTruthy(this.gridColumnPreferences[this.getGridKey()])) {
                            const savedColumns: GridColumnAdvancedConfigSave[] = new GridColumnAdvancedConfigSave().parseArray(
                                this.gridColumnPreferences[this.getGridKey()],
                                GridColumnAdvancedConfigSave
                            );
                            savedColumns.forEach((element) => {
                                const indexOfColumn: number = this.allColumns.map((x) => x.key).indexOf(element.key);
                                if (indexOfColumn >= 0) {
                                    this.allColumns[indexOfColumn].advancedType = element.advancedType;
                                    this.allColumns[indexOfColumn].position = element.position;
                                }
                            });
                        }
                        this.setColumnProperties();
                        this.buildTiles();
                        this.initalised.next(true);
                    });
            });
    }

    move(ids: number[], toTileId: GridColumnAdvancedTypeEnum): Observable<boolean> {
        // Switches type to grid/accordian/unused
        this.allColumns
            .filter((x) => ids.some((y) => y === x.id))
            .forEach((element) => {
                element.advancedType = toTileId as GridColumnAdvancedTypeEnum;
            });
        return of(true);
    }

    afterSuccessfulMove(toTile: MultiTileSelector, items: GridColumnAdvancedConfig[]) {
        this.orderColumns();
        const gridKey: string = this.getGridKey();
        const value: string = new GridColumnAdvancedConfigSave().postArray(
            this.allColumns.map((x) => x.toSaveModel()),
            GridColumnAdvancedConfigSave
        );
        this.gridColumnPreferences[gridKey] = value;
        this.userPreferencesService.save<{ [key: string]: string }>(
            CoreUserPreferenceKeys.gridColumnPreferences,
            this.gridColumnPreferences
        );
    }

    buildTiles() {
        // Intentionally not using getTranslation as this method may be called before translations load
        this.translationsService.translations.pipe(take(1))
        .subscribe((translations: string[]) =>
        {
            // We build these each time the pop-over opens incase they changed orderings manually on grid
            const gridTile = new MultiTileSelector();
            gridTile.id = GridColumnAdvancedTypeEnum.Grid;
            gridTile.title = translations['columnCustomiser.gridTile.title'];
            gridTile.items = this.allColumns
                .filter((x) => x.advancedType === GridColumnAdvancedTypeEnum.Grid)
                .sort((a, b) => {
                    return a.position - b.position;
                });

            const accordianTile = new MultiTileSelector();
            accordianTile.id = GridColumnAdvancedTypeEnum.Accordian;
            accordianTile.title = translations['columnCustomiser.accordianTile.title'];
            accordianTile.items = this.allColumns
                .filter((x) => x.advancedType === GridColumnAdvancedTypeEnum.Accordian)
                .sort((a, b) => {
                    return a.position - b.position;
                });

            const unusedTile = new MultiTileSelector();
            unusedTile.id = GridColumnAdvancedTypeEnum.Unused;
            unusedTile.title = translations['columnCustomiser.unusedTile.title'];
            unusedTile.items = this.allColumns
                .filter((x) => x.advancedType === GridColumnAdvancedTypeEnum.Unused)
                .sort((a, b) => {
                    return a.position - b.position;
                });

            this.tiles = [gridTile, accordianTile, unusedTile];

            this.initalised.next(true);
        });
    }

    resetColumns() {
        this.adapter
            .getDefaultColumns$()
            .pipe(take(1))
            .subscribe((columns: GridColumnAdvancedConfig[]) => {
                this.allColumns = columns;
                this.setColumnProperties();
                this.buildTiles();
                this.gridColumnPreferences[this.getGridKey()] = new GridColumnAdvancedConfigSave().postArray(
                    this.allColumns.map((x) => x.toSaveModel()),
                    GridColumnAdvancedConfigSave
                );
                this.userPreferencesService.save<{ [key: string]: string }>(
                    CoreUserPreferenceKeys.gridColumnPreferences,
                    this.gridColumnPreferences
                );
            });
    }

    orderColumns(): void {
        // Sets column positions/widths, this will reflect on the grid
        const gridColumns: GridColumnAdvancedConfig[] = this.tiles[0].disabledItems?.concat(
            this.tiles[0].enabledItems
        ) as GridColumnAdvancedConfig[];
        let position = 1;
        const widthPerSingleColumn = this.calculateWidthPerSingleColumn(gridColumns);
        gridColumns.forEach((element) => {
            element.position = position;
            element.widthPercentage = element.size * widthPerSingleColumn;
            position++;
        });

        this.disabledAccordianColumns = this.tiles[1].disabledItems as GridColumnAdvancedConfig[];
        this.disabledAccordianColumns.forEach((element) => {
            element.position = position;
            position++;
        });

        this.enabledAccordianColumns = this.tiles[1].enabledItems as GridColumnAdvancedConfig[];
        this.enabledAccordianColumns.forEach((element) => {
            element.position = position;
            position++;
        });

        const unusedColumns = this.tiles[2].disabledItems.concat(this.tiles[2].enabledItems) as GridColumnAdvancedConfig[];
        unusedColumns.forEach((element) => {
            element.position = position;
            position++;
        });

        // Refresh columns
        this.adapter.gridColumns = [...gridColumns];
        this.adapter.requestRefreshColumnSubject.next();
    }

    private getGridKey(): string {
        return `${environment.applicationURLPrefix}-${this.adapter.gridKey}`;
    }

    private setColumnProperties(): void {
        // Assign id to each column, this is for the multi-tile-selector
        let id = 1;
        this.allColumns.forEach((element) => {
            element.id = id;
            element.filter.id = id;
            id++;
        });
        // For each grid column, set a width based on how many grid columns appear.
        const gridColumns: GridColumnAdvancedConfig[] = this.allColumns
            .sort((a, b) => {
                return a.canMove === b.canMove ? 0 : b.canMove ? -1 : 1;
            })
            .filter((x) => x.advancedType === GridColumnAdvancedTypeEnum.Grid);

        const widthPerSingleColumn = this.calculateWidthPerSingleColumn(gridColumns);
        gridColumns.forEach((element) => {
            element.widthPercentage = element.size * widthPerSingleColumn;
        });

        // Refresh columns
        this.adapter.gridColumns = [...gridColumns];
        this.adapter.requestRefreshColumnSubject.next();

        // Set accordian columns so accordian shows
        this.enabledAccordianColumns = this.allColumns
            .filter((x) => x.advancedType === GridColumnAdvancedTypeEnum.Accordian && x.canMove)
            .sort((a, b) => {
                return a.position - b.position;
            });
        this.disabledAccordianColumns = this.allColumns.filter(
            (x) => x.advancedType === GridColumnAdvancedTypeEnum.Accordian && !x.canMove
        );

        // Set-up filter properties
        this.adapter.gridFilterColumns = this.allColumns.map((x) => x.filter)
            .filter(column => !this.adapter.excludeFilterColumns?.some(key => key === column.propertyKey));
    }

    /**
     * Calculates the width % of a single size column.
     * @param gridColumns The current columns on the grid
     * @returns The width % of a GridColumnAdvancedSizeEnum.Single column
     */
    private calculateWidthPerSingleColumn(gridColumns: GridColumnAdvancedConfig[]): number {
        const totalPercentage: number = 96 - this.adapter.totalPercentageOffset;
        const gridColumnsSize: number = gridColumns.reduce((sum, item) => sum + item.size, 0);
        let widthPerSingleColumn: number;
        if (gridColumnsSize <= this.adapter.numberOfColumnsBeforeScroll) {
            widthPerSingleColumn = totalPercentage / gridColumnsSize;
        } else {
            widthPerSingleColumn = totalPercentage / this.adapter.numberOfColumnsBeforeScroll;
        }
        return widthPerSingleColumn;
    }
}
