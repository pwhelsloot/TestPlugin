@entityMapping
Feature: EntityMapping

Validating different entity mapping cases

Scenario Outline: Correct results are shown for various entity types mapping
	Given entity <EntityType> mapping type <MappingType>
	When entity is mapped with target <MappingType> mapping entity
	Then correct results are shown

Examples:
	| MappingType      | EntityType             | MappingEntity                                |
	| Same             | SimpleEntity           | SimpleEntity                                 |
	| Simple           | SimpleTargetEntity     | SimpleEntity                                 |
	| Nullable         | SimpleTargetEntity     | SimpleEntity                                 |
	| SpecificProperty | SimpleTargetEntity     | SimpleEntity                                 |
	| BeforeMap        | SimpleTargetEntity     | SimpleEntity                                 |
	| AfterMap         | SimpleTargetEntity     | SimpleEntity                                 |
	| Callback         | SimpleTargetEntity     | SimpleEntity                                 |
	| Nested           | WithNestedSourceEntity | WithNestedTargetEntityWithNestedTargetEntity |