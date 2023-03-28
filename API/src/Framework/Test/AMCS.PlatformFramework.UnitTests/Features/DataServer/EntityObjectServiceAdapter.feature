@dataServer
Feature: EntityObjectServiceAdapter

Validating responses on values passed to EntityObjectServiceAdapter

Scenario: Null exception thrown when criteria is null
	Given parameter UserId is Null
	When EntityObjectServiceAdapter is called
	Then null exception is thrown
