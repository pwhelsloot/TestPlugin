@apiServiceFilter
Feature: ExpressionParser

Validating given expression parse to excpected result

Scenario: Combination of filter criterias match the expected filter expression generated
	Given various filter criterias
	When expression built out of them
	Then it matches expected filter expression

Scenario: Exception thrown on providing no filter criterias
	Given no filter criterias
	When validated for expression builder
	Then it fails the validation