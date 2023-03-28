module.exports = {
  meta: {
    schema: []
  },
  create: function (context) {
    return {
      Element: (node) => {
        if (node.name === 'form') {
          const sourceCode = context.getSourceCode();
          const comments = sourceCode.getAllComments();
          const ignoreCommentExists = comments && comments.some((x) => x.value === 'html-hint-disable form-tag-ban');
          if (!ignoreCommentExists) {
            context.report({ loc: { start: node.sourceSpan.start, end: node.sourceSpan.end } }, 'form elements are banned');
          }
        }
      }
    };
  }
};
