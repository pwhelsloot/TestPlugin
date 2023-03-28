@entityValidation
Feature: InvocationValidations

Validating String Invocation methods

Scenario Outline: See given string is Null Or Empty
	Given a string <String>
	When value is validated with an <ValidationExpression> by invoking twice
	Then actual result matches expected result <ExpectedResult>

Examples:
	| String     | ValidationExpression                 | ExpectedResult |
	|            | String.IsNullOrEmpty(StringProperty) | true           |
	| HelloWorld | String.IsNullOrEmpty(StringProperty) | false          |

Scenario Outline: See given string is Null Or with whitespace
	Given a string <String>
	When value is validated with an <ValidationExpression> by invoking twice
	Then actual result matches expected result <ExpectedResult>

Examples:
	| String       | ValidationExpression                      | ExpectedResult |
	|              | String.IsNullOrWhiteSpace(StringProperty) | true           |
	| <WhiteSpace> | String.IsNullOrWhiteSpace(StringProperty) | true           |
	| HelloWorld   | String.IsNullOrWhiteSpace(StringProperty) | false          |

Scenario Outline: See given string matches a regex
	Given a string <String>
	When value is validated with an <ValidationExpression> by invoking twice
	Then actual result matches expected result <ExpectedResult>

Examples:
	| String     | ValidationExpression                                                     | ExpectedResult |
	| 456        | Regex.IsMatch(StringProperty, "^[0-9]+$")                                | true           |
	| HelloWorld | Regex.IsMatch(StringProperty, "^[0-9]+$")                                | false          |
	| 456        | System.Text.RegularExpressions.Regex.IsMatch(StringProperty, "^[0-9]+$") | true           |
	| HelloWorld | System.Text.RegularExpressions.Regex.IsMatch(StringProperty, "^[0-9]+$") | false          |

	