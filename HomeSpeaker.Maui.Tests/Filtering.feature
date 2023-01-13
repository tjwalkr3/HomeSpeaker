Feature: Filtering

A short summary of the feature

@tag1
Scenario: Filter Songs
	Given the following songs in the Folder1 folder
	| Album  | Artist  | Name  |
	| Album1 | Artist1 | Song1 |
	And the following songs in the Folder2 folder
	| Album  | Artist  | Name  |
	| Album2 | Artist2 | Song2 |
	And I load all the songs
	When I set the filter text to 1
	And click the Filter button
	Then I see the following songs
	| Album  | Artist  | Folder  | Name  |
	| Album2 | Artist2 | Folder2 | Song2 |

