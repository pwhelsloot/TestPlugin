// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.
// can take away fastSearch

export const environment = {
  applicationVersion: '***applicationVersion***',
  production: true,
  elemosClientURL: '***elemosClientURL***',
  languages: ['en', 'en-GB', 'nb'],
  exagoApiScriptUrl: '***exagoApiScriptUrl***',
  applicationType: 'template',
  applicationURLPrefix: 'template/ui/',
  applicationApiPrefix: 'template',
  isAutomationEnabled: false
};
