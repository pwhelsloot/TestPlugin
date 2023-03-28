import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { generateModuleMetaDataForStorybook } from '@storybook-util/storybook-metadata.generator';
import { componentWrapperDecorator, Meta, Story } from '@storybook/angular';
import { BrowserOptions } from '../layouts/amcs-browser-grid-layout/browser-options-model';
import { AmcsStatusBadgeComponent } from './amcs-status-badge.component';
import { StatusBadgeDisplayModeEnum, StatusBadgeTypeEnum } from './status-badge.enum';

export default {
  title: StorybookGroupTitles.Form + 'Status Badge',
  component: AmcsStatusBadgeComponent,
  decorators: [
    componentWrapperDecorator(
      (story) =>
        `<app-amcs-browser-tile [options]="tileOptions"><div [ngStyle]="{ 'width': '150px', 'height': '30px' }">${story}</div></app-amcs-browser-tile>`
    ),
    generateModuleMetaDataForStorybook(AmcsStatusBadgeComponent, [])
  ],
  args: {
    displayMode: StatusBadgeDisplayModeEnum.Both
  },argTypes: {
    // Hide these from control options
    label: {
      control: { type: 'text' },
      description: 'Text that is being displayed on the badge, if label is blank (null) it displays progress.',
    },
    progress: {
      control: { type: 'text' },
      description: 'Progress of the status (0-100%), input is optional.',
    },
    matchParentWidth: {
      description: 'If true, takes up 100% of parents width (i.e. for grid), if false puts 10px padding on the status text.',
      control: { type: 'boolean' },
    },
    badgeType: {
      control: { type: 'select' },
      options: StatusBadgeTypeEnum,
      description: 'Dictates the display color scheme of the badge. Use StatusBadgeTypeEnum to select.',
    },
    displayMode: {
      control: { type: 'select' },
      options: StatusBadgeDisplayModeEnum,
      description: 'Dictates the display mode of the badge. Use StatusBadgeTypeEnum to select (default TextOnly)',
    },
    faIconClass: {
      control: { type: 'text' },
      description: 'FontAwesome icon to display, can take color classes too i.e. \'far fa-info-circle text-warning\'.',
    },
  },
  parameters: {
    design: {
      disabled: true
    },
    docs: {
      description: {
        component: 'I am the default Amcs Status Badge. I can be of fixed color, or be a progress bar.'
      },
      source: {
        type: 'dynamic'
      }
    }
  }
} as Meta<AmcsStatusBadgeComponent>;

const Template: Story<AmcsStatusBadgeComponent> = (args) => ({
  props: { ...args, StatusBadgeTypeEnum, StatusBadgeDisplayModeEnum }
});

export const Positive = Template.bind({});
Positive.args = {
  ...Positive.args,
  label: 'Positive',
  badgeType: StatusBadgeTypeEnum.Positive,
  tileOptions: buildTileOptions('Positive'),
  faIconClass: 'fas fa-check'
};

export const Warning = Template.bind({});
Warning.args = {
  ...Warning.args,
  badgeType: StatusBadgeTypeEnum.Warning,
  tileOptions: buildTileOptions('Warning'),
  faIconClass: 'fas fa-poop',
  progress: '33%'
};

export const Negative = Template.bind({});
Negative.args = {
  ...Negative.args,
  label: 'Negative',
  badgeType: StatusBadgeTypeEnum.Negative,
  tileOptions: buildTileOptions('Negative'),
  faIconClass: 'fas fa-stop'
};

export const Informative = Template.bind({});
Informative.args = {
  ...Informative.args,
  label: 'Informative',
  badgeType: StatusBadgeTypeEnum.Informative,
  tileOptions: buildTileOptions('Informative, Match Column Width'),
  faIconClass: 'fas fa-eye',
  progress: '70%',
  matchParentWidth: true
};

export const InformativeAlt = Template.bind({});
InformativeAlt.args = {
  ...InformativeAlt.args,
  label: 'InformativeAlt',
  badgeType: StatusBadgeTypeEnum.InformativeAlt,
  displayMode: StatusBadgeDisplayModeEnum.IconOnly,
  tileOptions: buildTileOptions('InformativeAlt, Icon Only'),
  faIconClass: 'far fa-info-circle text-warning'
};

export const Neutral = Template.bind({});
Neutral.args = {
  ...Neutral.args,
  label: 'Neutral',
  displayMode: StatusBadgeDisplayModeEnum.TextOnly,
  badgeType: StatusBadgeTypeEnum.Neutral,
  tileOptions: buildTileOptions('Neutral, Text Only')
};

export const NeutralNegative = Template.bind({});
NeutralNegative.args = {
  ...NeutralNegative.args,
  label: 'NeutralNegative',
  badgeType: StatusBadgeTypeEnum.NeutralNegative,
  tileOptions: buildTileOptions('NeutralNegative'),
  faIconClass: 'fas fa-clock'
};

export const Positive2 = Template.bind({});
Positive2.args = {
  ...Positive2.args,
  label: 'Positive2',
  badgeType: StatusBadgeTypeEnum.Positive2,
  tileOptions: buildTileOptions('Positive2'),
  faIconClass: 'fas fa-check-circle'
};

export const InformativeAlt2 = Template.bind({});
InformativeAlt2.args = {
  ...InformativeAlt2.args,
  label: 'InformativeAlt2',
  badgeType: StatusBadgeTypeEnum.InformativeAlt2,
  tileOptions: buildTileOptions('InformativeAlt2'),
  faIconClass: 'fas fa-exclamation-circle'
};

function buildTileOptions(title: string): BrowserOptions {
  const options = new BrowserOptions();
  options.title = `Status Badge - ${title}`;
  return options;
}
