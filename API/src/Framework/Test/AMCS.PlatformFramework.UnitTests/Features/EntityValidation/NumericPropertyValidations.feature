@entityValidation
Feature: NumericPropertyValidations

Validating the given numeric value different properties

@tag1
Scenario: Checking different propeties of a given numeric value
	Given a numeric value <NumericValue>
	When numeric value is validated with an <ValidationExpression>
	Then actual result matches expected result <ExpectedResult>

Examples:
	| NumericValue | ValidationExpression   | ExpectedResult |
	| 42           | SByteProperty == 42    | true           |
	| 42           | ByteProperty == 42     | true           |
	| 42           | Int16Property == 42    | true           |
	| 42           | UInt16Property == 42   | true           |
	| 42           | Int32Property == 42    | true           |
	| 42           | UInt32Property == 42   | true           |
	| 42           | Int64Property == 42    | true           |
	| 42           | UInt64Property == 42ul | true           |
	| 42           | SingleProperty == 42   | true           |
	| 42           | DoubleProperty == 42   | true           |
	| 42           | DecimalProperty == 42  | true           |