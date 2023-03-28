var m = setDeprecatedTags()
function setDeprecatedTags(){
  var m = new Map();
  m.set('app-amcs-browser-tile-deprecated','app-amcs-browser-tile');
  m.set('app-amcs-container-tile', 'app-dashboard-tile');
  m.set('app-amcs-child-table','app-amcs-grid');
  m.set('app-amcs-collapse');
  m.set('app-amcs-table','app-amcs-grid');
  m.set('app-amcs-comment','amcs-text-area');
  m.set('app-amcs-cost-template-selector','app-amcs-modal-selector');
  m.set('app-amcs-select2','app-amcs-select');
  m.set('app-amcs-selector-grid','app-amcs-grid with "showSelectedCheckbox" set to True');
  m.set('app-amcs-data-input-grid','app-amcs-grid-form-simple');
  m.set('app-amcs-daterangepicker','app-amcs-datepicker');
  m.set('app-amcs-dropdown-menu','app-amcs-dropdown');
  m.set('app-amcs-dropdown-select','app-amcs-select');
  m.set('app-amcs-filter-single-select','any other amcs select');
  m.set('app-amcs-menu-selector','app-amcs-dropdown');
  m.set('app-sidebar-menu');
  m.set('app-amcs-report-tile','app-dashboard-tile');
  m.set('app-amcs-search-portlet','app-dashboard-tile');
  m.set('app-amcs-select-definition-deprecated','app-amcs-select-definition');
  m.set('app-amcs-select-deprecated','app-amcs-select');
  m.set('app-amcs-timepicker-deprecated','app-amcs-timepicker');
  m.set('app-amcs-timepicker-extended','app-amcs-timepicker');
  m.set('app-amcs-typeahead','app-amcs-select-typeahead');
  return m;
}

module.exports = {
  meta: {
    schema: []
  },
  create: function (context) {
    return {
      Element: (node) => {
        if (m.has(node.name)) {
          let helpText = '';
          if(m.get(node.name))
          {
            helpText = `, use ${m.get(node.name)} instead`;
          }else {
            helpText = ' and will be removed in the future'
          }
          const sourceCode = context.getSourceCode();
          const comments = sourceCode.getAllComments();
          const ignoreCommentExists = comments && comments.some((x) => x.value === 'eslint-disable amcs-html-plugin/deprecated-tag-ban');
          if (!ignoreCommentExists) {
            context.report({ loc: { start: node.sourceSpan.start, end: node.sourceSpan.end } }, `${node.name} is deprecated${helpText}`);
          }
        }
      }
    };
  }
};
