@dataServer
Feature: PluginUpdateNotificationService

Validating functinality of Plugin Update Notification Service on various situations

Scenario: Plugin update notification service will handle multiple tenants
	Given multiple http clients running
	When plugin update notification service is initiated
	Then expected results are shown

Scenario: Plugin update notification service will retry until successful
	Given multiple http clients running and 4 retry attempts
	When plugin update notification service is initiated and disposed
	Then retry is attempted by 4 retry attempts provided
