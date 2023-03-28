import { EmptyRouteComponent } from '@shared-module/empty-route/empty-route.component';
import { StorybookGroupTitles } from '@storybook-util/storybook-group.titles';
import { Meta, moduleMetadata, Story } from '@storybook/angular';

export default {
  title: StorybookGroupTitles.Misc + 'Figma Prototypes',
  component: EmptyRouteComponent, // Component to display
  parameters: {
    viewMode: 'docs',
    docs: {
      page: () =>
        `This a is collection of figma prototypes which are yet to be converted to angular components.
          On the \'Canvas\' tab please select \'Design\' addon tab to view the figma prototype.`,
    },
  },
  decorators: [
    moduleMetadata({
      declarations: [EmptyRouteComponent],
    }),
  ],
} as Meta;

const Template: Story<EmptyRouteComponent> = (args) => ({
  props: {},
});
export const WarningsAndRouteStatus = Template.bind({});
WarningsAndRouteStatus.parameters = {
  design: {
    type: 'figma',
    url: 'https://www.figma.com/file/nG8aIOxihvD0NnjmS6hP93/AMCS-Design-System-Library-v1?node-id=277%3A0',
    allowFullscreen: true,
  },
};
