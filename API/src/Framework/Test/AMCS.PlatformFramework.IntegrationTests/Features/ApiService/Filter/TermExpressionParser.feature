@apiServiceFilter
Feature: TermExpressionParser

Validating parse output of various data types

Scenario Outline: ParseStringTerm
	Given various <StringTerm> strings <ExpectedResult> expected result
	When expression built with input strings
	Then output matches expected result

Examples:
	| StringTerm  | ExpectedResult                     |
	| abc123      | stringProp eQ 'abc123'             |
	| def456      | stringProp			NE		     'def456'     |
	| abc         | stringProp startsWITh 'abc'        |
	| c12         | stringProp conTains 'c12'          |
	| 123         | stringProp endswith '123'          |
	| hello,there | stringProp IN ('hello', 'there')   |
	|             | stringProp lt 5                    |
	|             | stringProp lte 5                   |
	|             | stringProp gt 5                    |
	|             | stringProp gte 5                   |
	|             | invalidProp eq 'abc123'            |
	|             | stringProp between 'abc' and 'def' |

Scenario Outline: ParseIntegerTerm
	Given various <IntegerTerm> integer <ExpectedResult> expected result
	When expression built with input integer
	Then output matches expected result

Examples:
	| IntegerTerm   | ExpectedResult            |
	| 851           | intProp eq 851            |
	| 853           | intProp ne 853            |
	| 853           | intProp lt 853            |
	| 853           | intProp lte 853           |
	| 853           | intProp gt 853            |
	| 853           | intProp gte 853           |
	| 1, 2, 3, 4, 5 | intProp in (1,2,3, 4,		5) |
	| 10,3000       | intProp from 10 to   3000 |
	|               | intProp startswith 1      |
	|               | intProp contains 1        |
	|               | intProp endswith 1        |
	|               | intProp eq 'abc'          |

Scenario Outline: ParseDoubleTerm
	Given various <DoubleTerm> double <ExpectedResult> expected result
	When expression built with input double
	Then output matches expected result

Examples:
	| DoubleTerm              | ExpectedResult                           |
	| 12.34                   | doubleProp eq 12.34                      |
	| 34.567                  | doubleProp ne 34.567                     |
	| 34.567                  | doubleProp lt 34.567                     |
	| 34.567                  | doubleProp lte 34.567                    |
	| 34.567                  | doubleProp gt 34.567                     |
	| 34.567                  | doubleProp gte 34.567                    |
	| 1.1, 2.2, 3, 4.4, 5.555 | doubleProp in (1.1, 2.2, 3, 4.4,		5.555) |
	| 10.125,3000.985         | doubleProp FrOm 10.125 tO  3000.985      |
	|                         | doubleProp startswith 1                  |
	|                         | doubleProp contains 1                    |
	|                         | doubleProp endswith 1                    |
	|                         | doubleProp eq 'abc'                      |

Scenario Outline: ParseDecimalTerm
	Given various <DecimalTerm> decimal <ExpectedResult> expected result
	When expression built with input decimal
	Then output matches expected result

Examples:
	| DecimalTerm                  | ExpectedResult                            |
	| 12.34m                       | decimalProp eq 12.34                      |
	| 34.567m                      | decimalProp ne 34.567                     |
	| 34.567m                      | decimalProp lt 34.567                     |
	| 34.567m                      | decimalProp lte 34.567                    |
	| 34.567m                      | decimalProp gt 34.567                     |
	| 34.567m                      | decimalProp gte 34.567                    |
	| 1.1m, 2.2m, 3m, 4.4m, 5.555m | decimalProp in (1.1, 2.2, 3, 4.4,		5.555) |
	| 10.125m,3000.985m            | decimalProp FrOm 10.125 tO  3000.985      |
	|                              | decimalProp startswith 1                  |
	|                              | decimalProp contains 1                    |
	|                              | decimalProp endswith 1                    |
	|                              | decimalProp eq 'abc'                      |

Scenario Outline: ParseDateTimeTerm
	Given various <DateTimeTerm> dateTime <ExpectedResult> expected result
	When expression built with input DateTime dateTime
	Then output matches expected result
	# in Below example 7 and 8, given only one date, other dates will be created in the step definition to reduce the large of conversions from string to int and then to Datetime
Examples:
	| DateTimeTerm | ExpectedResult                                          |
	| 2017, 12, 28 | dateProp eq '2017-12-28'                                |
	| 2017, 12, 22 | dateProp ne '2017-12-22'                                |
	| 2017, 12, 22 | dateProp lt '2017-12-22'                                |
	| 2017, 12, 22 | dateProp lte '2017-12-22'                               |
	| 2017, 12, 22 | dateProp gt '2017-12-22'                                |
	| 2017, 12, 22 | dateProp gte '2017-12-22'                               |
	| 2017, 12, 21 | dateProp in ('2017-12-21','2017-12-22',   '2017-12-23') |
	| 2017, 11, 15 | dateProp from '2017-11-15' to '2017-12-22'              |
	|              | dateProp startswith '2017-12-22'                        |
	|              | dateProp contains '2017-12-22'                          |
	|              | dateProp endswith '2017-12-22'                          |
	|              | dateProp eq 'abc'                                       |

