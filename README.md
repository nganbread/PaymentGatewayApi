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

Follow [Get Started](#Get-Started) and navigate to `/swagger`

## Design

- Idempotency
    - The merchant is expected to provide a unique identifier for their payment. This is to ensure that payments are not processed twice.
    - The unique identifier only needs to be unique for that merchant.
- Event Sourcing
	- Even though events are all fired in a single method, as the payment flow becomes more complicated (sanctions screening, validation etc) it is 'simple' to move these events around to different services/APIs
- Versioning
    - The API and documentation is versioned
- Integration Tests
    - Integration tests can be executed completely within the test process. This is enabled by default, but can be toggled on/off in `appsettings.integration.Development.json`
        - Set `IntegrationTests.InMemoryPaymentGatewayApi = true` to run the payment gateway API as an in-memory test server
        - If `IntegrationTests.InMemoryPaymentGatewayApi = false` then `PaymentGatewayApiUrl` is required.
        - Set `IntegrationTests.InMemoryFakeAcquiringBankApi = true` to configure the the acquiring bank API as an in-memory test server
    - The tests make use of an `IHostBuilder` (see `IntegrationTestHostBuilder`) to manage the integration test services, logging and configuration. While we are not 'hosting' anything, using these dotnet APIs makes it easier to work with nuget packages (eg `IHttpClientFactory`) and keeps it more consistent with the rest of the application
- Fake Acquiring Bank
    - The fake API is hard coded to return specific responses based on the `CardNumber` found in the request body.
    - To swap out the API in production, `AcquiringBankApi.Uri` can be set in `appsettings.json`
    - See `/Fake/Checkout.PaymentGateway.Fake.AcquiringBank.Api/README.md`
- Docker
    - `docker-compose` has been utilised to build, run and test the application.
    - There are 3 services which are configured with docker-compose
        - `payment-gateway-api`
        - `fake-acquiring-bank-api`
        - `integration-tests`
    - When `integration-tests` is started it will automatically execute the integration tests against the API, which is then set up to communicate with the faked acquiring bank.
- Documentation
    - API documentation is automatically discovered and made accessible by swagger
    - Configuring the code to be auto discoverable by swagger can be difficult sometimes

## Limitations

- Domain
    - The domain classes (`Payment`) does not perform full validation to ensure that it is always in a consistent state
    - A finite state model library could be used to simplify the code
- Data Storage
    - Data is stored in memory and is not persisted
- Cross-Platform
    - Everything has been written to be cross-platform but has only been tested on Windows
- Authentication/Authorization
    - Proper authentication/authorization has not been fully implemented
    - To authenticate with the API set the `Authorize` request header to a/your/the merchants name
- HTTPS
    - The API does not use HTTPs
- CI/CD
    - This has not been implemented, however the build should work everywhere that docker is supported