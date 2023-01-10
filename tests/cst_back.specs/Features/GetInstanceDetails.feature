Feature: GetInstanceDetails

Allow a user to get a details of an instance by his id

Scenario: get instance details by id
	Given I am a user
	When I got an instance with id "1"
	And I need his details
	Then I got details
