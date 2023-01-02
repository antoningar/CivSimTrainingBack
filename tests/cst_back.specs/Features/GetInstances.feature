Feature: GetInstances

Feature to allow a user to get all instances

Scenario: GetInstances
	Given I am a user
	When I want all instances
	Then I get a list of instances
