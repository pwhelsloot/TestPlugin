@testDataValidation
Feature: TestDataOptionsValidator

Various validaions on given test data

Scenario Outline: Class with test data gives appropriate ouput
Given class with data <TestData>
	When class is validated with testdata validator
	Then validation matches expected result <ExpectedResult>
Examples:
	| TestData            | ExpectedResult |
	| ValidPropertyType   | pass           |
	| InvalidPropertyType | fail           |
	| NoTestData          | fail           |
