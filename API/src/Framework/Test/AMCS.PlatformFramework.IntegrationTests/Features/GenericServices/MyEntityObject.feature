@genericServices
Feature: MyEntityObject

Validating the type and id of the provided input is returned correctly

Scenario Outline: Correct results are shown when passing various entity object
	Given an <EntityObjectService> entity object service <EntityObject> entity object
	When entity object service <EntityObjectService> entity object <EntityObject> is passed to data services
	Then ID <ID> and type <EntityObject> matches expected result

Examples:
	| EntityObjectService      | EntityObject    | ID |
	| MyEntityObjectService    | MyEntityObject1 | 43 |
	| MyEntityObjectService    | MyEntityObject2 | 42 |
	| MyEntityObjectService    | MyEntityObject3 | 44 |
	| MyEntityObjectService3   |                 | 0  |
	| MySQLEntityObjectAccess1 |                 | 0  |
	| MySQLEntityObjectAccess  | MyEntityObject2 | 0 |
	| MySQLEntityObjectAccess  | MyEntityObject3 | 0 |