Scenario Outline: ParseLocalDateTimeTerm
	Given various <LocalDateTimeTerm> dateTime <ExpectedResult> expected result
	When expression built with input LocalDateTime dateTime
	Then output matches expected result
	# in Below example 7 and 8, given only one date, other dates will be created in the step definition to reduce the large of conversions from string to int and then to Datetime
Examples:
	| LocalDateTimeTerm | ExpectedResult                                               |
	| 2017, 12, 28      | localDateProp eq '2017-12-28'                                |
	| 2017, 12, 22      | localDateProp ne '2017-12-22'                                |
	| 2017, 12, 22      | localDateProp lt '2017-12-22'                                |
	| 2017, 12, 22      | localDateProp lte '2017-12-22'                               |
	| 2017, 12, 22      | localDateProp gt '2017-12-22'                                |
	| 2017, 12, 22      | localDateProp gte '2017-12-22'                               |
	| 2017, 12, 21      | localDateProp in ('2017-12-21','2017-12-22',   '2017-12-23') |
	| 2017, 11, 15      | localDateProp from '2017-11-15' to '2017-12-22'              |

Scenario Outline: ParseZonedDateTerm
	Given two dates dateOne 2017-12-08 dateTwo 2017-12-28 expected result <ExpectedResult>
	When these dates are converted to a particular ZonedDate
	And expression built with input ZonedDateTime dateTime
	Then output matches expected result
	
Examples:
	| ExpectedResult                                      |
	| ZonedDateTimeProp eq '2017-12-08'                   |
	| ZonedDateTimeProp ne '2017-12-08'                   |
	| ZonedDateTimeProp lt '2017-12-08'                   |
	| ZonedDateTimeProp lte '2017-12-08'                  |
	| ZonedDateTimeProp gt '2017-12-08'                   |
	| ZonedDateTimeProp gte '2017-12-08'                  |
	| ZonedDateTimeProp in ('2017-12-08','2017-12-28')    |
	| ZonedDateTimeProp from '2017-12-08' to '2017-12-28' |

Scenario Outline: ParseZonedDateTimeTerm
	Given two dates dateOne 2017-12-08T18:43:09.477+01:00 dateTwo 2017-12-28T18:43:09.477+01:00 expected result <ExpectedResult>
	When these dates are converted to a particular ZonedDateTime
	And expression built with input ZonedDateTime dateTime
	Then output matches expected result
	
Examples:
	| ExpectedResult                                                                            |
	| ZonedDateTimeProp eq '2017-12-08T18:43:09.477+01:00'                                      |
	| ZonedDateTimeProp ne '2017-12-08T18:43:09.477+01:00'                                      |
	| ZonedDateTimeProp lt '2017-12-08T18:43:09.477+01:00'                                      |
	| ZonedDateTimeProp lte '2017-12-08T18:43:09.477+01:00'                                     |
	| ZonedDateTimeProp gt '2017-12-08T18:43:09.477+01:00'                                      |
	| ZonedDateTimeProp gte '2017-12-08T18:43:09.477+01:00'                                     |
	| ZonedDateTimeProp in ('2017-12-08T18:43:09.477+01:00','2017-12-28T18:43:09.477+01:00')    |
	| ZonedDateTimeProp from '2017-12-08T18:43:09.477+01:00' to '2017-12-28T18:43:09.477+01:00' |

Scenario Outline: ParseBooleanTerm
	Given various <BooleanTerm> boolean <ExpectedResult> expected result
	When expression built with input boolean
	Then output matches expected result

Examples:
	| BooleanTerm | ExpectedResult                  |
	| true        | boolProp eq true                |
	| false       | boolProp eq false               |
	| true        | boolProp ne true                |
	| false       | boolProp ne false               |
	|             | boolProp lt true                |
	|             | boolProp lte true               |
	|             | boolProp gt true                |
	|             | boolProp gte true               |
	|             | boolProp startswith true        |
	|             | boolProp contains false         |
	|             | boolProp endswith true          |
	|             | boolProp eq 'abc'               |
	|             | boolProp in (true,false)        |
	|             | boolProp between true and false |

Scenario Outline: ParseEnumTerm
	Given various <EnumTerm> enum <ExpectedResult> expected result
	When expression built with input enum
	Then output matches expected result

Examples:
	| EnumTerm | ExpectedResult            |
	| abc      | enumProp eq 'abc'         |
	| abc,def  | enumProp in ('abc','def') |
	| 0        | enumProp eq 0             |