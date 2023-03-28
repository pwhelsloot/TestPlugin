@sqlRelated
Feature: SQLQueryBuilder

Validating different SQL query builder cases

Scenario Outline: Build Simple Query
	Given a queryString <QueryType>
	When queryString <QueryType> is built using query builder
	Then actual result equals expected result <QueryType>

Examples:
	| QueryType                           |
	| SimpleQuery                         |
	| SimpleSelectClearedWhereQuery       |
	| SimpleSelectAddedWhereQuery         |
	| SimpleReplacedSelectAddedWhereQuery |
	| SimpleSelectReplacedWhereQuery      |
	| SelectMultipleWhereQuery            |