FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS publish
COPY /Fake/Checkout.PaymentGateway.Fake.AcquiringBank.Api /src
WORKDIR /src
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
EXPOSE 80
COPY --from=publish /app /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Checkout.PaymentGateway.Fake.AcquiringBank.Api.dll"]