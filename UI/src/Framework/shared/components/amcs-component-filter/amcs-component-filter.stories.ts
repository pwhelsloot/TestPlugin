import { AmcsDate } from '@core-module/models/date/amcs-date.model';
import { ComponentFilterStorageService } from '@shared-module/services/amcs-component-filter/component-filter-storage.service';
import { SharedTranslationsService } from '@shared-module/services/shared-translations.service';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { Meta, Story } from '@storybook/angular';
import { BrowserOptions } from '../layouts/amcs-browser-grid-layout/browser-options-model';
import { AmcsComponentFilterComponent } from './amcs-component-filter.component';
import { ComponentFilterSB } from './component-filter-sb.model';

export default {
  title: StorybookGroupTitles.Data + 'Component Filter',
  component: AmcsComponentFilterComponent,
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am default Amcs component filter, I filter provided data based on selected filters. Filtering can be done locally or via API.'
      },
      source: {
        type: 'code'
      }
    },
    controls: {
      exclude: ['apiFilterChange']
    }
  },
  decorators: [generateModuleMetaDataForStorybook(AmcsComponentFilterComponent, [ComponentFilterStorageService, SharedTranslationsService])]
} as Meta;

const Template: Story<AmcsComponentFilterComponent> = (args: AmcsComponentFilterComponent) => {
  return {
    props: { ...args, apiFilterChange },
    template: ` 
    <app-amcs-browser-tile [options]="tileOptions">
    <app-amcs-component-filter 
    [isApiFilter]=isApiFilter
    [isOverlay]="true"
    (apiFilterChange)="apiFilterChange($event)"
    [properties]="componentFilterProperties" 
    [(data)]="componentFilterData">
    <app-amcs-grid 
    [columns]="componentFilterColumns" 
    [data]="componentFilterData">
    </app-amcs-grid>
  </app-amcs-component-filter>
  </app-amcs-browser-tile>`
  };
};

export const Basic = Template.bind({});
Basic.args = {
  ...Basic.args,
  tileOptions: buildTileOptions(false),
  componentFilterProperties: ComponentFilterSB.getFilterProperties(),
  componentFilterData: buildRows(),
  componentFilterColumns: ComponentFilterSB.getGridColumns(),
  isApiFilter: false
};

export const ApiFilter = Template.bind({});
ApiFilter.args = {
  ...ApiFilter.args,
  tileOptions: buildTileOptions(true),
  componentFilterProperties: ComponentFilterSB.getFilterProperties(),
  componentFilterData: buildRows(),
  componentFilterColumns: ComponentFilterSB.getGridColumns(),
  isApiFilter: true
};

function apiFilterChange(filter) {
  //SB does local filtering even with isApiFilter option
  //insert your api call with provided filter here
  console.log(filter);
}

function buildRows(): ComponentFilterSB[] {
  const today = AmcsDate.create();
  const componentFilterData = [];
  componentFilterData.push(new ComponentFilterSB('Inbound', 'Cardboard', 5, 3.5, today, AmcsDate.addDays(today, 7)));
  componentFilterData.push(new ComponentFilterSB('Outbound', 'Cardboard', 10, 3.5, today, AmcsDate.addDays(today, 10)));
  componentFilterData.push(
    new ComponentFilterSB('Bin Collection', 'Rubbish', 5, 2.5, AmcsDate.addDays(today, -5), AmcsDate.addDays(today, 20))
  );
  componentFilterData.push(new ComponentFilterSB('Inbound', 'Paper', 5, 2.5, today, AmcsDate.addDays(today, 7)));
  componentFilterData.push(
    new ComponentFilterSB('Bin Collection', null, 10, 2.5, AmcsDate.addDays(today, -3), AmcsDate.addDays(today, 15))
  );
  componentFilterData.push(new ComponentFilterSB('Inbound', 'Paper', 5, 4.5, today, AmcsDate.addDays(today, 12)));
  componentFilterData.push(new ComponentFilterSB('Bin Collection', 'Rubbish', 3, 2.5, today, AmcsDate.addDays(today, 21)));
  componentFilterData.push(new ComponentFilterSB('Outbound', 'Paper', 5, 5, AmcsDate.addDays(today, -7), AmcsDate.addDays(today, 10)));
  componentFilterData.push(new ComponentFilterSB('Outbound', 'Paper', 5, 2.25, today, AmcsDate.addDays(today, 20)));
  componentFilterData.push(new ComponentFilterSB('Inbound', 'Rubbish', 5, 12.5, today, AmcsDate.addDays(today, 5)));
  componentFilterData.push(new ComponentFilterSB('Outbound', 'Cardboard', 5, 10, AmcsDate.addDays(today, -5), AmcsDate.addDays(today, 6)));
  componentFilterData.push(new ComponentFilterSB('Inbound', 'Paper', 5, 2.5, today, AmcsDate.addDays(today, 11)));
  return componentFilterData;
}

function buildTileOptions(isApiFilter: boolean): BrowserOptions {
  const options = new BrowserOptions();
  options.title = isApiFilter?'AMCS Component Filter - Api Filtering':'AMCS Component Filter - Local Filtering';
  return options;
}
