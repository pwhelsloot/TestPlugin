@entityValidation
Feature: NullableValidations

Validate given value is null or not

Scenario Outline: See value provided is null or not
	Given an integer value <IntegerValue> enum value <EnumValue>
	When values are checked with validation expression <ValidationExpression>
	Then actual result matches expected result <ExpectedResult>

Examples:
# Enum value corresponds to value in the Enum list under Test properties folder
	| IntegerValue | EnumValue | ValidationExpression | ExpectedResult |
	|              |           | IntProperty == null  | true           |
	|              |           | EnumProperty == null | true           |
	|              |           | IntProperty == 42    | false          |
	| 42           | B         | IntProperty == null  | false          |
	| 42           | B         | IntProperty == 42    | true           |
	| 42           | B         | EnumProperty != null | true           |
	| 42           | B         | EnumProperty == 2    | true           |