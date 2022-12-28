Feature: CreateAccount

Allow a user to create an account

Scenario: Create Account
	Given As a user
	And My username is "<username>"
	And My email  is "<email>"
	And My password is "<password>"
	And My password confirmation is "<confPassword>"
	When I want to create my  account
	Then I should receive a response my id

	Examples: 
	| username | email   | password | confPassword |
	| aaaa     | a@a.com | aaaaaaaa | aaaaaaaa     |