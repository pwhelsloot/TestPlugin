@dataServer
Feature: EntityObjectManager

Validating entity object manager ouput on various conditions

Scenario: Exception is thrown when calling entity object manager with null
	Given type manager as null
	When entity object manager is initiated
	Then null reference exception is thrown

Scenario: Correct result shown when calling entity object manager with empty type manager
	Given type manager as empty
	When entity object manager is initiated
	Then entity count is zero