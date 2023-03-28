// Karma configuration file, see link for more information
// https://karma-runner.github.io/1.0/config/configuration-file.html

module.exports = function (config) {
  config.set({
    basePath: '',
    frameworks: ['jasmine', '@angular-devkit/build-angular'],
    plugins: [
      require('karma-jasmine'),
      require('karma-chrome-launcher'),
      require('karma-jasmine-html-reporter'),
      require('karma-sabarivka-reporter'),
      require('karma-coverage'),
      require('karma-junit-reporter'),
      require('@angular-devkit/build-angular/plugins/karma'),
    ],
    client: {
      clearContext: false, // leave Jasmine Spec Runner output visible in browser
      captureConsole: false
    },
    junitReporter: {
      outputDir: './test-results', // results will be saved as $outputDir/$browserName.xml
      outputFile: 'junit.xml', // if included, results will be saved as $outputDir/$browserName/$outputFile
      suite: '', // suite will become the package name attribute in xml testsuite element
      useBrowserName: false, // add browser name to report and classes names
      nameFormatter: undefined, // function (browser, result) to customize the name attribute in xml testcase element
      classNameFormatter: undefined, // function (browser, result) to customize the classname attribute in xml testcase element
      properties: {}, // key value pair of properties to add to the <properties> section of the report
    },
    reporters: ['progress', 'kjhtml', 'junit', 'sabarivka', 'coverage'],
    coverageReporter: {
      dir: require('path').join(__dirname, 'coverage'),
      reporters: [
        { type: 'html', subdir: '.' },
        { type: 'lcovonly', subdir: '.' },
        { type: 'cobertura', subdir: '.' },
        { type: 'text-summary', subdir: '.' },
      ],
      check: {
        emitWarning: false,
        global: {
          lines: 11.3,
        },
      },
      include: [
        'auth/**/!(*.spec|*.stories|*.module|environment*|main|*.constants|*.enum).ts',
        'core/**/!(*.spec|*.stories|*.module|environment*|main|*.constants|*.enum).ts',
        'shared/**/!(*.spec|*.stories|*.module|environment*|main|*.constants|*.enum).ts',
        'translate/!(*.spec|*.stories|*.module|environment*|main|*.constants|*.enum).ts',
      ],
    },
    port: 9876,
    colors: true,
    logLevel: config.LOG_INFO,
    autoWatch: true,
    browsers: ['Chrome'],
    singleRun: false,
  });
};
