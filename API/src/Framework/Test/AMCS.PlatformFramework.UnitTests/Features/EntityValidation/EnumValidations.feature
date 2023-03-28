@entityValidation
Feature: EnumValidations

Validating Enum entities

Scenario Outline: Enum validation
	Given a set of enums
	When enums are validated with an <EnumExpression>
	Then corresponding results is shown

Examples:
	| EnumExpression    |
	| EnumProperty == 2 |
	| EnumProperty < 3  |