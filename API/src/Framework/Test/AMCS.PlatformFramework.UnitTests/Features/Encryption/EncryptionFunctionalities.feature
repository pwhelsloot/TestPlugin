@encryption
Feature: EncryptionFunctionalities

Validating various encryption decryption techniques

Scenario Outline: Simple round trip
	Given an <InputString> input string
	When encryption service <EncryptionMode> is started
	Then decrypted value of encrypted matches input string

Examples:
	| InputString  | EncryptionMode |
	| Hello world! | BouncyCastle   |
	| Hello world! | NETFramework   |

Scenario Outline: Round trip performance
	Given an <InputString> input string
	When encryption service <EncryptionMode> is started
	And encryption runs continuously for a second
	Then more than 1000 iterations of encryption had happened

Examples:
	| InputString  | EncryptionMode |
	| Hello world! | BouncyCastle   |
	| Hello world! | NETFramework   |

Scenario: Encrypt using legacy encryptor decrypt using BouncyCastle
	Given an Hello world! input string
	When legacy encryption service is used to encrypt
	And BouncyCastle encryption service is used to decrypt
	Then decrypted value of encrypted matches input string

Scenario: Encrypt using string encryptor decrypt using BouncyCastle
	Given an Hello world! input string
	When string encryption service is used to encrypt
	And BouncyCastle encryption service is used to decrypt
	Then decrypted value of encrypted matches input string