@entityValidation
Feature: StringPropertyValidations

Validating properties of a given string

Scenario: Checking the value of given string
	Given a string <String>
	When value is validated with an <StringExpression> by invoking once
	Then actual result matches expected result <ExpectedResult>
Examples:
	| String | StringExpression        | ExpectedResult |
	| Hi     | StringProperty == "Hi"  | true           |
	| Hi     | StringProperty == "Bye" | false          |
	| Hi     | StringProperty != null  | true           |
	| Hi     | StringProperty == null  | false          |