// The file contents for the current environment will overwrite these during build.
// The build system defaults to the dev environment which uses `environment.ts`, but if you do
// `ng build --env=prod` then `environment.prod.ts` will be used instead.
// The list of which env maps to which file can be found in `.angular-cli.json`.
// can take away fastSearch

export const environment = {
  applicationVersion: '1.0.0.0',
  production: false,
  elemosClientURL: 'https://install.amcsgroup.io/AMCS.Setup.Install.GUI.application?appurl=https://f1-dev-svc-client.amcsplatform.com//ELEMOS-f1-dev.amcsapplication',
  languages: ['en', 'en-GB', 'nb', 'es-MX', 'en-AU', 'nl', 'fr-FR', 'de-DE'],
  exagoApiScriptUrl: 'http://localhost/ExagoWebReports/WrScriptResource.axd?s=ExagoApi',
  applicationType: 'template',
  applicationURLPrefix: 'template/assets',
  applicationApiPrefix: 'template/api',
  isAutomationEnabled: true
};
