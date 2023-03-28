var HTMLHint = require('htmlhint').HTMLHint;
// A rule which ensures our form controls have the required appAutomationLocator inputs
HTMLHint.addRule({
    id: 'form-control-automation-inputs',
    description: 'Ensures form controls have the required appAutomationLocator inputs.',
    init: function (parser, reporter) {
        var self = this;
        var tagStart = function (event) {
            if (isAttributeFound(event, 'formControlName') || isAttributeFound(event, 'formControl')) {
                if (!isAttributeFound(event, 'compName')) {
                    reporter.warn('compName input is missing', event.line, event.col, self, event.raw);
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