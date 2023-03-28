var deprecatedTagBan = require('./rules/deprecated-tag-ban');
var formTagBanRule = require('./rules/form-tag-ban');
var ngProjectLabelBan = require('./rules/ng-project-label-ban')
//------------------------------------------------------------------------------
// Rule Definition
//------------------------------------------------------------------------------

module.exports.rules = {
  'form-tag-ban': formTagBanRule,
  'deprecated-tag-ban': deprecatedTagBan,
  'ng-project-label-ban': ngProjectLabelBan
};
