@dataServer
Feature: EntityObjectService

Validating responses on various values passed to EntityObjectService

Scenario: Null exception thrown when criteria is null
	Given parameter Criteria is Null
	When EntityObjectService is called
	Then null exception is thrown

Scenario: Null exception thrown when userd id is null
	Given parameter UserId is Null
	When EntityObjectService is called
	Then null exception is thrown

Scenario Outline: Appropriate response is got when entityObject service calling query exists having data session
	Given various parameters
	And mock setup return object as <ReturnObjectType>
	When EntityObjectService is called
	Then appropriate response is shown as expected result <ExpectedResult>
Examples:
	| ReturnObjectType | ExpectedResult |
	| 1                | true           |
	| null             | false          |

Scenario: Null exception thrown when entityObject service calling query has no data session
	Given various parameters
	And parameter DataSession is Null
	When EntityObjectService is called
	Then null exception is thrown
