@dataServer
Feature: BusinessObjectService

Validating functinality of business object service on various situations

Scenario: Correct results are returned when fetching with business object service using entity object
	Given businessObjectService is started
	When entity is fetched using DummyObject
	Then correct results are returned

Scenario: Correct results are returned when fetching with business object service using entity type
	Given businessObjectService is started
	When entity is fetched using DummyEntity
	Then correct results are returned

Scenario: All values are returned when fetching all with business object service
	Given businessObjectService is started
	When all of objects are requested using service
	Then all of objects are returned

Scenario Outline: Exception will be thrown when called business object service with invalid input
	Given invalid input data <InvalidData>
	When business service is started
	Then exception is thrown
Examples:
	| InvalidData                      |
	| InvalidBOMultipleObjectNames.xml |
	| InvalidBOTableSchema.xml         |
	