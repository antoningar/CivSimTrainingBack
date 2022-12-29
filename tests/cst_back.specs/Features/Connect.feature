Feature: Connect

A user must be able to connect

Scenario: Connect to my account
	Given I am a user
	And My ids are "<username>" / "<password>"
	When I login
	Then I have access to my account

	Examples: 
	| username | password |
	| sil2ob   | aaaaaaaa |
