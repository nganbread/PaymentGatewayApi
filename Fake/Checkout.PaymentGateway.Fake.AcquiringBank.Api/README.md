# Fake Acquiring Bank API

This fake API is hard coded to return specific status codes and data depending on the provided request. Example request:

```json
{
	"CardNumber" : "1234567890123456"
}
```

Responses are determined by the last digit of the card number:

| Last Digit | Response Code  | Description 						|
|-------------|---------------|-------------------------------------|
| 1			  | 201			  | Returns {"PaymentId":"<guid>"}		|
| 2			  | 500			  | 									|
| Other		  | 400			  | 									|

Try these valid credit cards numbers:

- `4929395704290281`
- `4716521177828292`
- `4929087984766953`
- `5442263024015504`
- `6011150598328265`
- `4532444939089776`
- `5102828446302077`
- `4929474760910478`
- `6011673967121599`