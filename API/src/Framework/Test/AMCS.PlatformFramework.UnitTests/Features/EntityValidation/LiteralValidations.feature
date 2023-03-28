@entityValidation
Feature: LiteralValidations

Validating string,numeric,boolean comparisons and grouping of different types

Scenario Outline: Validating numeric comparisons
	Given two numerics value1 <Value1> value2 <Value2>
	When they are compared with comparator <Comparator>
	Then actual result matches expected result <ExpectedResult>

Examples:
	| Value1 | Value2 | Comparator | ExpectedResult |
	| 1      | 2      | <          | true           |
	| 1      | 2      | >          | false          |
	| 1      | 2      | <=         | true           |
	| 1      | 2      | >=         | false          |
	| 1      | 2      | !=         | true           |
	| 1      | 1.0    | ==         | true           |
	| 1e1    | 10     | ==         | true           |
	| 1.0e1  | 10     | ==         | true           |
	| 0.1e2  | 10     | ==         | true           |
	| .1e2   | 10     | ==         | true           |
	| -1     | 1      | <          | true           |
	| --1    | 1      | ==         | true           |
	| ++1    | 1      | ==         | true           |

Scenario Outline: Validating boolean comparisons
	Given two boolean value1 <BooleanValue1> value2 <BooleanValue2>
	When they are compared with comparator <Comparator>
	Then actual result matches expected result <ExpectedResult>

Examples:
	| BooleanValue1 | BooleanValue2 | Comparator | ExpectedResult |
	| true          |               |            | true           |
	| false         |               |            | false          |
	| !false        |               |            | true           |
	| !!true        |               |            | true           |
	| true          | true          | AND        | true           |
	| true          | false         | OR         | true           |
	| true          | true          | OR         | true           |
	| false         | true          | OR         | true           |
	| false         | false         | OR         | false          |
	| false         | true          | AND        | false          |
	| true          | false         | AND        | false          |

Scenario Outline: Validating string comparisons
	Given two string value1 <StringValue1> value2 <StringValue2>
	When they are compared with comparator <Comparator>
	Then actual result matches expected result <ExpectedResult>

Examples:
	| StringValue1 | StringValue2 | Comparator | ExpectedResult |
	| "ab"         | "ab"         | ==         | true           |
	| "a"          | "a"          | ==         | true           |
	| "a"          | "b"          | ==         | false          |
	| "a"          | "b"          | !=         | true           |
	| "a"          | "b"          | >          | false          |
	| "a"          | "a"          | >=         | true           |
	| "a"          | "a"          | >          | false          |
	| "a"          | "b"          | <          | true           |
	| "a"          | "b"          | <=         | true           |
	