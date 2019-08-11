FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS publish
COPY / /src
WORKDIR /src/Tests/Checkout.PaymentGateway.Api.Tests.Integration
RUN dotnet build
CMD dotnet test --no-build