module.exports = {
  core: {
    builder: 'webpack5',
  },
  stories: ['../shared/**/*.stories.mdx', '../shared/**/*.stories.@(js|jsx|ts|tsx)'],
  addons: ['storybook-addon-designs', '@storybook/addon-links', '@storybook/addon-essentials'],
};
