Feature: SearchInstances

Allow user to search instances by string


Scenario: Search Instances by string
	Given I am a user
	And I searching instances with "<string>"
	When I want all instances
	Then I get a list of instances
	And They all contains "<string>"

	Examples: 
	| string  |
	| Victory |
