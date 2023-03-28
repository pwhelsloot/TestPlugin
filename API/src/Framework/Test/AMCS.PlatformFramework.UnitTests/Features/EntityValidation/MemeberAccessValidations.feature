@entityValidation
Feature: MemeberAccessValidations
Validate string length based on input

Scenario Outline: String length validation based on input provided
	Given a string <StringValue>
	When value is validated with an <ValidationExpression> by invoking once
	Then actual result matches expected result <ExpectedResult>

Examples:
	| StringValue | ValidationExpression                                 | ExpectedResult |
	| Hi          | StringProperty != null && StringProperty.Length < 10 | true           |
	|             | StringProperty != null && StringProperty.Length < 10 | false          |
