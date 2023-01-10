Feature: SearchInstances

Allow user to search instances by string


Scenario: Search Instances by string
	Given I am a user
	And I searching instances with "<string>"
	When I want all instances
	Then They all contains "<string>"

	Examples: 
	| string  |
	| Victory |
