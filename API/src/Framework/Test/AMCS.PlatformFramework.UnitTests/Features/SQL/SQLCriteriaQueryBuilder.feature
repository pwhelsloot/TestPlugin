@sqlRelated
Feature: SQLCriteriaQueryBuilder

Validating different SQL criteria query builder cases

@tag1
Scenario Outline: Corresponding ouput shown based on provided criteria query type
	Given criteria query type <CriteriaQueryType>
	When criteria <CriteriaQueryType> is passed query builder
	Then corresponding results are shown based on <CriteriaQueryType>

Examples:
	| CriteriaQueryType |
	| Select            |
	| Count             |
	| Exists            |
	| ExistsHasOrderBy  |
	| CountHasOrderBy   |
	| SelectHasOrderBy  |
	| ExistWithFetch    |
	| CountWithFetch    |
