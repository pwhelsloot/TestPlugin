

var HTMLHint = require('htmlhint').HTMLHint;
// A rule ensures our [uniqueKey] attributes are unique per file
HTMLHint.addRule({
    id: 'automation-unique-key-uniqueness',
    description: 'Ensures uniqueKey attibutes are unique per file.',
    init: function (parser, reporter) {
        var self = this;
        let uniqueKeys = [];
        var tagStart = function (event) {
            var uniqueKeyIndex = getAttributeIndex(event, 'uniqueKey');
            if (uniqueKeyIndex !== null) {
                var uniqueKeyValue = event.attrs[uniqueKeyIndex].value;
                if (uniqueKeys.includes(uniqueKeyValue)) {
                    // We allow duplicate unique keys if a formControlName exists as this will be unique
                    if (!isAttributeFound(event, 'formControlName')) {
                        reporter.warn('uniqueKey: ' + uniqueKeyValue + ' is not unique (in this file)', event.line, event.col, self, event.raw);
                    }
                } else {
                    uniqueKeys.push(uniqueKeyValue);
                }
            }
        };
        var end = function (event) {
            uniqueKeys = [];
        }
        parser.addListener('tagstart', tagStart.bind(this));
        parser.addListener('end', end.bind(this));

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