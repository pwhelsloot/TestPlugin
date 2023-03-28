var HTMLHint = require('htmlhint').HTMLHint;
// A rule which bans the <form> tag. This can be disabled via <!-- html-hint-disable form-tag-ban --> comment
// This comment MUST be directly above each EACH <form> tag to ignore in the file
HTMLHint.addRule({
    id: 'form-tag-ban',
    description: 'Bans the form tag.',
    init: function (parser, reporter) {
        var self = this;
        var previousComment = '';
        var allEvent = function (event) {
            // Always look for a comment and store it
            if (event.type === 'comment') {
                previousComment = event.content.toLowerCase().trim();
            } else if (event.type === 'tagstart' && event.tagName.toLowerCase() === 'form') {
                if (previousComment !== 'html-hint-disable form-tag-ban') {
                    reporter.warn('Found form.', event.line, event.col, self, event.raw);
                } else {
                    // Wipe previous comment incase there is a second <form> tag in same file
                    previousComment = '';
                }
                // Wipe previous comment if tag isn't form and it's not just a new line
            } else if (event.type !== 'text' && event.raw !== '\r\n') {
                previousComment = '';
            }
        };
        parser.addListener('all', allEvent.bind(this));
    }
});
