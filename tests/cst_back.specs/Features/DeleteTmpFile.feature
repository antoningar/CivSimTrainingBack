Feature: DeleteTmpFile

Allow client to delete all tmp files by usewr id

@tag1
Scenario: Delete tmp files
	Given I am a client
	And My username is "sil2ob"
	When I want to delete my tmp files
	Then My files are deleted
