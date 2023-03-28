@dataSupport
Feature: ReflectionHelper

Validating correct data is returned on using various input types

Scenario Outline: GetDefaultValueFactory returns correct value
	Given disableILGeneration <DisableILGeneration> or not
	When GetDefaultValueFactory is called
	Then result matches expected result

Examples:
	| DisableILGeneration |
	| false               |
	| true                |

Scenario Outline: GetPropertyGetter returns correct value
	Given disableILGeneration <DisableILGeneration> or not
	When GetPropertyGetter is called with int property <IntValue>
	Then result matches expected result

Examples:
	| DisableILGeneration | IntValue |
	| false               | 0        |
	| true                | 0        |
	| false               | 75       |
	| true                | 75       |
	| false               | 9499     |
	| true                | 9499     |

Scenario Outline: GetPropertySetter for property without set throws not supported exception
	Given disableILGeneration <DisableILGeneration> or not
	When GetPropertySetter is called without any property set
	Then not supported exception is thrown

Examples:
	| DisableILGeneration |
	| false               |
	| true                |

Scenario Outline: GetPropertySetter returns correct value
	Given disableILGeneration <DisableILGeneration> or not
	When GetPropertySetter is called with int property <IntValue>
	Then result matches expected result

Examples:
	| DisableILGeneration | IntValue |
	| false               | 0        |
	| true                | 0        |
	| false               | 15       |
	| true                | 15       |
	| false               | 9999     |
	| true                | 9999     |

Scenario Outline: GetEntityPropertySetter returns correct value
	Given disableILGeneration <DisableILGeneration> or not
	When GetEntityPropertySetter is called with int and null value
	Then result matches expected result

Examples:
	| DisableILGeneration |
	| false               |
	| true                |
	
Scenario Outline: GetEntityPropertySetter for char property will coerce from string
	Given disableILGeneration <DisableILGeneration> or not
	When GetEntityPropertySetter is called with input data type <InputDataType> and value <InputValue>
	Then result matches <ExpectedResult> expected result

Examples:
	| DisableILGeneration | InputDataType         | InputValue | ExpectedResult |
	| true                | SingleCharacter       | C          | C              |
	| true                | SingleCharacterString | C          | C              |
	| true                | MultiCharacterString  | Characters | C              |
	| true                | Integer               | 5          | 5              |
	| false               | SingleCharacter       | C          | C              |
	| false               | SingleCharacterString | C          | C              |
	| false               | MultiCharacterString  | Characters | C              |
	| false               | Integer               | 5          | 5              |
	