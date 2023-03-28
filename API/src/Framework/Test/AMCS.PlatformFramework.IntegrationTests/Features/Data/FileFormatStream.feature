@dataSupport
Feature: FileFormatStream

Validating correct data is returned on using various image format streams

Scenario Outline: Correct results are returned on checking format of provided image stream
	Given an image <Image> image stream <ImageStream>
	When image stream is read using file format validator
	Then correct image stream base64 format matches expected result <ExpectedBase64Format>

Examples:
	| Image     | ImageStream | ExpectedBase64Format    |
	| JpgBase64 | .jpeg       | data:image/jpeg;base64, |
	| GifBase64 | .gif        | data:image/gif;base64,  |
	| PngBase64 | .png        | data:image/png;base64,  |

Scenario Outline: Correct results are returned on reading provided image stream
	Given an <Image> image 
	When image is read with file stream format validator
	Then correct results are returned

Examples:
	| Image     |
	| JpgBase64 |
	| GifBase64 |
	| PngBase64 |

	Scenario: Calling file format stream constructor with Null throws NullException
	Given no image
	When file format stream is called
	Then exception is thrown

	Scenario: Calling file format stream constructor with non zero position throws exception
	Given an JpgBase64 image 
	When file format stream is called with non zero position
	Then exception is thrown
	