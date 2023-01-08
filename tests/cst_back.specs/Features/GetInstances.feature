Feature: GetInstances

Feature to allow a user to get all instances

Scenario: GetInstances
	Given I am a user
	When I want all instances
	Then I get a list of instances

Scenario: Get Instances with filter
	Given I am a user
	And I'm focus on "<type>" "<value>" 
	When I want all instances
	Then I get a list of instances
	And Their "<type>" are only "<value>"

	Examples: 
	| type         | value              |
	| Goal         | Scientific Victory |
	| Map          | Seven Seas         |
	| Civilization | Dido               |
