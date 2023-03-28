@dataServer
Feature: WorkflowTemplateParser

Validating functinality of Workflow Template Parser on various situations

Scenario Outline: WorkflowTemplateParser parse template variables
	Given template <Template> value <Value>
	When WorkflowTemplateParser is built with template and value
	Then result matches <ExpectedResult> expected result
Examples:
	| Template                                                      | Value  | ExpectedResult                                    |
	| http://localhost/?id={{someVariable\|default:5}}              | 2      | http://localhost/?id=2                            |
	| http://localhost/?id={{someVariable\|default:BYEYALL\|quote}} | HIYALL | http://localhost/?id="HIYALL"                     |
	| http://localhost/?id={{someVariable\|default:BYEYALL\|quote}} | null   | http://localhost/?id="BYEYALL"                    |
	| http://localhost/?id={{someVariable}}                         | HIYALL | http://localhost/?id=HIYALL                       |
	| http://localhost/?id={{someVariable\|quote}}                  | HIYALL | http://localhost/?id="HIYALL"                     |
	| http://localhost/?id={{someVariable\|default:5}}              | null   | http://localhost/?id=5                            |
	| http://localhost/?id={{someVariable2\|default:5}}             | null   | http://localhost/?id={{someVariable2\|default:5}} |

Scenario: WorkflowTemplateParser parse json
	Given a json
	When WorkflowTemplateParser is built with json
	Then result matches expected result

Scenario Outline: Simple WorkflowTemplateParser throws exception parsing invalid template variables
	Given template <InvalidTemplate> value <Value>
	When  WorkflowTemplateParser is built with template and value
	Then InvalidOperationException is thrown
Examples:
	| InvalidTemplate               | Value           |
	| {{someVariable\|magicalpipe}} | shouldnotmatter |
	| {{someVariable\|magicalpipe}} | shouldnotmatter |
	| {{\|someVariable\|}}          | shouldnotmatter |
	| {{someVariable\|}}            | shouldnotmatter |
	| {{someVariable\|default}}     | null            |
	| {{someVariable\|default:}}    | null            |
	| {{someVariable\|default:\|}}  | null            |
	| {{}}                          | shouldnotmatter |
	| {{\|}}                        | shouldnotmatter |
	| {{\|\|\|\|}}                  | shouldnotmatter |