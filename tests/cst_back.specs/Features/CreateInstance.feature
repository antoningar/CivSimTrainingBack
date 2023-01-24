Feature: CreateInstance

Featureto allow a client to create an  instance

@tag1
Scenario: Create an instance
	Given I am a user
	And my username is "bahtiens"
	And my goal is "max science turn 50"
	When I want t create my instances
	Then I receive my instance id
