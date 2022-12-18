Feature: CreateAccount

Allow a user to create an account

Scenario: Create Account
	Given As a user
	And My username is "<username>"
	And My email  is "<email>"
	And My password is "<password>"
	And My password confirmation is "<confPassword>"
	When I want to create my  account
	Then I should receive a response  <response>

	Examples: 
	| username | email   | password | confPassword | response |
	| aaa      | a@a.com | aaaaaaaa | aaaaaaaa     | 400      |
	| aaaa     | a.a.com | aaaaaaaa | aaaaaaaa     | 400      |
	| aaaa     | a@a.com | aaaa     | aaaa         | 400      |
	| aaaa     | a@a.com | aaaaaaaa | aaaa         | 400      |
	| aaaa     | a@a.com | aaaaaaaa | aaaaaaaa     | 200      |