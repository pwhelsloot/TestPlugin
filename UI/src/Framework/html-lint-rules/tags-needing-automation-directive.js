var HTMLHint = require('htmlhint').HTMLHint;
// A rule which ensures the appAutomationLocator directive is on the below tags
var tags = ['input', 'button', 'mat-select', 'table'];
HTMLHint.addRule({
    id: 'tags-needing-automation-directive',
    description: 'Ensures the specified tags have the automation locator directive.',
    init: function (parser, reporter) {
        var self = this;
        var tagStart = function (event) {
            if (tags.includes(event.tagName.toLowerCase())) {

                // Ignore any hidden input fields
                var typeIndex = getAttributeIndex(event, 'type');
                if (typeIndex !== null) {
                    if (event.attrs[typeIndex].value === 'hidden') {
                        return;
                    }
                }
                // Ignore cases like amcs-button which do have a button element but don't need the directive
                // as they attach the data-testId manually
                if (isAttributeFound(event, 'attr.data-testId')) {
                    return;
                }
                // Only need directive if formControlName isn't provided (?)
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




    //   The following code is a rough example of the 'a' tag, the directive is allowed to be on the tag itself or any tag within in.
   //   Comment out as results in lots of errors, some 'a' tags are within 'li's which hold the directive...

    //    var aTags = [];
    //    var innerATags = [];
    //    var aTagId = 1;
    //    var insideATag = false;

    //    var atagStart = function (event) {
    //        var tagName = event.tagName.toLowerCase();
    //        if (tagName === 'a') {
    //            insideATag = true;
    //            if (isDirectiveMissing(event)) {
    //                aTags.push({ event: event, id: aTagId });
    //            }
    //        } else if (insideATag) {
    //            innerATags.push({ event: event, id: aTagId });
    //        }
    //    }
    //    var atagEnd = function (event) {
    //        var tagName = event.tagName.toLowerCase();
    //        if (tagName === 'a') {
    //            aTagId++;
    //            insideATag = false;
    //            innerATags.push({ event: event, id: aTagId });
    //        }
    //    }
    //    var end = function (event) {
    //        aTags.forEach(function (aTag) {
    //            let isDirectiveMissing = false;
    //            innerATags.forEach(function (innerATag) {
    //                if (isDirectiveMissing) {
    //                    return;
    //                }
    //                isDirectiveMissing = isDirectiveMissing(innerATag);
    //            }.bind(this));
    //            if (isDirectiveMissing) {
    //                reporter.warn('appAutomationLocator directive missing', aTag.event.line, aTag.event.col, self, aTag.event.raw);
    //            }
    //        }.bind(this));
    //    }

    //    parser.addListener('tagstart', atagStart.bind(this));
    //    parser.addListener('tagend', atagEnd.bind(this));
    //    parser.addListener('end', end.bind(this));