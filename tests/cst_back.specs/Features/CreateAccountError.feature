Feature: CreateAccountError

When creating an account, some validators should check if the account should be created

Scenario: Create Account
	Given As a user
	And My username is "<username>"
	And My email  is "<email>"
	And My password is "<password>"
	And My password confirmation is "<confPassword>"
	When I want to create my  account, I shouldn't received my id

	Examples: 
	| username | email   | password | confPassword |
	| aa      | a@a.com | aaaaaaaa | aaaaaaaa     |
	| aaaa     | a.a.com | aaaaaaaa | aaaaaaaa     |
	| aaaa     | a@a.com | aaaa     | aaaa         |
	| aaaa     | a@a.com | aaaaaaaa | aaaa         |