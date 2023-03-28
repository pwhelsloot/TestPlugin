var HTMLHint = require('htmlhint').HTMLHint;
// A rule which ensures specific tags have the required appAutomationLocator inputs
var tags = ['app-amcs-save-button',
    'app-amcs-button',
    'app-amcs-select',
    'app-amcs-input',
    'app-amcs-date-range-filter',
    'app-amcs-expand-collapse',
    'app-amcs-datepicker',
    'app-combination-selector',
    'app-amcs-select-with-search',
    'app-amcs-numerical-input',
    'app-amcs-text-area',
    'app-amcs-switch',
    'app-amcs-timepicker',
    'app-amcs-dropdown',
    'app-amcs-multi-select-with-search',
    'app-amcs-typeahead',
    'app-amcs-select-typeahead',
    'app-amcs-select-compact-typeahead',
    'app-amcs-highlight-icon',
    'app-amcs-external-payment-button',
    'app-amcs-button-login-google',
    'app-amcs-button-login-azure',
    'app-amcs-button-login-okta',
    'app-amcs-radio-tile',
    'app-amcs-highlight-icon'];
HTMLHint.addRule({
    id: 'tags-needing-automation-inputs',
    description: 'Ensures specific tags have the required appAutomationLocator inputs.',
    init: function (parser, reporter) {
        var self = this;
        var tagStart = function (event) {
            if (tags.includes(event.tagName.toLowerCase())) {
                if (!isAttributeFound(event, 'compName')) {
                    reporter.warn('compName missing', event.line, event.col, self, event.raw);
                }
                // Only need uniqueKey if formControlName isn't provided
                if (!isAttributeFound(event, 'uniqueKey') && !isAttributeFound(event, 'formControlName')) {
                    reporter.warn('uniqueKey input is missing', event.line, event.col, self, event.raw);
                }
            }
        };
        parser.addListener('tagstart', tagStart.bind(this));

        // helpers (can't be shared with basic .js)
        function isAttributeFound(event, attributeName) {
            let foundAttribute = false;
            if (!event.attrs) {
                event.attrs = [];
            }
            for (var i = 0; i < event.attrs.length; i++) {
                let eventAttributeName = event.attrs[i].name;
                // Remove the '[' and ']' characters if they exist
                eventAttributeName = eventAttributeName.replace('[', '').replace(']', '');
                if (eventAttributeName === attributeName) {
                    foundAttribute = true;
                    break;
                }
            }
            return foundAttribute;
        }
    }
});