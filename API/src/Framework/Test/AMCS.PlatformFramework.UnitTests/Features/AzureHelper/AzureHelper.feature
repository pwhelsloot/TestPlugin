@azureHelper
Feature: AzureHelper

Validating various cases of helpers when working with Azure services

Scenario: Check running on app service
	Given no app service running
	And environment variable WEBSITE_HOSTNAME environment value a1-m76-svc-elemos.azurewebsites.net
	When environment variable is set to environment value
	Then app service starts running

Scenario Outline: GetSiteName
	Given environment variable <EnvironmentVariable> environment value <EnvironmentUrl>
	When environment variable is set to environment value
	Then site name equals <ExpectedValue> expected value

Examples:
	| EnvironmentVariable | EnvironmentUrl                              | ExpectedValue |
	| WEBSITE_HOSTNAME    | a1-m76-svc-elemos.azurewebsites.net         | a1-m76        |
	| WEBSITE_HOSTNAME    | a1-m76-svc-elemos-staging.azurewebsites.net | a1-m76        |
	
Scenario Outline: GetSlotName
	Given environment variable <EnvironmentVariable> environment value <EnvironmentUrl>
	And environment variable <EnvironmentVariable1> environment value <EnvironmentUrl1>
	When environment variable is set to environment value
	Then slot name equals <ExpectedValue> expected value

Examples:
	| EnvironmentVariable | EnvironmentUrl                                   | EnvironmentVariable1 | EnvironmentUrl1        | ExpectedValue |
	| WEBSITE_HOSTNAME    | a1-m76-svc-elemos-staging.azurewebsites.net      | WEBSITE_SITE_NAME    | a1-m76-svc-elemos      | staging       |
	| WEBSITE_HOSTNAME    | a1-m76-svc-elemos.azurewebsites.net              | WEBSITE_SITE_NAME    | a1-m76-svc-elemos      |               |
	| WEBSITE_HOSTNAME    | a1-m76-svc-vehicle-staging.azurewebsites.net"    | WEBSITE_SITE_NAME    | a1-m76-svc-vehicle     | staging       |
	| WEBSITE_HOSTNAME    | a1-m76-svc-integration-staging.azurewebsites.net | WEBSITE_SITE_NAME    | a1-m76-svc-integration | staging       |

Scenario Outline: GetExpectedDatabaseName
	Given environment variable <EnvironmentVariable> environment value <EnvironmentUrl>
	And environment variable <EnvironmentVariable1> environment value <EnvironmentUrl1>
	When environment variable is set to environment value
	Then database name equals <ExpectedValue> expected value

Examples:
	| EnvironmentVariable | EnvironmentUrl                                   | EnvironmentVariable1 | EnvironmentUrl1        | ExpectedValue               |
	| WEBSITE_HOSTNAME    | a1-m76-svc-elemos-staging.azurewebsites.net      | WEBSITE_SITE_NAME    | a1-m76-svc-elemos      | a1-m76-sqldb-elemos-staging |
	| WEBSITE_HOSTNAME    | a1-m76-svc-elemos.azurewebsites.net              | WEBSITE_SITE_NAME    | a1-m76-svc-elemos      | a1-m76-sqldb-elemos         |
	| WEBSITE_HOSTNAME    | a1-m76-svc-vehicle-staging.azurewebsites.net"    | WEBSITE_SITE_NAME    | a1-m76-svc-vehicle     | a1-m76-sqldb-elemos-staging |
	| WEBSITE_HOSTNAME    | a1-m76-svc-integration-staging.azurewebsites.net | WEBSITE_SITE_NAME    | a1-m76-svc-integration | a1-m76-sqldb-elemos-staging |

Scenario Outline: Setting site name as random generated instance gives correct result based on site prosence
	Given environment variable <EnvironmentVariable> environment value <RandomInstance>
	When environment variable is set to environment value
	Then generated instance name length equals expected length

Examples:
	| EnvironmentVariable | RandomInstance    |
	| WEBSITE_SITE_NAME   | a1-m76-svc-elemos |
	| WEBSITE_SITE_NAME   | b1-dev-svc-elemos |
	| WEBSITE_SITE_NAME   | <null>            |
	| WEBSITE_SITE_NAME   |                   |

Scenario Outline: Setting site name as random generated instance will not return duplicates
	Given environment variable <EnvironmentVariable> environment value <RandomInstance>
	When environment variable is set to environment value
	And random instances generated for <RandomInstanceCount> times
	Then generated instances will all be distinct

Examples:
	| EnvironmentVariable | RandomInstance    | RandomInstanceCount |
	| WEBSITE_SITE_NAME   | a1-m76-svc-elemos | 20                  |
	| WEBSITE_SITE_NAME   | b1-dev-svc-elemos | 60                  |
	| WEBSITE_SITE_NAME   | <null>            | 50                  |
	| WEBSITE_SITE_NAME   |                   | 35                  |

Scenario Outline: Setting site name as random generated instance will be of length less than 50
	Given environment variable <EnvironmentVariable> environment value <RandomInstance>
	When environment variable is set to environment value
	And random instances generated for <RandomInstanceCount> times
	Then generated instances will all be of length less than 50

Examples:
	| EnvironmentVariable | RandomInstance           | RandomInstanceCount |
	| WEBSITE_SITE_NAME   | a1-m76-svc-elemos        | 1                   |
	| WEBSITE_SITE_NAME   | b1-dev-svc-elemos        | 1                   |
	| WEBSITE_SITE_NAME   | eu1-workflowsrvr-dev-app | 1                   |
	| WEBSITE_SITE_NAME   | <null>                   | 1                   |
	| WEBSITE_SITE_NAME   |                          | 1                   |