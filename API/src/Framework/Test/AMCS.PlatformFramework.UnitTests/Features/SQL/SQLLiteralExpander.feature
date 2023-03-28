@sqlRelated
Feature: SQLLiteralExpander

Validating different SQL literal expander cases

Scenario Outline: Literal expander expands correctly based on given data
	Given data of type <DataType> value <Value>
	When passed to literal expander
	Then correct result is shown
	
Examples:
	| DataType     | Value      |
	| Boolean      | true       |
	| Integer      | 45         |
	| Decimal      | 42.4       |
	| MultipleBool | true, false |
	| List         | 1, 2, 3, 4    |
	| Null         |            |
