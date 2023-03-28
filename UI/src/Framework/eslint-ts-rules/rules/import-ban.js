module.exports = {
  meta: {
    schema: [],
  },
  create: function (context) {
    return {
      ImportDeclaration: (node) => {
        if (node.source && node.source.value === 'rxjs/Rx') {
          context.report(node, 'rxjs/Rx import is banned');
        }
      },
    };
  },
};
