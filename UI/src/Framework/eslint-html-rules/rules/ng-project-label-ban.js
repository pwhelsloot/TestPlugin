module.exports = {
  meta: {
    messages: {
      projectAs: 'Consider using the [label] input for this control instead, ngProjectAs is not required anymore',
    },
    schema: [],
    fixable: 'code',
  },
  create: function (context) {
    return {
      Element: (node) => {
        if (isCustomLabel(node)) {
          context.report({
            loc: { start: node.sourceSpan.start, end: node.sourceSpan.end },
            messageId: 'projectAs'
          });
        }
      },
    };

    function isCustomLabel(node) {
      return (
        node.name === 'ng-container' &&
        node.attributes &&
        node.attributes.some((attr) => attr.name === 'ngProjectAs' && attr.value === 'label')
      );
    }
  },
};
