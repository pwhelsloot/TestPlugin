@mexPostProcessing
Feature: MexPostProcessing
	Allows plugins to add post processing to the plugin metadata xml during mex

Scenario: Mex is performed between a multi tenant plugin and core
	Given the core version is <core version>
	And the plugin version is <plugin version>
	When executing the get metadata mex post processing
	Then then the reverse proxy rules have AcceptsOAuthToken added
Examples: 
  | plugin version | core version |
  | 8.9.0.0        | 8.9.0.0      |
  | 8.9.0.0        | 8.10.0.0     |
  | 8.9.0.0        | 8.11.0.0     |
  | 8.10.0.0       | 8.9.0.0      |
  | 8.10.0.0       | 8.10.0.0     |
  | 8.10.0.0       | 8.11.0.0     |
  | 8.11.0.0       | 8.9.0.0      |
  | 8.11.0.0       | 8.10.0.0     |
  | 8.11.0.0       | 8.11.0.0     |
