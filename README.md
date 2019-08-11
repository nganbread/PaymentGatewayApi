# Checkout.com Payment Gateway API

## Get Started

1. Navigate to `/Fake/Checkout.PaymentGateway.Fake.AcquiringBank.Api`
1. Execute `dotnet run`
1. In another console, navigate to `/Checkout.PaymentGateway.Api/`
1. Execute `dotnet run`
1. To run integration tests:
    1. In another console, navigate to `/Tests/Checkout.PaymentGateway.Api.Tests.Integration/`
    1. Execute `dotnet test`
1. Access the API on `http://localhost:5000`

or using docker

1. Navigate to `/`
1. Execute `docker-compose build --parallel`
1. Execute
    - with integration tests: `docker-compose up`
    - without integration tests: `docker-compose up payment-gateway-api`
1. Access the API on `http://localhost:4000`

## API Documentation

Start the API and navigate to `/swagger`

## Design

- Idempotency
    - The merchant is expected to provide a unique identifier for their payment. This is to ensure that payments are not processed twice.
    - The unique identifier only needs to be unique for that merchant.
- Event Sourcing
	- Even though events are all fired in a single method, as the payment flow becomes more complicated (sanctions screening, validation etc) it is 'simple' to move these events around to different services/APIs

## Limitations

- Domain
    - The domain classes (`Payment`) does not perform full validation to ensure that it is always in a consistent state
    - A finite state model library could be used to simplify the code

## TODO/Beyond Current Scope

- HTTPS
- Authentication/Authorization
- Secure Encryption
- Persistent Data Store