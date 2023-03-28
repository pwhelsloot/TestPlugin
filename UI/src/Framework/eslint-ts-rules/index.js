var importBanRule = require('./rules/import-ban');
var apiRequestBanRule = require('./rules/api-request-ban');

//------------------------------------------------------------------------------
// Rule Definition
//------------------------------------------------------------------------------

module.exports.rules = {
  'import-ban': importBanRule,
  'api-request-ban': apiRequestBanRule
};
