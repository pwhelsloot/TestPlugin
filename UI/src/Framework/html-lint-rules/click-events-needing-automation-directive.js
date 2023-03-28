var HTMLHint = require('htmlhint').HTMLHint;
// A rule which ensures the appAutomationLocator directive is on any tag with a click event
// However a click event on the below tags doesnt require the directive as these tags extend from it.
var excludedTags = ['app-amcs-save-button',
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
    'app-amcs-highlight-icon'
];
HTMLHint.addRule({
    id: 'click-events-needing-automation-directive',
    description: 'Ensures click events have the appAutomationLocator directive.',
    init: function (parser, reporter) {
        var self = this;
        var tagStart = function (event) {
            if (isAttributeFound(event, '(click)')) {
                // Ignore cases like amcs-button which do have a button element but don't need the directive
                // as they attach the data-testId manually
                if (isAttributeFound(event, 'attr.data-testId')) {
                    return;
                }
                // Ignore cases where the click event action is hidden from user and preventing some default behaviour
                var clickAttributeIndex = getAttributeIndex(event, '(click)');
                if (event.attrs[clickAttributeIndex].value.includes('preventDefault')) {
                    return;
                }
                if (excludedTags.includes(event.tagName.toLowerCase())) {
                    return;
                }
                if (!isAttributeFound(event, 'formControlName') && !isAttributeFound(event, 'appAutomationLocator')) {
                    reporter.warn('appAutomationLocator directive missing', event.line, event.col, self, event.raw);
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
                eventAttributeName = eventAttributeName.replace('[', '').replace(']', '');
                if (eventAttributeName === attributeName) {
                    foundAttribute = true;
                    break;
                }
            }
            return foundAttribute;
        }
        function getAttributeIndex(event, attributeName) {
            if (!event.attrs) {
                event.attrs = [];
            }
            for (var i = 0; i < event.attrs.length; i++) {
                let eventAttributeName = event.attrs[i].name;
                // Remove the '[' and ']' characters if they exist
                eventAttributeName = eventAttributeName.replace('[', '').replace(']', '');
                if (eventAttributeName === attributeName) {
                    return i;
                }
            }
            return null;
        }
    }
});