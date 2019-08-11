using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Checkout.PaymentGateway.Api.Tests.Unit.Startup
{
    public class ContainerTests
    {
        private readonly ServiceCollection _serviceCollection;
        private readonly Mock<IConfiguration> _configuration;

        public ContainerTests()
        {
            _serviceCollection = new ServiceCollection();
            _configuration = new Mock<IConfiguration>();

            var configurationSection = new Mock<IConfigurationSection>();

            _configuration
                .Setup(x => x.GetSection(It.IsAny<string>()))
                .Returns(configurationSection.Object);

            new Api.Startup(_configuration.Object).ConfigureServices(_serviceCollection);
        }

        [Theory]
        [MemberData(nameof(ControllerTypes))]
        public void GivenStartupHasConfiguredTheServiceCollection_ThenTheServiceProviderCanResolveTheController(Type controllerType)
        {
            //Controllers are automatically discovered, so to test the container we need to add it to the service collection
            _serviceCollection.AddTransient(controllerType, controllerType);

            var serviceProvider = _serviceCollection.BuildServiceProvider();

            serviceProvider.GetRequiredService(controllerType);
        }

        public static IEnumerable<object[]> ControllerTypes =>
            typeof(Api.Startup)
                .Assembly
                .GetTypes()
                //Might be possible to use the ApiExplorer to find controller types
                .Where(x => x.Name.EndsWith("Controller"))
                .Select(x => new object[] {x});
    }
}
