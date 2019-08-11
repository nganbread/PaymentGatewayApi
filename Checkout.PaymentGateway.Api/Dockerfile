FROM mcr.microsoft.com/dotnet/core/sdk:2.2-stretch AS publish
COPY / /src
WORKDIR /src
ENV ASPNETCORE_ENVIRONMENT=DockerBuild
RUN dotnet test
WORKDIR /src/Checkout.PaymentGateway.Api
RUN dotnet publish -c Release -o /app

FROM mcr.microsoft.com/dotnet/core/aspnet:2.2-stretch-slim
EXPOSE 80
COPY --from=publish /app /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Checkout.PaymentGateway.Api.dll"]