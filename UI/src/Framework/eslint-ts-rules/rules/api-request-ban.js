module.exports = {
  meta: {
    schema: [],
  },
  create: function (context) {
    return {
      NewExpression: (node) => {
        if (node.callee.type === 'Identifier' && node.callee.name === 'ApiRequest') {
          context.report({
            node,
            message:
              'Creating ApiRequests has been banned, please use the ApiBusinessService or extend from ApiBaseService to access the api',
            data: { identifier: node.name }
          });
        }
      },
    };
  },
};
