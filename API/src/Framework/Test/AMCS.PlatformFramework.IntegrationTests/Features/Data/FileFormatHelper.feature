@dataSupport
Feature: FileFormatHelper

Validating correct data is returned on using various image formats

Scenario Outline: Correct results are returned on checking format of provided image
	Given an image <Image> image format <ImageFormat>
	When format is fetched with file format validator
	Then correct format is returned as <ExpectedBase64Format> expected base64 format

Examples:
	| Image     | ImageFormat | ExpectedBase64Format    |
	| JpgBase64 | .jpeg       | data:image/jpeg;base64, |
	| GifBase64 | .gif        | data:image/gif;base64,  |
	| PngBase64 | .png        | data:image/png;base64,  |

Scenario Outline: Correct results are returned on checking format of provided image bytes
	Given sample image byte array <ImageBytes> expected format enum <ExpectedFormatEnum>
	When format is fetched with file format validator
	Then correct format is returned as <ExpectedFormatEnum> expected format enum

Examples:
	| ImageBytes                          | ExpectedFormatEnum |
	| FF,D8,FF,DB                         | Jpeg               |
	| FF,D8,FF,EE                         | Jpeg               |
	| FF,D8,FF,E0,00,10,4A,46,49,46,00,01 | Jpeg               |
	| FF,D8,FF,E1,AA,AA,45,78,69,66,00,00 | Jpeg               |
	| FF,D8,FF,E1,FF,FF,45,78,69,66,00,00 | Jpeg               |
	| 47,49,46,38,37,61                   | Gif                |
	| 47,49,46,38,39,61                   | Gif                |
	| 89,50,4E,47,0D,0A,1A,0A             | Png                |
	| 42,4D                               | Bmp                |
	| FF,D8,DB,DB                         | NotSupported       |
	| FF,D8,FF,E1                         | NotSupported       |



