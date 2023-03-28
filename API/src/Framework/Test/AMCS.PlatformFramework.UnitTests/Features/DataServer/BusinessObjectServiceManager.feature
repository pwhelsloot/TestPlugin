@dataServer
Feature: BusinessObjectServiceManager

Validating exceptions are thrown when missing a value on calling BusinessObjectServiceManager

Scenario: Null exception thrown when criteria is null
	Given parameter Criteria is Null
	When BusinessServiceManager is called
	Then null exception is thrown

Scenario: Null exception thrown when userd id is null
	Given parameter UserId is Null
	When BusinessServiceManager is called
	Then null exception is thrown

Scenario: Null exception thrown when data session is null
	Given parameter DataSession is Null
	When BusinessServiceManager is called
	Then null exception is thrown