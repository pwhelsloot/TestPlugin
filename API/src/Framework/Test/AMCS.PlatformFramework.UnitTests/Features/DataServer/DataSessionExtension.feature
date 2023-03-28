@dataServer
Feature: DataSessionExtension

Validating responses on various values passed to DataSessionExtension

Scenario: Null exception thrown when criteria is null
	Given parameter Criteria is Null
	When DataSessionExtension is called
	Then null exception is thrown

Scenario: Null exception thrown when userd id is null
	Given parameter UserId is Null
	When DataSessionExtension is called
	Then null exception is thrown

Scenario: Null exception thrown when data session is null
	Given parameter DataSession is Null
	When DataSessionExtension is called
	Then null exception is thrown

Scenario: Appropriate response is got when data session calling query exists
	Given various parameters
	When DataServices is called
	Then appropriate response is shown
