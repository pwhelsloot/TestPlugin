import { Filter } from '@coremodels/filter/Filter';
import { FilterConfig } from '@coremodels/filter/FilterConfig';
import { FilterDefinition } from '@coremodels/filter/FilterDefinition';
import { FilterOption } from '@coremodels/filter/FilterOption';
import { AmcsFilterService } from '@coreservices/amcs-filter.service';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { BehaviorSubject } from 'rxjs';

describe('AmcsFilterService', () => {
    let service: AmcsFilterService;
    let filterDefinition1, filterDefinition2, filterDefinition3: FilterDefinition;

    beforeEach(() => {
        const option1 = new FilterOption(1, 'skip', false, 1);
        const option2 = new FilterOption(2, 'wheelie bin', true, 1);
        filterDefinition1 = new FilterDefinition(1, 'Services', [option1, option2], 'wheel', true);

        const option3 = new FilterOption(3, 'job', true, 2);
        const option4 = new FilterOption(4, 'route visit', true, 2);
        filterDefinition2 = new FilterDefinition(2, 'Activity Types', [option3, option4]);

        const option5 = new FilterOption(5, 'Aberdeen', true, 3);
        const option6 = new FilterOption(6, 'Aberfeldy', true, 3);
        const option7 = new FilterOption(7, 'Aberfoyle', true, 3);
        const option8 = new FilterOption(8, 'Inverness', true, 3);
        const option9 = new FilterOption(9, 'Edinburgh', true, 3);
        filterDefinition3 = new FilterDefinition(3, 'Sites', [option5, option6, option7, option8, option9]);

        // need to set these values so that equals comparisons between config and what is published are correct.
        option1.matchesPattern = false; // skip doesn't match 'wheel'
        option2.matchesPattern = true; // wheelie bin matches 'wheel'
        option3.matchesPattern = true; // all others have no pattern so they match.
        option4.matchesPattern = true;
        option5.matchesPattern = true;
        option6.matchesPattern = true;
        option7.matchesPattern = true;
        option8.matchesPattern = true;
        option9.matchesPattern = true;

        const translationService = {} as SharedTranslationsService;
        translationService.translations = new BehaviorSubject<string[]>([]);
        service = new AmcsFilterService(translationService);
    });

    it('should publish correct filter definitions once configured', (done) => {
        const config = new FilterConfig([filterDefinition1, filterDefinition2], 1);
        service.configure(config);

        // expect these filter defs to have the same no of options as the config.
        filterDefinition1.numberOfOptionsToDisplay = 1;
        filterDefinition2.numberOfOptionsToDisplay = 1;

        service.filterDefinitions$.subscribe((filterDefinitions: FilterDefinition[]) => {
            expect(filterDefinitions.length).toBe(2);
            expect(filterDefinitions[0].equals(filterDefinition1)).toBeTruthy();
            expect(filterDefinitions[1].equals(filterDefinition2)).toBeTruthy();

            expect(filterDefinitions[0].numberOfOptionsToDisplay).toBe(1);
            expect(filterDefinitions[0].getOptionsToDisplay().length).toBe(1);
            expect(filterDefinitions[0].getNumberOfOptionsNotDisplayed()).toBe(0);

            // where the number of options is 1 greater than the number of options to display,
            // it should display all the options rather than displaying a "1 more" link.
            // eg. if number of options to display is 1:
            // 1 option -> 1 option displayed
            // 2 options -> 2 options displayed
            // 3 options -> 1 option displayed and "2 more" link
            // so in this case, it should display 2 options.
            expect(filterDefinitions[1].numberOfOptionsToDisplay).toBe(1);
            expect(filterDefinitions[1].getOptionsToDisplay().length).toBe(2);
            expect(filterDefinitions[1].getNumberOfOptionsNotDisplayed()).toBe(0);

            done();
        });
    });

    it('should sort the filter options during configuration', (done) => {
        const config = new FilterConfig([filterDefinition3], 4);
        service.configure(config);

        filterDefinition3.numberOfOptionsToDisplay = 4;

        service.filterDefinitions$.subscribe((filterDefs: FilterDefinition[]) => {
            expect(filterDefs.length).toBe(1);
            const options = filterDefs[0].filterOptions;

            expect(options[0].description).toBe('Aberdeen');
            expect(options[1].description).toBe('Aberfeldy');
            expect(options[2].description).toBe('Aberfoyle');
            expect(options[3].description).toBe('Edinburgh');
            expect(options[4].description).toBe('Inverness');
            done();
        });
    });

    it('should publish the flat list of selected filter options once configured', (done) => {
        /*
      in addition to the UI state published as service.filterDefinitions$ there should also be a stream of
        all selected filter options across all filter definitions in a flat list.
    */

        const config = new FilterConfig([filterDefinition1, filterDefinition2]);
        service.configure(config);

        // option 1 was not selected, so this stream should have the 3 selected ones - options 2, 3 and 4.

        service.selectedFilterOptions$.subscribe((options: FilterOption[]) => {
            expect(options.length).toBe(3);
            expect(options.find((x) => x.id === 2)).toBeTruthy();
            expect(options.find((x) => x.id === 3)).toBeTruthy();
            expect(options.find((x) => x.id === 4)).toBeTruthy();
            done();
        });
    });

    it('should publish the initial filter once configured', (done) => {
        const config = new FilterConfig([filterDefinition1, filterDefinition2]);
        service.configure(config);

        // one of the options in the config was not selected, so apart from that, the filter should include all options from the config.

        service.filter$.subscribe((filter: Filter) => {
            expect(filter.filterDefinitions.length).toBe(2);
            expect(filter.filterDefinitions[0].filterOptions.length).toBe(1);
            expect(filter.filterDefinitions[0].filterOptions[0]).toBe(2);

            expect(filter.filterDefinitions[1].filterOptions.length).toBe(2);
            expect(filter.filterDefinitions[1].filterOptions[0]).toBe(3);
            expect(filter.filterDefinitions[1].filterOptions[1]).toBe(4);
            done();
        });
    });

    it('should publish correct state when a filter definition is expanded / contracted', (done) => {
        const config = new FilterConfig([filterDefinition1]);
        service.configure(config);

        // initial state: expanded.

        let filterDefIndex = 0;

        service.filterDefinitions$.subscribe((filterDefs: FilterDefinition[]) => {
            filterDefIndex++;

            if (filterDefIndex === 1) {
                expect(filterDefs[0].expanded).toBeTruthy();
            }

            if (filterDefIndex === 2) {
                expect(filterDefs[0].expanded).toBeFalsy();
            }

            if (filterDefIndex === 3) {
                expect(filterDefs[0].expanded).toBeTruthy();
                done();
            }
        });

        service.expand(1); // contract
        service.expand(1); // expand
    });

    it('should filter the filter options correctly', (done) => {
        const config = new FilterConfig([filterDefinition3], 2);
        service.configure(config);

        let filterDefIndex = 0;
        service.filterDefinitions$.subscribe((filterDefs: FilterDefinition[]) => {
            filterDefIndex++;

            if (filterDefIndex === 2) {
                expect(filterDefs.length).toBe(1);
                expect(filterDefs[0].filterOptions.length).toBe(5);
                const matches = filterDefs[0].filterOptions.filter((x) => x.matchesPattern);
                expect(matches.length).toBe(3);
                expect(matches.find((x) => x.description === 'Aberdeen')).toBeTruthy();
                expect(matches.find((x) => x.description === 'Aberfeldy')).toBeTruthy();
                expect(matches.find((x) => x.description === 'Aberfoyle')).toBeTruthy();

                // where the number of matches is number-to-display + 1, should display all the options
                expect(filterDefs[0].numberOfOptionsToDisplay).toBe(2);
                expect(filterDefs[0].getOptionsToDisplay().length).toBe(3);
                expect(filterDefs[0].getNumberOfOptionsNotDisplayed()).toBe(0);
            }

            if (filterDefIndex === 3) {
                expect(filterDefs.length).toBe(1);
                expect(filterDefs[0].filterOptions.length).toBe(5);
                const matches = filterDefs[0].filterOptions.filter((x) => x.matchesPattern);
                expect(matches.length).toBe(5);

                // in this case, number of matches is greater than number-to-display + 1 so should respect the
                // number-of-options-to-display setting.
                expect(filterDefs[0].numberOfOptionsToDisplay).toBe(2);
                expect(filterDefs[0].getOptionsToDisplay().length).toBe(2);
                expect(filterDefs[0].getNumberOfOptionsNotDisplayed()).toBe(3);
                done();
            }
        });

        service.filterFilterOptions(filterDefinition3.id, 'Aber');
        service.filterFilterOptions(filterDefinition3.id, '');
    });

    it('should publish correct filter definitions on select / unselect', (done) => {
        const config = new FilterConfig([filterDefinition1, filterDefinition2]);
        service.configure(config);

        let filterIndex = 0;

        service.filterDefinitions$.subscribe((filterDefs: FilterDefinition[]) => {
            filterIndex++;

            if (filterIndex === 2) {
                const option1 = filterDefs[0].filterOptions.find((x) => x.id === 1);
                expect(option1.selected).toBeFalsy();
            }

            if (filterIndex === 3) {
                const option1 = filterDefs[0].filterOptions.find((x) => x.id === 1);
                expect(option1.selected).toBeTruthy();
                done();
            }
        });

        service.selectFilterOption(false, filterDefinition1.id, 1);
        service.selectFilterOption(true, filterDefinition1.id, 1);
    });

    // Will be tested and altered later
    // it('should publish correct filter on select / unselect', (done) => {
    //   const config = new FilterConfig([filterDefinition1, filterDefinition2]);
    //   service.configure(config);

    //   let filterIndex = 0;

    //   service.filter$.subscribe((filter: Filter) => {
    //     filterIndex++;

    //     if (filterIndex === 2) {
    //       expect(filter.filterDefinitions.length).toBe(2);
    //       const filterDef = filter.filterDefinitions.find(x => x.filterDefinitionId === filterDefinition1.id);
    //       expect(filterDef.filterOptions.length).toBe(2);
    //       expect(filterDef.filterOptions.find(x => x === 1)).toBeTruthy();
    //       expect(filterDef.filterOptions.find(x => x === 2)).toBeTruthy();
    //       done();
    //     }

    //     if (filterIndex === 3) {
    //       expect(filter.filterDefinitions.length).toBe(2);
    //       const filterDef = filter.filterDefinitions.find(x => x.filterDefinitionId === filterDefinition1.id);
    //       expect(filterDef.filterOptions.length).toBe(1);
    //       expect(filterDef.filterOptions[0]).toBe(2);
    //     }
    //   });

    //   service.selectFilterOption(true, filterDefinition1.id, 1);
    //   service.selectFilterOption(false, filterDefinition1.id, 1);
    // });

    it('should switch between multi filter and single filter modes correctly', () => {
        const config = new FilterConfig([filterDefinition1, filterDefinition2], 4);
        service.configure(config);

        filterDefinition1.numberOfOptionsToDisplay = 4;
        filterDefinition2.numberOfOptionsToDisplay = 4;

        service.switchToSingleFilterMode(2);
        expect(service.singleFilterModeFilterDefinition.id === filterDefinition2.id).toBeTruthy();

        service.switchToMultiFilterMode();
        expect(service.singleFilterModeFilterDefinition).toBeNull();
    });

    it('should be possible to get the list of distinct start characters from a filter definition', () => {
        const config = new FilterConfig([filterDefinition3], 4);
        service.configure(config);

        service.switchToSingleFilterMode(3);

        service.singleFilterModeFilterDefinition.setDistinctFilterOptionStartCharacters();

        expect(service.singleFilterModeFilterDefinition.startChars.length).toBe(3);
        expect(service.singleFilterModeFilterDefinition.startChars.find((x) => x === 'A')).toBeTruthy();
        expect(service.singleFilterModeFilterDefinition.startChars.find((x) => x === 'E')).toBeTruthy();
        expect(service.singleFilterModeFilterDefinition.startChars.find((x) => x === 'I')).toBeTruthy();
    });

    it('should be possible to get the list of filter options grouped by start character', () => {
        const config = new FilterConfig([filterDefinition3], 2);
        service.configure(config);

        service.switchToSingleFilterMode(3);
        service.filterFilterOptions(3, 'b'); // matches all options except for Inverness

        // the "grouping" is done by setting a flag called "displayStartCharacter" on the first option starting with a particular character.
        // as description and zero for the option value.

        service.singleFilterModeFilterDefinition.setAllOptionsMatchingPatternGroupedByFirstCharacter();
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions.length).toBe(4);
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions[0].displayInitialCharacter).toBeTruthy();
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions[0].description).toBe('Aberdeen');
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions[1].displayInitialCharacter).toBeFalsy();
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions[1].description).toBe('Aberfeldy');
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions[2].displayInitialCharacter).toBeFalsy();
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions[2].description).toBe('Aberfoyle');
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions[3].displayInitialCharacter).toBeTruthy();
        expect(service.singleFilterModeFilterDefinition.groupedCharOptions[3].description).toBe('Edinburgh');
    });

    // Will be tested and altered later
    // it('should clear published filters correctly with clearAllFilters', (done) => {
    //   const config = new FilterConfig([filterDefinition1, filterDefinition2], 2);
    //   service.configure(config);

    //   let filterIndex = 0;
    //   service.filter$.subscribe((filter: Filter) => {
    //     filterIndex++;

    //     if (filterIndex === 2) {
    //       expect(filter.filterDefinitions.length).toBe(2);
    //       const filterDef1 = filter.filterDefinitions.find(x => x.filterDefinitionId === filterDefinition1.id);
    //       expect(filterDef1.filterOptions.length).toBe(0);
    //       const filterDef2 = filter.filterDefinitions.find(x => x.filterDefinitionId === filterDefinition2.id);
    //       expect(filterDef2.filterOptions.length).toBe(0);
    //       done();
    //     }
    //   });

    //   service.clearAllFilters();
    // });

    it('should clear ui state correctly with clearAllFilters', (done) => {
        const config = new FilterConfig([filterDefinition1, filterDefinition2], 2);
        service.configure(config);

        let filterIndex = 0;
        service.filterDefinitions$.subscribe((filterDefs: FilterDefinition[]) => {
            filterIndex++;

            if (filterIndex === 2) {
                expect(filterDefs.length).toBe(2);
                const filterDef1 = filterDefs.find((x) => x.id === filterDefinition1.id);
                const filterDef1SelectedOptions = filterDef1.filterOptions.filter((x) => x.selected);
                expect(filterDef1SelectedOptions.length).toBe(0);
                expect(filterDef1.pattern).toBe('');

                const filterDef2 = filterDefs.find((x) => x.id === filterDefinition2.id);
                const filterDef2SelectedOptions = filterDef2.filterOptions.filter((x) => x.selected);
                expect(filterDef2SelectedOptions.length).toBe(0);
                expect(filterDef2.pattern).toBe('');
                done();
            }
        });

        service.clearAllFilters();
    });
});
